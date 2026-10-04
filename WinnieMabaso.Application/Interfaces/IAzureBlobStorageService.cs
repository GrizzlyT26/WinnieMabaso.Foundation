namespace WinnieMabaso.Application.Interfaces;

public interface IAzureBlobStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string storagePath);

    Task DeleteFileAsync(
        string storagePath);
}