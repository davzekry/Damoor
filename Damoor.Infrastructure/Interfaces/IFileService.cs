namespace Damoor.Infrastructure.Interfaces;

public interface IFileService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType);
    Task DeleteAsync(string fileUrl);
}