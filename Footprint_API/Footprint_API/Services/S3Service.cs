using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.S3.Util;
using Amazon.S3.Model;
using Footprint_API.Models;
using System.Net;
using Amazon.S3.Transfer;
using System.IO;

namespace Footprint_API.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<S3Response> CreateBucketAysnc(string bucketName)
        {
            if (await AmazonS3Util.DoesS3BucketExistAsync(_client, bucketName) == false)
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                var response = await _client.PutBucketAsync(putBucketRequest);

                return new S3Response
                {
                    Message = response.ResponseMetadata.ToString(),
                    status = response.HttpStatusCode
                };
            }
            else
            {
                return new S3Response
                {
                    status = HttpStatusCode.BadRequest,
                    Message = "Bucket already exsits."
                };
            }
        }

        public async Task<S3Response> GetBucketsAysnc()
        {
            
            ListBucketsResponse response = await _client.ListBucketsAsync();
            var json = JsonConvert.SerializeObject(response.Buckets);
            return new S3Response
            {
                Message = json,
                status = response.HttpStatusCode
            };
        }

        public async Task<S3Response> HasBucketAysnc(string bucketName)
        {
            if (await AmazonS3Util.DoesS3BucketExistAsync(_client, bucketName) == true)
            {
                return new S3Response
                {
                    status = HttpStatusCode.OK,
                    Message = "Bucket already exsits."
                };
            }
            else
            {
                return new S3Response
                {
                    status = HttpStatusCode.BadRequest,
                    Message = "Bucket does not exsit."
                };
            }
            
        }

        public async Task<S3Response> DeleteBucketsAysnc(string bucketName)
        {

            if (await AmazonS3Util.DoesS3BucketExistAsync(_client, bucketName) == true)
            {
                var deleteBucketRequest = new DeleteBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                var response = await _client.DeleteBucketAsync(deleteBucketRequest);

                return new S3Response
                {
                    status = HttpStatusCode.OK,
                    Message = response.ResponseMetadata.ToString()
                };
            }
            else
            {
                return new S3Response
                {
                    status = HttpStatusCode.BadRequest,
                    Message = "Bucket does not exsit."
                };
            }
            
        }

        public async Task<S3Response> GetFilesAysnc(string bucketName)
        {
            
            //TO do
            return new S3Response
            {
                status = HttpStatusCode.OK,
                Message = ""
            };
        }

        public async Task<S3Response> UploadFileAsync(string bucketName, string filePath)
        {
           
            var fileTransferUtility = new TransferUtility(_client);

            await fileTransferUtility.UploadAsync(filePath, bucketName);

            return new S3Response
            {
                status = HttpStatusCode.OK,
                Message = "File uploaded successfully."
            };

        }

        public async Task<S3Response> UploadFileAsync(string bucketName, string filePath, string key)
        {
            var fileTransferUtility = new TransferUtility(_client);

            await fileTransferUtility.UploadAsync(filePath, bucketName, key);

            return new S3Response
            {
                status = HttpStatusCode.OK,
                Message = "File uploaded successfully."
            };
        }

        /*public Task<S3Response> UploadFileAsync(string bucketName, string filePath, string key)
        {
            var fileTransferUtility = new TransferUtility(_client);

            //Option 1
            await fileTransferUtility.UploadAsync(FilePath, bucketName);

            //Option 2
            await fileTransferUtility.UploadAsync(FilePath, bucketName, UpdateWithKeyName);

            //Option 3
            using (var fileToUpload = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                await fileTransferUtility.UploadAsync(fileToUpload, bucketName, FileStreamUpload);
            }

            //Option 4
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                FilePath = FilePath,
                StorageClass = S3StorageClass.Standard,
                PartSize = 6291456,//6Mb
                Key = AdvancedUpload,
                CannedACL = S3CannedACL.NoACL
            };

            fileTransferUtilityRequest.Metadata.Add("Param1", "Value1");
            fileTransferUtilityRequest.Metadata.Add("Param2", "Value2");

            await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

            return new S3Response
            {
                status = HttpStatusCode.OK,
                Message = "File uploaded successfully."
            };
        }*/

        public async Task<S3Response> DownloadFileAsync(string bucketName, string filePath, string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            string responseBody;
            using (var response = await _client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                responseBody = reader.ReadToEnd();
            }

            var createText = responseBody;

            File.WriteAllText(Path.Combine(filePath, key), createText);

            return new S3Response
            {
                status = HttpStatusCode.OK,
                Message = "File downloaded successfully."
            };
        }

        public async Task<S3Response> DeleteFileAsync(string bucketName, string filePath)
        {
            const string keyName = "Test.txt";
            
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            string responseBody;
            using (var response = await _client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                var title = response.Metadata["Param1"];
                var contentType = response.Headers["Content-Type"];

                Console.WriteLine($"Object meta, Title: {title}");
                Console.WriteLine($"Content type, {contentType}");

                responseBody = reader.ReadToEnd();
            }

            var pathAndFileName = $"C:\\Users\\Kevin Xu\\Documents\\{keyName}";
            var createText = responseBody;
            File.WriteAllText(pathAndFileName, createText);

            return new S3Response
            {
                status = HttpStatusCode.OK,
                Message = "File downloaded successfully."
            };
        }
    }
}
