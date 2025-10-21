using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using MuseumSystem.Domain.Options;

namespace MuseumSystem.Application.Services
{
    public class CloudStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public CloudStorageService(IOptions<GoogleCloudStorageOptions> options, StorageClient storageClient)
        {
            _storageClient = storageClient;
            _bucketName = options.Value.BucketName;
        }

        public async Task<string> Upload3DModelAsync(Stream fileStream, string fileName, string contentType)
        {
            var objectName = $"{Guid.NewGuid()}_{fileName}";
            await _storageClient.UploadObjectAsync(_bucketName, objectName, contentType, fileStream);

            // Public URL (nếu bucket public)
            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var prefix = $"https://storage.googleapis.com/{_bucketName}/";
            if (!filePath.StartsWith(prefix))
            {
                throw new ArgumentException("Invalid file path for deletion.");
            }
            var objectName = filePath.Substring(prefix.Length); // Lấy tên đối tượng từ URL
            await _storageClient.DeleteObjectAsync(_bucketName, objectName);

        }
    }
}
