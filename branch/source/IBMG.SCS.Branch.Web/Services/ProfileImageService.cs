using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IBMG.SCS.Branch.Web.Settings;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace IBMG.SCS.Branch.Web.Services
{
    public class ProfileImageService : IProfileImageService
    {
        private readonly ProfileImageSetting _settings;
        private readonly ILogger<ProfileImageService> _logger;

        public ProfileImageService(
            IOptions<DocumentSettings> documentSettings,
            ILogger<ProfileImageService> logger)
        {
            _settings = documentSettings.Value.ProfileImageSetting;
            _logger = logger;
        }

        public async Task<string> UploadAsync(
            byte[] imageBytes,
            string fileName,
            string? existingImageUrl,
            CancellationToken cancellationToken)
        {
            Validate(imageBytes, fileName);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = GetContentType(extension);

            var blobServiceClient =
                new BlobServiceClient(_settings.BlobConnectionString);

            var containerClient =
                blobServiceClient.GetBlobContainerClient(_settings.BlobContainerName);

            await containerClient.CreateIfNotExistsAsync(
                PublicAccessType.Blob,
                cancellationToken: cancellationToken);

            var blobName = $"{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using var ms = new MemoryStream(imageBytes);
            await blobClient.UploadAsync(
                ms,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                },
                cancellationToken);

            await this.DeleteOldImageIfExists(containerClient, existingImageUrl, cancellationToken);

            return blobClient.Uri.ToString();
        }

        private void Validate(byte[] bytes, string fileName)
        {
            if (bytes.Length > _settings.MaxFileSize)
            {
                throw new ValidationException("Image too large");
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_settings.AllowedProfileImageTypes.Contains(ext))
            {
                throw new ValidationException($"Invalid image type: {ext}");
            }
        }

        private static string GetContentType(string ext) =>
            ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".heic" => "image/heic",
                _ => "application/octet-stream",
            };

        private async Task DeleteOldImageIfExists(
            BlobContainerClient containerClient,
            string? existingImageUrl,
            CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(existingImageUrl))
                return;

            try
            {
                var blobName = Path.GetFileName(new Uri(existingImageUrl).AbsolutePath);
                await containerClient
                    .GetBlobClient(blobName)
                    .DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete old image");
            }
        }
    }
}
