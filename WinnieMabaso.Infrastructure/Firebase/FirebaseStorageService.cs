using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Infrastructure.Firebase;

public class FirebaseStorageService
    : IFirebaseStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public FirebaseStorageService(
        StorageClient storageClient,
        IConfiguration configuration)
    {
        _storageClient = storageClient;

        _bucketName =
            configuration["Firebase:StorageBucket"]
            ?? throw new InvalidOperationException(
                "Firebase Storage bucket is not configured.");
    }


    // ==================================================
    // UPLOAD FILE
    // ==================================================

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


        await _storageClient.UploadObjectAsync(
            _bucketName,
            storagePath,
            contentType,
            fileStream);

        return storagePath;
    }


    // ==================================================
    // DELETE FILE
    // ==================================================

    public async Task DeleteFileAsync(
        string storagePath)
    {
        if (string.IsNullOrWhiteSpace(
            storagePath))
        {
            return;
        }

        await _storageClient.DeleteObjectAsync(
            _bucketName,
            storagePath);
    }
}