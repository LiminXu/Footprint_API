using Footprint_API.Models;
using System.IO;
using System.Threading.Tasks;

namespace Footprint_API.Services
{
    public interface IS3Service
    {
        /*S3 Bucket API*/
        Task<S3Response> CreateBucketAysnc(string bucketName);
        Task<S3Response> HasBucketAysnc(string bucketName);
        Task<S3Response> GetBucketsAysnc();
        Task<S3Response> DeleteBucketsAysnc(string bucketName);

        /*S3 Object API*/
        Task<S3Response> GetFilesAysnc(string bucketName);
        Task<S3Response> UploadFileAsync(string bucketName, string filePath);
        Task<S3Response> UploadFileAsync(string bucketName, string filePath, string key);
        Task<S3Response> DownloadFileAsync(string bucketName, string filePath,string key);
        Task<S3Response> DeleteFileAsync(string bucketName, string filePath);
    }
}