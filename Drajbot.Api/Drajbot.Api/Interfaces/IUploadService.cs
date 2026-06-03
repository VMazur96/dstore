using Microsoft.AspNetCore.Http;

namespace Drajbot.Api.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
    }
}