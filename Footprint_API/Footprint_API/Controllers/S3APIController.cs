using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Footprint_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using Footprint_API.Models;
using System.Net;

namespace Footprint_API.Controllers
{

    [Route("api/s3api")]
    [ApiController]
    public class S3APIController : ControllerBase
    {
        private readonly IS3Service _service;
        private string _tempDir = "C:\\Users\\Kevin Xu\\apiTemp";
        private string _iniFile = "C:\\Users\\Kevin Xu\\file.ini";

        public S3APIController(IS3Service service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("CreateBucket/{bucketName}")]
        public async Task<IActionResult> CreateBucket([FromRoute] string bucketName)
        {
            var response = await _service.CreateBucketAysnc(bucketName);

            return new JsonResult(response);
        }

        [HttpGet]
        [Route("GetBuckets")]
        public async Task<IActionResult> GetBuckets()
        {
            var response = await _service.GetBucketsAysnc();

            return new JsonResult(response);
        }

        [HttpGet]
        [Route("GetFiles/{bucketName}")]
        public async Task<IActionResult> GetFiles([FromRoute] string bucketName)
        {
            var response = await _service.GetFilesAysnc(bucketName);

            return new JsonResult(response);
        }

        [HttpPost]
        [Route("AddUser/{bucketName}/{userId}")]
        public async Task<IActionResult> AddUser([FromRoute] string bucketName, [FromRoute] string userId)
        {
            var response1 = await _service.UploadFileAsync(bucketName, _iniFile, userId + "/files/file.ini");
            var response2 = await _service.UploadFileAsync(bucketName, _iniFile, userId + "/wills/file.ini");
            if (response1.status == HttpStatusCode.OK &&
               response2.status == HttpStatusCode.OK)
            {
                return new JsonResult(new S3Response
                {
                    status = HttpStatusCode.OK,
                    Message = "File uploaded successfully."
                });
            }
            else
            {
                return new JsonResult(new S3Response
                {
                    status = HttpStatusCode.InternalServerError,
                    Message = "Create user folder failed."
                });
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddFile/{bucketName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName)
        {
            var file = Request.Form.Files[0];

            //Check temp directory exists
            emptyTempDirectory(_tempDir);
            if (file.Length > 0)//Check file is not empty
            {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string fullPath = Path.Combine(_tempDir, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var response = await _service.UploadFileAsync(bucketName, fullPath);

                return new JsonResult(response);
            }

            return new JsonResult(new S3Response
            {
                status = HttpStatusCode.BadRequest,
                Message = "Upload file is empty."
            });
           
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddFile/{bucketName}/{userId}/{folderName}/{fileName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName, [FromRoute] string userId, [FromRoute] string folderName, [FromRoute] string fileName)
        {
            var key = userId + "/" + folderName + "/" + fileName;
            return await InsertFiles(bucketName, key);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddFile/{bucketName}/{userId}/{fileName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName, [FromRoute] string userId, [FromRoute] string fileName)
        {
            var key = userId + "/" + fileName;
            return await InsertFiles(bucketName, key);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddFile/{bucketName}/{key}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName, [FromRoute] string key)
        {
            return await InsertFiles(bucketName, key);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("AddFiles/{bucketName}/{key}")]
        public async Task<IActionResult> AddFiles([FromRoute] string bucketName, [FromRoute] string key)
        {
            emptyTempDirectory(_tempDir);
            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        string fullPath = Path.Combine(_tempDir, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        await _service.UploadFileAsync(bucketName, fullPath, key);
                    }
                }
                return new JsonResult(new S3Response
                {
                    status = HttpStatusCode.OK,
                    Message = "All files uploaded successfully."
                });
            }
            else
            {
                return new JsonResult(new S3Response
                {
                    status = HttpStatusCode.BadRequest,
                    Message = "Upload file is empty."
                });
            }
        }

        private async Task<IActionResult> InsertFiles(string bucketName, string key)
        {
            var file = Request.Form.Files[0];

            //Check temp directory exists
            emptyTempDirectory(_tempDir);
            if (file.Length > 0)//Check file is not empty
            {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string fullPath = Path.Combine(_tempDir, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var response = await _service.UploadFileAsync(bucketName, fullPath, key);

                return new JsonResult(response);
            }

            return new JsonResult(new S3Response
            {
                status = HttpStatusCode.BadRequest,
                Message = "Upload file is empty."
            });
        }

        [HttpGet]
        [Route("GetFile/{bucketName}/{userId}/{folderName}/{fileName}")]
        public async Task<IActionResult> GetFile([FromRoute] string bucketName, [FromRoute] string userId, [FromRoute] string folderName, [FromRoute] string fileName)
        {
            var key = userId +"/" + folderName + "/" + fileName;
            return await RetrieveFile(bucketName, key);
        }

        [HttpGet]
        [Route("GetFile/{bucketName}/{userId}/{fileName}")]
        public async Task<IActionResult> GetFile([FromRoute] string bucketName, [FromRoute] string userId, [FromRoute] string fileName)
        {
            var key = userId + "/" + fileName;
            return await RetrieveFile(bucketName, key);
        }

        [HttpGet]
        [Route("GetFile/{bucketName}/{key}")]
        public async Task<IActionResult> GetFile([FromRoute] string bucketName, [FromRoute] string key)
        {
            return await RetrieveFile(bucketName, key);
        }

        private async Task<IActionResult> RetrieveFile(string bucketName, string key)
        {
            var response = await _service.DownloadFileAsync(bucketName, _tempDir, key);
            if (response.status == HttpStatusCode.OK)
            {
                var path = Path.Combine(_tempDir, key);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            else
            {
                return new JsonResult(response);
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformat-sofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        private void emptyTempDirectory(string tempDir)
        {
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            else
            {
                string[] files = Directory.GetFiles(tempDir);
                foreach (string filename in files)
                {
                    Directory.Delete(filename);
                }
            }
        }
    }
}