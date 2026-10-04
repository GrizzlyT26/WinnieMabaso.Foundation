using WinnieMabaso.Application.DTOs.VolunteerDocuments;

namespace WinnieMabaso.Application.Interfaces;

public interface IVolunteerDocumentService
{
    Task<VolunteerDocumentResponse> UploadAsync(
        string volunteerUserId,
        string volunteerApplicationId,
        string documentType,
        Stream fileStream,
        string fileName,
        string contentType,
        long fileSize);

    Task<List<VolunteerDocumentResponse>> GetMyDocumentsAsync(
        string volunteerUserId);

    Task<List<VolunteerDocumentResponse>> GetByApplicationAsync(
        string volunteerApplicationId,
        string volunteerUserId);

    Task<VolunteerDocumentResponse?> GetByIdAsync(
        string documentId,
        string volunteerUserId);

    Task DeleteAsync(
        string documentId,
        string volunteerUserId);
}