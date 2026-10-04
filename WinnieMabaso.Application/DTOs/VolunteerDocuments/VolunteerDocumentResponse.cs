using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.DTOs.VolunteerDocuments;

public class VolunteerDocumentResponse
{
    public string Id { get; set; } = string.Empty;

    public string VolunteerUserId { get; set; } = string.Empty;

    public string VolunteerApplicationId { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string StoragePath { get; set; } = string.Empty;

    public string? DownloadUrl { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public string? UploadedByUserId { get; set; }
}