using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beloning.Data.UnitOfWork;
using Beloning.Model;
using Beloning.Model.Enum;
using Beloning.Services.Base;
using Beloning.Services.Contracts;
using Beloning.Services.Model;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Beloning.Services
{
    public class ReferralService : BaseService, IReferralService
    {
        private readonly IConfiguration _configuration;
        private static ISearchServiceClient _searchClient;

        public ReferralService(IUnitOfWork unitOfWork, ILogger<BaseService> logger, IMapper mapper, IConfiguration configuration) : base(unitOfWork, logger, mapper)
        {
            _configuration = configuration;
            _searchClient = new SearchServiceClient(configuration["SearchServiceName"], new SearchCredentials(configuration["SearchServiceKey"]));
        }

        public async Task<ResponseDto> Add(ReferralDto referral)
        {
            var user = await UnitOfWork.UserRepository.GetByMail(referral.User.Email);
            if (user == null)
            {
                user = User.CreateUser(referral.User.Email, referral.User.Name, (Language)referral.User.Language);
                await UnitOfWork.UserRepository.Add(user);
                UnitOfWork.SaveChanges();
            }

            var patient = await UnitOfWork.PatientRepository.GetByNihii(referral.Patient.Nihii);
            if (patient == null)
            {
                patient = Patient.CreatePatient(referral.Patient.Email, referral.Patient.Nihii, referral.Patient.Name, (Language)referral.Patient.Language, (Gender)referral.Patient.Gender,
                referral.Patient.DateOfBirth, referral.Patient.Remarks);
                await UnitOfWork.PatientRepository.Add(patient);
                UnitOfWork.SaveChanges();
            }

            var newReferral = Referral.CreateReferral(user.Id, patient.Id, ReferralStatus.Open);
            await UnitOfWork.ReferralRepository.Add(newReferral);

            foreach (var fileName in referral.FileNames)
            {
                newReferral.AddFile(ReferralFile.CreateReferralFile(newReferral.Id, $"{patient.AnonymizationId}-{fileName}"));
            }
            UnitOfWork.SaveChanges();
            await CopyBlobItems(patient.Nihii, patient.AnonymizationId, referral.FileNames);
            //await AddMessageToQueueAsync(JsonConvert.SerializeObject(new { user = user, patient = patient, referral = newReferral }));
            _searchClient.Indexers.Run(_configuration["IndexerName"]);
            await CheckIndexerStatus(newReferral.ReferralFiles.Select(p => p.FileName).ToArray(), patient.AnonymizationId, newReferral.Id);
            return ResponseDto.CreateSuccessResponseDto();
        }

        public async Task<ResponseDto<DashboardDto>> Update(int id, int status)
        {
            var referral = await UnitOfWork.ReferralRepository.Get(id);
            referral.UpdateStatus((ReferralStatus)status);
            UnitOfWork.SaveChanges();
            return await GetDashboardInformation();
        }

        public async Task<ResponseDto<DashboardDto>> GetDashboardInformation()
        {
            var referrals = await UnitOfWork.ReferralRepository.Find(0, 20);
            var dashboard = new DashboardDto
            {
                TotalReferrals = referrals.Count(),
                OpenReferrals = referrals.Count(p => p.Status == ReferralStatus.Open),
                DeniedReferrals = referrals.Count(p => p.Status == ReferralStatus.Denied),
                ResolvedReferrals = referrals.Count(p => p.Status == ReferralStatus.Resolved),
                ReferralInfo = new List<ReferralInfo>()
            };
            
            foreach (var referral in referrals)
            {
                dashboard.ReferralInfo.Add(new ReferralInfo
                {
                    Id = referral.Id,
                    CreatedOn = referral.CreatedOn,
                    PatientName = referral.Patient.Name,
                    PhysicianName = referral.User.Name,
                    Remarks = referral.Patient.Remarks,
                    Status = (int)referral.Status
                });

                if (referral.Patient.Gender == Gender.Female) {
                    dashboard.FemaleRatio += 1;
                } else {
                    dashboard.MaleRatio += 1; 
                }
            }

            return ResponseDto<DashboardDto>.CreateSuccessResponseDto(dashboard);
        }

        private async Task CheckIndexerStatus(string[] fileNames, Guid anonymizationId, int referralId)
        {
            Console.WriteLine("Waiting for indexing to complete...");
            IndexerExecutionStatus requestStatus = IndexerExecutionStatus.InProgress;
            try
            {
                await _searchClient.Indexers.GetAsync(_configuration["IndexerName"]);
                while (requestStatus.Equals(IndexerExecutionStatus.InProgress))
                {
                    Thread.Sleep(3000);
                    IndexerExecutionInfo info = await _searchClient.Indexers.GetStatusAsync(_configuration["IndexerName"]);
                    requestStatus = info.LastResult.Status;
                    Console.WriteLine("Current indexer status: {0}", requestStatus.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving indexer status: {0}", ex.Message);
            }

            ISearchIndexClient indexClient = _searchClient.Indexes.GetClient(_configuration["IndexName"]);
            foreach (var f in fileNames)
            {
                var searchResult = await indexClient.Documents.SearchAsync("*", new SearchParameters { Filter = $"(search.in(fileName,'{f}', '|'))" });
                foreach (var result in searchResult.Results)
                {
                    if (result.Document["fileName"].ToString() == f)
                    {
                        var uploadBatch = new List<IndexSchema>();
                        var indexDoc = new IndexSchema
                        {
                            id = result.Document["id"].ToString(),
                            anonymizationId = anonymizationId.ToString(),
                            referralId = referralId.ToString()
                        };
                        uploadBatch.Add(indexDoc);
                        var batch = IndexBatch.MergeOrUpload(uploadBatch);
                        await indexClient.Documents.IndexAsync(batch);
                        uploadBatch.Clear();
                    }
                }
            }
        }

        private async Task CopyBlobItems(string nihii, Guid anonymizationId, string[] fileNames)
        {
            var sourceContainerName = "portal-upload";
            var sourceContainer = await GetImageContainer(sourceContainerName);
            foreach (var fileName in fileNames)
            {
                var file = sourceContainer.GetBlockBlobReference($"{nihii}/{fileName}");
                // Construct the URI to the source file, including the SAS token.
                var destinationContainerName = "medical-images";
                var destinationContainer = await GetImageContainer(destinationContainerName);
                var destBlob = destinationContainer.GetBlockBlobReference($"{anonymizationId}-{fileName}");
                // Copy the file to the blob.
                await destBlob.StartCopyAsync(file);
            }
        }
        
        private async Task<CloudBlobContainer> GetImageContainer(string containerName)
        {
            var storageCredentials = new StorageCredentials(_configuration["StorageName"], _configuration["StorageKey"]);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        private async Task<ResponseDto> AddMessageToQueueAsync(string message)
        {
            try
            {
                // Retrieve storage account from connection string.
                var storageCredentials = new StorageCredentials(_configuration["StorageName"], _configuration["StorageKey"]);
                var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);

                // Create the queue client.
                var queueClient = cloudStorageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue.
                var queue = queueClient.GetQueueReference("referral-queue");

                // Create the queue if it doesn't already exist.
                await queue.CreateIfNotExistsAsync();

                // Create a message and add it to the queue.
                var queuemessage = new CloudQueueMessage(message);
                await queue.AddMessageAsync(queuemessage);
                return ResponseDto.CreateSuccessResponseDto();
            }
            catch (Exception ex)
            {
                return ResponseDto.CreateErrorResponseDto(ex.Message);
            }
        }

        public async Task<ResponseDto<List<PatientInfoDto>>> GetPatients()
        {
            var patients = await UnitOfWork.PatientRepository.Find("",0, 20);
            var patientDtos = new List<PatientInfoDto>();
            foreach (var p in patients)
            {
                patientDtos.Add(new PatientInfoDto { Name = p.Name, Value = p.AnonymizationId.ToString() });
            }
            return ResponseDto<List<PatientInfoDto>>.CreateSuccessResponseDto(patientDtos);

        }

        public partial class IndexSchema
        {
            [IsSearchable, IsFilterable]
            [System.ComponentModel.DataAnnotations.Key]
            public string id { get; set; }

            [IsSearchable]
            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            public string content { get; set; }

            public string metadata_storage_content_type { get; set; }

            [IsFilterable, IsFacetable]
            public Int64 metadata_storage_size { get; set; }

            [IsFilterable, IsFacetable]
            public DateTimeOffset? metadata_storage_last_modified { get; set; }

            public string metadata_storage_content_md5 { get; set; }

            [IsFilterable]
            public string metadata_storage_name { get; set; }


            [IsFilterable]
            public string metadata_storage_path { get; set; }

            public string metadata_content_type { get; set; }
            public string metadata_language { get; set; }
            public string metadata_title { get; set; }

            [IsSearchable, IsFilterable, IsFacetable]
            public string[] people { get; set; }

            [IsSearchable, IsFilterable, IsFacetable]
            public string[] organizations { get; set; }

            [IsSearchable, IsFilterable, IsFacetable]
            public string[] locations { get; set; }

            [IsSearchable, IsFilterable, IsFacetable]
            public string[] keyphrases { get; set; }

            [IsSearchable, IsFilterable, IsFacetable]
            public string language { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] medical_mentions { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] medical_mention_concepts { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] disease_disorders { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] disease_disorder_concepts { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] sign_symptoms { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] sign_symptom_concepts { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] anatomical_sites { get; set; }

            [Analyzer(AnalyzerName.AsString.StandardLucene)]
            [IsSearchable, IsFilterable, IsFacetable]
            public string[] anatomical_site_concepts { get; set; }
            [IsFilterable]
            public string nihii { get; set; }
            [IsFilterable]
            public string anonymizationId { get; set; }
            [IsFilterable]
            public string referralId { get; set; }
        }
    }
 }
