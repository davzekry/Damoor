using Damoor.Infrastructure.Interfaces; // Updated namespace

namespace Damoor.Infrastructure.Services;

public class LocalFileService : IFileService
{
    private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public LocalFileService()
    {
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(_basePath, uniqueName);

        using var fs = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fs);

        return $"/uploads/{uniqueName}";
    }

    public Task DeleteAsync(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var fullPath = Path.Combine(_basePath, fileName);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}