
using Microsoft.AspNetCore.Http;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadAsync(IFormFile file, string folderName);
        bool DeleteFile(string filePath);

    }
}
