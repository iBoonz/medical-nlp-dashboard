using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using TikaOnDotNet.TextExtraction;

namespace MedicalEntityExtraction
{
    class Program
    {
        static string url = "http://localhost:8080/DemoServlet";
        static string format = "xml";

        static string umlsuser = "";
        static string umlspw = "";
        private static string SearchServiceName = "SearchServiceName";
        private static string SearchServiceKey = " ";
        private static string SearchServiceIndexName = "SearchServiceIndexName";
        private static string AzureFunctionSiteName = "AzureFunctionSiteName";
        private static string AzureFunctionUsername = "";
        private static string AzureFunctionPassword = "";
        private static string BlobContainerNameForImageStore = "BlobContainerNameForImageStore";
        private static string BlobContainerNameForProcessedImageStore = "BlobContainerNameForProcessedImageStore";
        private static string BlobContainerConnectionString = "";
        private static string CognitiveServicesAccountKey = "";
        private static string SkillsetName = "SkillsetNamet";
        private static string DataSourceName = "DataSourceName";
        private static string IndexerName = "IndexerName";
        private static string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";
        private static string TEXT_ANALYTICS_API_ENDPOINT = "https://westeurope.api.cognitive.microsoft.com/text/analytics/v2.0/";
        private static string TEXT_TRANSLATION_API_SUBSCRIPTION_KEY = "";
        private static string TEXT_ANALYTICS_API_SUBSCRIPTION_KEY = "";

        private static string _azureFunctionHostKey;
        private static string _searchServiceEndpoint;
        private static ISearchServiceClient _searchClient;

        private static HttpClient _httpClient = new HttpClient();

        private static ISearchServiceClient SearchClient = new SearchServiceClient(SearchServiceName, new SearchCredentials(SearchServiceKey));
        private static ISearchIndexClient indexClient = SearchClient.Indexes.GetClient(SearchServiceIndexName);

        static TextExtractor textExtractor = new TextExtractor();

        static void Main(string[] args)
        {
            _searchClient = new SearchServiceClient(SearchServiceName, new SearchCredentials(SearchServiceKey));
            _httpClient.DefaultRequestHeaders.Add("api-key", SearchServiceKey);
            _searchServiceEndpoint = String.Format("https://{0}.{1}", SearchServiceName, _searchClient.SearchDnsSuffix);

            // Modify the schema of the Azure Search Index to allow new entities to be stored
            // This will include support for DiseaseDisorder, MedicationMention, SignSymptoms, 
            // AnatomicalSites broken down by terms as well as by UMLS Concepts

            //RunAsync().GetAwaiter().GetResult();
            FillMedicalEntities().GetAwaiter().GetResult();
        }
        
        public static async Task<string> GetAzureFunctionHostKey(HttpClient client)
        {
            string uri = String.Format("https://{0}.scm.azurewebsites.net/api/functions/admin/masterkey", AzureFunctionSiteName);

            byte[] credentials = Encoding.ASCII.GetBytes(String.Format("{0}:{1}", AzureFunctionUsername, AzureFunctionPassword));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

            HttpResponseMessage response = await client.GetAsync(uri);
            string responseText = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseText);
            return json.SelectToken("masterKey").ToString();
        }

        private static async Task<bool> RunAsync()
        {
            try
            {
                bool result = await DeleteIndexingResources();
                if (!result)
                    return result;
                result = await CreateDataSource();
                if (!result)
                    return result;
                result = await CreateSkillSet();
                if (!result)
                    return result;
                result = await CreateIndex();
                if (!result)
                    return result;
                result = await CreateIndexer();
                if (!result)
                    return result;

                result = await CheckIndexerStatus();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        private static async Task<bool> DeleteIndexingResources()
        {
            Console.WriteLine("Deleting Data Source, Index, Indexer and SynonymMap if they exist...");
            try
            {
                await _searchClient.DataSources.DeleteAsync(DataSourceName);
                await _searchClient.Indexes.DeleteAsync(SearchServiceIndexName);
                await _searchClient.Indexers.DeleteAsync(IndexerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting resources: {0}", ex.Message);
                return false;
            }
            return true;
        }

        private static async Task<bool> CreateDataSource()
        {
            Console.WriteLine("Creating Data Source...");
            try
            {
                DataSource dataSource = DataSource.AzureBlobStorage(
                    name: DataSourceName,
                    storageConnectionString: BlobContainerConnectionString,
                    containerName: BlobContainerNameForImageStore,
                    description: "Data source for cognitive search example"
                );
                await _searchClient.DataSources.CreateAsync(dataSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating data source: {0}", ex.Message);
                return false;
            }
            return true;
        }

        private static async Task<bool> CreateSkillSet()
        {
            Console.WriteLine("Creating Skill Set...");
            try
            {
                if (_azureFunctionHostKey == null)
                {
                    _azureFunctionHostKey = await GetAzureFunctionHostKey(_httpClient);
                }
                using (StreamReader r = new StreamReader("skillset.json"))
                {
                    string json = r.ReadToEnd();
                    json = json.Replace("[AzureFunctionEndpointUrl]", String.Format("https://{0}.azurewebsites.net", AzureFunctionSiteName));
                    json = json.Replace("[AzureFunctionDefaultHostKey]", _azureFunctionHostKey);
                    json = json.Replace("[BlobContainerName]", BlobContainerNameForProcessedImageStore);
                    json = json.Replace("[CognitiveServicesKey]", CognitiveServicesAccountKey);
                    string uri = String.Format("{0}/skillsets/{1}?api-version=2017-11-11-Preview", _searchServiceEndpoint, SkillsetName);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Create Skill Set response: \n{0}", responseText);
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating skillset: {0}", ex.Message);
                return false;
            }
            return true;
        }

        private static async Task<bool> CreateIndex()
        {
            Console.WriteLine("Creating Index...");
            try
            {
                using (StreamReader r = new StreamReader("index.json"))
                {
                    string json = r.ReadToEnd();
                    string uri = String.Format("{0}/indexes/{1}?api-version=2017-11-11-Preview", _searchServiceEndpoint, SearchServiceIndexName);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Create Index response: \n{0}", responseText);
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating index: {0}", ex.Message);
                return false;
            }
            return true;
        }

        private static async Task<bool> CreateIndexer()
        {
            Console.WriteLine("Creating Indexer...");
            try
            {
                using (StreamReader r = new StreamReader("indexer.json"))
                {
                    string json = r.ReadToEnd();
                    json = json.Replace("[IndexerName]", IndexerName);
                    json = json.Replace("[DataSourceName]", DataSourceName);
                    json = json.Replace("[IndexName]", SearchServiceIndexName);
                    json = json.Replace("[SkillSetName]", SkillsetName);
                    string uri = String.Format("{0}/indexers/{1}?api-version=2017-11-11-Preview", _searchServiceEndpoint, IndexerName);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
                    string responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Create Indexer response: \n{0}", responseText);
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating indexer: {0}", ex.Message);
                return false;
            }
            return true;
        }

        private static async Task<bool> CheckIndexerStatus()
        {
            Console.WriteLine("Waiting for indexing to complete...");
            IndexerExecutionStatus requestStatus = IndexerExecutionStatus.InProgress;
            try
            {
                await _searchClient.Indexers.GetAsync(IndexerName);
                while (requestStatus.Equals(IndexerExecutionStatus.InProgress))
                {
                    Thread.Sleep(3000);
                    IndexerExecutionInfo info = await _searchClient.Indexers.GetStatusAsync(IndexerName);
                    requestStatus = info.LastResult.Status;
                    Console.WriteLine("Current indexer status: {0}", requestStatus.ToString());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving indexer status: {0}", ex.Message);
                return false;
            }
            return requestStatus.Equals(IndexerExecutionStatus.Success);
        }

        private static async Task<bool> FillMedicalEntities()
        {
            Console.WriteLine("Querying Index...");
            try
            {
                ISearchIndexClient indexClient = _searchClient.Indexes.GetClient(SearchServiceIndexName);
                DocumentSearchResult searchResult = await indexClient.Documents.SearchAsync("*");
                Console.WriteLine("Query Results:");
                foreach (SearchResult result in searchResult.Results)
                {
                    var translated = await Translate(result.Document["text"].ToString(), Language.English);
                    var medicaEntities = ProcessDoc(translated);
                    // Upload the new entities to Azure Search
                    UploadMedicalEntities(medicaEntities, result.Document["id"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}", ex.Message);
                return false;
            }
            return true;
        }


        private static async Task<string> Translate(string textToTranslate, Language language)
        {
            // auto-detect source language if requested
            var fromLanguageCode = await DetectLanguage(textToTranslate);
            string toLanguageCode = GetLanguageCode(language);

            // handle null operations: no text or same source/target languages
            if (textToTranslate == "" || fromLanguageCode == toLanguageCode)
            {
                return textToTranslate;
            }

            // send HTTP request to perform the translation
            string endpoint = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "translate");
            string uri = string.Format(endpoint + "&from={0}&to={1}", fromLanguageCode, toLanguageCode);

            System.Object[] body = new System.Object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", TEXT_TRANSLATION_API_SUBSCRIPTION_KEY);
                request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                var translations = result[0]["translations"];
                var translation = translations[0]["text"];
                return translation;
            }
        }

        private static string GetLanguageCode(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return "en";
                case Language.Dutch:
                    return "nl";
                default:
                    return "en";
            }
        }

        private static async Task<string> DetectLanguage(string text)
        {
            try
            {
                ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
                {
                    Endpoint = "https://westeurope.api.cognitive.microsoft.com"
                }; //Replace 'westus' with the correct region for your Text Analytics subscription

                // Extracting language
                Console.WriteLine("===== LANGUAGE EXTRACTION ======");

                var langResults = await client.DetectLanguageAsync(
                    false,
                    new LanguageBatchInput(
                        new List<LanguageInput>
                            {
                          new LanguageInput(id: "1", text: text),
                            }));

                return langResults.Documents.First().DetectedLanguages[0].Iso6391Name;
            }
            catch (Exception ex)
            {
                return "en";
                throw ex;
            }
        }

        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", TEXT_ANALYTICS_API_SUBSCRIPTION_KEY);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        static MedicalEntities ProcessDoc(string  text)
        {
            var medicalEntities = new MedicalEntities();
            medicalEntities.DiseaseDisorderList = new List<Term>();
            medicalEntities.MedicationMentionList = new List<Term>();
            medicalEntities.SignSymptomMentionList = new List<Term>();
            medicalEntities.AnatomicalSiteMentionList = new List<Term>();
            medicalEntities.DiseaseDisorderConceptList = new List<OntologyConcept>();
            medicalEntities.MedicationMentionConceptList = new List<OntologyConcept>();
            medicalEntities.SignSymptomMentionConceptList = new List<OntologyConcept>();
            medicalEntities.AnatomicalSiteMentionConceptList = new List<OntologyConcept>();
            medicalEntities.ConceptNameDictionary = new Dictionary<int, string>();

            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);

                // Take a max of X KB of text
                var subText = text.Substring(0, Math.Min(20480, text.Length));
                var postData = "q=" + subText;
                postData += "&format=" + format;
                postData += "&umlsuser=" + umlsuser;
                postData += "&umlspw=" + umlspw;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Timeout = 20 * 60 * 1000;   // 20 min

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (responseString != "")
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(responseString);
                    string parsedText = "";
                    int begin, end;
                    Guid termId;

                    foreach (var node in xml.ChildNodes[1])
                    {
                        //Sofa 
                        if (((System.Xml.XmlElement)node).LocalName == "Sofa")
                        {
                            parsedText = ((XmlElement)node).GetAttribute("sofaString");
                        }
                        else if (((System.Xml.XmlElement)node).LocalName == "UmlsConcept")
                        {
                            medicalEntities.ConceptNameDictionary[Convert.ToInt32(((XmlElement)node).GetAttribute("xmi:id"))] =
                                ((XmlElement)node).GetAttribute("preferredText");
                        }
                        else if (((System.Xml.XmlElement)node).LocalName == "DiseaseDisorderMention")
                        {
                            begin = Convert.ToInt32(((XmlElement)node).GetAttribute("begin"));
                            end = Convert.ToInt32(((XmlElement)node).GetAttribute("end"));

                            if (!(medicalEntities.DiseaseDisorderList.Any(t => t.term == parsedText.Substring(begin, end - begin).ToLower())))
                            {
                                termId = Guid.NewGuid();
                                medicalEntities.DiseaseDisorderList.Add(new Term
                                {
                                    termId = termId,
                                    term = parsedText.Substring(begin, end - begin).ToLower(),
                                });

                                var ontologyConceptArray = ((XmlElement)node).GetAttribute("ontologyConceptArr").ToString();
                                if (ontologyConceptArray.Length > 0)
                                {
                                    foreach (var c in ontologyConceptArray.Split(' '))
                                    {
                                        medicalEntities.DiseaseDisorderConceptList.Add(new OntologyConcept
                                        {
                                            conceptId = Guid.NewGuid(),
                                            termId = termId,
                                            ontologyConcept = c
                                        });
                                    }
                                }
                            }

                        }
                        else if (((System.Xml.XmlElement)node).LocalName == "MedicationMention")
                        {
                            begin = Convert.ToInt32(((XmlElement)node).GetAttribute("begin"));
                            end = Convert.ToInt32(((XmlElement)node).GetAttribute("end"));
                            if (!(medicalEntities.MedicationMentionList.Any(t => t.term == parsedText.Substring(begin, end - begin).ToLower())))
                            {
                                termId = Guid.NewGuid();
                                medicalEntities.MedicationMentionList.Add(new Term
                                {
                                    termId = termId,
                                    term = parsedText.Substring(begin, end - begin).ToLower()
                                });
                                var ontologyConceptArray = ((XmlElement)node).GetAttribute("ontologyConceptArr").ToString();
                                if (ontologyConceptArray.Length > 0)
                                {
                                    foreach (var c in ontologyConceptArray.Split(' '))
                                    {
                                        medicalEntities.MedicationMentionConceptList.Add(new OntologyConcept
                                        {
                                            conceptId = Guid.NewGuid(),
                                            termId = termId,
                                            ontologyConcept = c
                                        });
                                    }
                                }
                            }

                        }
                        else if (((System.Xml.XmlElement)node).LocalName == "SignSymptomMention")
                        {
                            begin = Convert.ToInt32(((XmlElement)node).GetAttribute("begin"));
                            end = Convert.ToInt32(((XmlElement)node).GetAttribute("end"));
                            if (!(medicalEntities.SignSymptomMentionList.Any(t => t.term == parsedText.Substring(begin, end - begin).ToLower())))
                            {
                                termId = Guid.NewGuid();
                                medicalEntities.SignSymptomMentionList.Add(new Term
                                {
                                    termId = termId,
                                    term = parsedText.Substring(begin, end - begin).ToLower()
                                });
                                var ontologyConceptArray = ((XmlElement)node).GetAttribute("ontologyConceptArr").ToString();
                                if (ontologyConceptArray.Length > 0)
                                {
                                    foreach (var c in ontologyConceptArray.Split(' '))
                                    {
                                        medicalEntities.SignSymptomMentionConceptList.Add(new OntologyConcept
                                        {
                                            conceptId = Guid.NewGuid(),
                                            termId = termId,
                                            ontologyConcept = c
                                        });
                                    }
                                }
                            }
                        }
                        else if (((System.Xml.XmlElement)node).LocalName == "AnatomicalSiteMention")
                        {
                            begin = Convert.ToInt32(((XmlElement)node).GetAttribute("begin"));
                            end = Convert.ToInt32(((XmlElement)node).GetAttribute("end"));
                            if (!(medicalEntities.AnatomicalSiteMentionList.Any(t => t.term == parsedText.Substring(begin, end - begin).ToLower())))
                            {
                                termId = Guid.NewGuid();
                                medicalEntities.AnatomicalSiteMentionList.Add(new Term
                                {
                                    termId = termId,
                                    term = parsedText.Substring(begin, end - begin).ToLower()
                                });
                                var ontologyConceptArray = ((XmlElement)node).GetAttribute("ontologyConceptArr").ToString();
                                if (ontologyConceptArray.Length > 0)
                                {
                                    foreach (var c in ontologyConceptArray.Split(' '))
                                    {
                                        medicalEntities.AnatomicalSiteMentionConceptList.Add(new OntologyConcept
                                        {
                                            conceptId = Guid.NewGuid(),
                                            termId = termId,
                                            ontologyConcept = c
                                        });
                                    }
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return medicalEntities;
        }

        static void UploadMedicalEntities(MedicalEntities medicaEntities, string id)
        {
            // Upload the specified entity to Azure Search
            // Note it is much more efficient to upload content in batches

            // Convert Concept ID's to actual Concept names
            var MedicationMentionNames = new List<string>();
            var AnatomicalSiteNames = new List<string>();
            var DiseaseDisorderNames = new List<string>();
            var SignSymptomNames = new List<string>();
            foreach (var concept in medicaEntities.MedicationMentionConceptList.Select(x=>x.ontologyConcept).Distinct())
                MedicationMentionNames.Add(medicaEntities.ConceptNameDictionary.Where(x => x.Key == Convert.ToInt32(concept)).First().Value);
            foreach (var concept in medicaEntities.AnatomicalSiteMentionConceptList.Select(x => x.ontologyConcept).Distinct())
                AnatomicalSiteNames.Add(medicaEntities.ConceptNameDictionary.Where(x => x.Key == Convert.ToInt32(concept)).First().Value);
            foreach (var concept in medicaEntities.DiseaseDisorderConceptList.Select(x => x.ontologyConcept).Distinct())
                DiseaseDisorderNames.Add(medicaEntities.ConceptNameDictionary.Where(x => x.Key == Convert.ToInt32(concept)).First().Value);
            foreach (var concept in medicaEntities.SignSymptomMentionConceptList.Select(x => x.ontologyConcept).Distinct())
                SignSymptomNames.Add(medicaEntities.ConceptNameDictionary.Where(x => x.Key == Convert.ToInt32(concept)).First().Value);

            try
            {
                var uploadBatch = new List<IndexSchema>();
                var indexDoc = new IndexSchema();
                // Get the key value so that we can merge content with the existing content
                // The key is a url token encoding of the path (e.g. https://azsdemos.blob.core.windows.net/medical-tutorial/nihms637915.pdf)
                //file = MedicalStorage + file.Substring(file.LastIndexOf("\\") + 1);
                //indexDoc.metadata_storage_path = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(file));
                indexDoc.id = id;
                indexDoc.medical_mentions = medicaEntities.MedicationMentionList.Select(x => x.term).Distinct().ToArray();
                indexDoc.medical_mention_concepts = MedicationMentionNames.Distinct().ToArray();
                indexDoc.sign_symptoms = medicaEntities.SignSymptomMentionList.Select(x => x.term).Distinct().ToArray();
                indexDoc.sign_symptom_concepts = SignSymptomNames.Distinct().ToArray();
                indexDoc.anatomical_sites = medicaEntities.AnatomicalSiteMentionList.Select(x => x.term).Distinct().ToArray();
                indexDoc.anatomical_site_concepts= AnatomicalSiteNames.Distinct().ToArray();
                indexDoc.disease_disorders = medicaEntities.DiseaseDisorderList.Select(x => x.term).Distinct().ToArray();
                indexDoc.disease_disorder_concepts = DiseaseDisorderNames.Distinct().ToArray();
                indexDoc.nihii = "87051718996";
                uploadBatch.Add(indexDoc);

                var batch = IndexBatch.MergeOrUpload(uploadBatch);

                indexClient.Documents.Index(batch);
                uploadBatch.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
