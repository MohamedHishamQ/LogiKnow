using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LogiKnow.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LogiKnow.Infrastructure.Storage;

public class StorageService : IStorageService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<StorageService> _logger;

    public StorageService(IConfiguration configuration, IWebHostEnvironment env, ILogger<StorageService> logger)
    {
        _configuration = configuration;
        _env = env;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, string directory = "uploads", CancellationToken ct = default)
    {
        var azureConnString = _configuration["AzureStorage:ConnectionString"];
        var containerName = _configuration["AzureStorage:ContainerName"] ?? "logiknow-books";

        // Fallback to local file system if Azure is not configured
        if (string.IsNullOrEmpty(azureConnString) || azureConnString == "REPLACE")
        {
            _logger.LogInformation("Azure Storage not configured or set to REPLACE. Falling back to local file system.");
            return await UploadToLocalAsync(fileName, fileStream, directory, ct);
        }

        try
        {
            var blobServiceClient = new BlobServiceClient(azureConnString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            // Ensure container exists and is publicly readable
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

            // Construct folder path within the container
            var blobName = string.IsNullOrEmpty(directory) ? fileName : $"{directory}/{fileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };

            await blobClient.UploadAsync(fileStream, options, ct);
            
            _logger.LogInformation("File {FileName} uploaded to Azure Blob Storage successfully.", fileName);
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to Azure Storage. Falling back to local storage.");
            return await UploadToLocalAsync(fileName, fileStream, directory, ct);
        }
    }

    private async Task<string> UploadToLocalAsync(string fileName, Stream fileStream, string directory, CancellationToken ct)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), directory);
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var filePath = Path.Combine(uploadsPath, fileName);
        
        // Reset stream position if needed
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        using (var localStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(localStream, ct);
        }

        var relativePath = $"/{directory}/{fileName}".Replace("\\", "/");
        return relativePath;
    }
}
