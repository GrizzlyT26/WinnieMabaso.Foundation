using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Infrastructure.Azure;

public class AzureBlobStorageService
    : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;

        _containerName =
            configuration["AzureStorage:ContainerName"]
            ?? throw new InvalidOperationException(
                "Azure Storage container name is not configured.");
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string storagePath)
    {
        if (fileStream == null)
        {
            throw new ArgumentNullException(
                nameof(fileStream));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));
        }

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException(
                "Storage path is required.",
                nameof(storagePath));
        }

        BlobContainerClient containerClient =
            _blobServiceClient.GetBlobContainerClient(
                _containerName);

        BlobClient blobClient =
            containerClient.GetBlobClient(
                storagePath);

        BlobUploadOptions uploadOptions =
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

        await blobClient.UploadAsync(
            fileStream,
            uploadOptions);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(
        string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return;
        }

        BlobContainerClient containerClient =
            _blobServiceClient.GetBlobContainerClient(
                _containerName);

        BlobClient blobClient =
            containerClient.GetBlobClient(
                storagePath);

        await blobClient.DeleteIfExistsAsync();
    }
}