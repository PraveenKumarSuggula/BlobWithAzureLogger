using BlobDocument.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using System.ComponentModel;

namespace BlobDocument.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly Response<byte[]> _response;
        private ILogger _logger { get; }

        public DocumentsController(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger("DocumentGetAPILogger");
            _response = new Response<byte[]>();
        }

        [HttpGet]
        public async Task<Response<byte[]>> GetBlobFile()
        {
            _logger.LogInformation("DEBUG message. GET enpoint was called.");

            _response.Errors ??= new Dictionary<string, string[]>();

            string Connection = _config.GetConnectionString("ConnectionStringsForBlobStorage");

            try
            {
                string containerName = "";
                string blobId = "";

                blobId = "214" + "/" + "c6682a7e-5b6e-4e91-862e-eadbfa34e263";

                containerName = _config.GetConnectionString("ProviderContainerName");

                BlobContainerClient blobClient = new BlobContainerClient(Connection, containerName);

                BlobClient file = blobClient.GetBlobClient(blobId);

                if (await file.ExistsAsync())
                {
                    var downloadContent = await file.DownloadAsync();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        await downloadContent.Value.Content.CopyToAsync(ms);

                        _response.Result = ms.GetBuffer();

                        //_response.Result = "data:image/" + extension + ";base64," + Convert.ToBase64String(ms.GetBuffer());
                        _logger.LogInformation("Result: " + Convert.ToBase64String(ms.GetBuffer()));
                        return _response;
                    }
                }
                else
                {
                    _response.Errors.Add("BlobError", new string[] { "Document does not exists" });
                    _logger.LogError("Error: " + _response.Errors.ToString());
                }

            }
            catch (Exception ex)
            {
                _response.Errors.Add("BlobError", new string[] { ex.Message });
                _logger.LogError("Error: " + ex.Message);
            }

            _logger.LogError("Error: " + "Uncatched error");
            return _response;
        }
    }
}
