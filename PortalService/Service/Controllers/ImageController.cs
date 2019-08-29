using System.Threading.Tasks;
using Beloning.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Service.Infrastructure;
using System;
using System.IO;
using System.Text;

namespace Service.Controllers
{
    [Produces("image/png")]
    [Route("api/Image")]
    public class ImageController : BaseController
    {
        private string[] imageExtensions = { "jpg", "jpeg", "png", "tiff", "gif", "bmp", "svg" };
        private readonly IStorageService _storageService;
        public ImageController(IStorageService storageService)
        {
            _storageService = storageService;
        }
        [HttpGet]
        public async Task<IActionResult> GetImage(string uri)
        {
            var image = await _storageService.GetImage(uri);
            return File(image.Result, "image/*");
        }

        [HttpPost("{nihii}")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> PostImage(string nihii)
        {
            try
            {
                if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                    return BadRequest($"Expected a multipart request, but got {Request.ContentType}");

                // Used to accumulate all the form url encoded key value pairs in the 
                // request.
                var formAccumulator = new KeyValueAccumulator();
                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), 52000);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    ContentDispositionHeaderValue contentDisposition;
                    var hasContentDispositionHeader =
                        ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                    if (hasContentDispositionHeader)
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            
                            using (var targetStream = new MemoryStream())
                            {
                                await section.Body.CopyToAsync(targetStream);

                                var imageName = contentDisposition.FileName.ToString();
                                bool isImage = false;
                                foreach (var ext in imageExtensions)
                                {
                                    if (imageName.ToLower().EndsWith(ext))
                                    {
                                        isImage = true;
                                    }
                                }

                                imageName = $"{nihii}/" + imageName;
                                //imageName = isImage ? $"{nihii}/images/" + imageName : $"{nihii}/documents/" + imageName;
                               
                                var photoUriResponse = await _storageService.Post(imageName, targetStream.ToArray());
                                if (!photoUriResponse.Success)
                                {
                                    return BadRequest(photoUriResponse.Error);
                                }
                                return Json($"{imageName}.jpg");
                            }
                        }
                        else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                        {
                            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                            using (var streamReader = new StreamReader(section.Body, Encoding.UTF8, true, 1024, true))
                            {
                                var value = await streamReader.ReadToEndAsync();
                                if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                                    value = string.Empty;
                                formAccumulator.Append(key.Value, value);
                            }
                        }
                    section = await reader.ReadNextSectionAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + " ---" + ex.StackTrace);
            }
           
        }
    }
}
