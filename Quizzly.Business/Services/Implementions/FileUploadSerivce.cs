using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Quizzly.Business.Services.Interfaces;


namespace Quizzly.Business.Services.Implementions
{
    public class FileUploadService : IFileUploadService
    {
        //private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Cloudinary _cloudinary;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IConfiguration config)
        {
            var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file size
            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException("File size cannot exceed 5MB.");
            }

            // Validate file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Only image files (jpg, jpeg, png, gif, webp) are allowed.");
            }

            // Create unique filename
            var fileName = Guid.NewGuid().ToString() + fileExtension;




            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result.SecureUrl.ToString();
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                var publicId = ExtractPublicIdFromUrl(filePath);
                if (string.IsNullOrEmpty(publicId))
                    return false;

                var deleteParams = new DeletionParams(publicId);
                var result = _cloudinary.Destroy(deleteParams);

                return result.Result == "ok";
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            if (string.IsNullOrEmpty(cloudinaryUrl))
                return null;

            try
            {
                // Cloudinary URL format: https://res.cloudinary.com/{cloud_name}/{resource_type}/{type}/{version}/{public_id}.{format}
                // We need to extract the public_id part

                var uri = new Uri(cloudinaryUrl);
                var pathSegments = uri.AbsolutePath.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                // Skip the first segments (cloud_name, resource_type, type, version) and get the public_id
                if (pathSegments.Length >= 4)
                {
                    // Join all segments after version to handle folder structures
                    var publicIdWithExtension = string.Join("/", pathSegments.Skip(4));

                    // Remove file extension
                    var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
                    if (lastDotIndex > 0)
                    {
                        return publicIdWithExtension.Substring(0, lastDotIndex);
                    }

                    return publicIdWithExtension;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
