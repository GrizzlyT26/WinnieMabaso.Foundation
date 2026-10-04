using WinnieMabaso.Application.DTOs.VolunteerDocuments;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Services;

public class VolunteerDocumentService
    : IVolunteerDocumentService
{
    private readonly IVolunteerDocumentRepository _documentRepository;
    private readonly IVolunteerApplicationRepository _applicationRepository;
    private readonly IAzureBlobStorageService _storageService;

    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> AllowedFileTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {
                ".pdf",
                "application/pdf"
            },
            {
                ".doc",
                "application/msword"
            },
            {
                ".docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            },
            {
                ".jpg",
                "image/jpeg"
            },
            {
                ".jpeg",
                "image/jpeg"
            },
            {
                ".png",
                "image/png"
            }
        };


    public VolunteerDocumentService(
        IVolunteerDocumentRepository documentRepository,
        IVolunteerApplicationRepository applicationRepository,
        IAzureBlobStorageService storageService)
    {
        _documentRepository =
            documentRepository;

        _applicationRepository =
            applicationRepository;

        _storageService =
            storageService;
    }


    // ==================================================
    // UPLOAD DOCUMENT
    // ==================================================

    public async Task<VolunteerDocumentResponse>
        UploadAsync(
            string volunteerUserId,
            string volunteerApplicationId,
            string documentType,
            Stream fileStream,
            string fileName,
            string contentType,
            long fileSize)
    {
        // ------------------------------------------
        // Validate volunteer user
        // ------------------------------------------

        if (string.IsNullOrWhiteSpace(
            volunteerUserId))
        {
            throw new ArgumentException(
                "Volunteer user ID is required.");
        }


        // ------------------------------------------
        // Validate application
        // ------------------------------------------

        if (string.IsNullOrWhiteSpace(
            volunteerApplicationId))
        {
            throw new ArgumentException(
                "Volunteer application ID is required.");
        }


        // ------------------------------------------
        // Validate document type
        // ------------------------------------------

        if (string.IsNullOrWhiteSpace(
            documentType))
        {
            throw new ArgumentException(
                "Document type is required.");
        }


        // ------------------------------------------
        // Validate stream
        // ------------------------------------------

        if (fileStream == null)
        {
            throw new ArgumentNullException(
                nameof(fileStream));
        }


        // ------------------------------------------
        // Validate file size
        // ------------------------------------------

        if (fileSize <= 0)
        {
            throw new ArgumentException(
                "The uploaded file cannot be empty.");
        }


        if (fileSize > MaxFileSize)
        {
            throw new ArgumentException(
                "File size cannot exceed 5 MB.");
        }


        // ------------------------------------------
        // Validate file name
        // ------------------------------------------

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.");
        }


        // ------------------------------------------
        // Validate content type
        // ------------------------------------------

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "File content type is required.");
        }


        // ==================================================
        // VERIFY VOLUNTEER APPLICATION
        // ==================================================

        VolunteerApplication? application =
            await _applicationRepository
                .GetByIdAsync(
                    volunteerApplicationId);

        if (application == null)
        {
            throw new KeyNotFoundException(
                "Volunteer application could not be found.");
        }


        // ==================================================
        // OWNERSHIP CHECK
        // ==================================================

        if (application.VolunteerUserId !=
            volunteerUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to upload documents for this application.");
        }


        // ==================================================
        // VALIDATE FILE EXTENSION
        // ==================================================

        string extension =
            Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(
            extension))
        {
            throw new ArgumentException(
                "The uploaded file must have a valid file extension.");
        }


        if (!AllowedFileTypes.TryGetValue(
                extension,
                out string? expectedContentType))
        {
            throw new ArgumentException(
                "Unsupported file type. " +
                "Allowed types are PDF, DOC, DOCX, JPG, JPEG and PNG.");
        }


        // ==================================================
        // VALIDATE CONTENT TYPE
        // ==================================================

        if (!string.Equals(
                contentType,
                expectedContentType,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "The uploaded file content type does not match its extension.");
        }


        // ==================================================
        // GENERATE SAFE FILE NAMES
        // ==================================================

        string documentId =
            Guid.NewGuid().ToString();

        string originalFileName =
            Path.GetFileName(fileName);

        string storedFileName =
            $"{documentId}{extension.ToLowerInvariant()}";


        // ==================================================
        // AZURE STORAGE PATH
        // ==================================================

        string storagePath =
            $"volunteers/" +
            $"{volunteerUserId}/" +
            $"applications/" +
            $"{volunteerApplicationId}/" +
            $"{storedFileName}";


        // ==================================================
        // UPLOAD TO AZURE BLOB STORAGE
        // ==================================================

        string blobUrl;

        blobUrl =
            await _storageService
                .UploadFileAsync(
                    fileStream,
                    storedFileName,
                    expectedContentType,
                    storagePath);


        // ==================================================
        // CREATE FIRESTORE METADATA
        // ==================================================

        VolunteerDocument document =
            new VolunteerDocument
            {
                Id =
                    documentId,

                VolunteerUserId =
                    volunteerUserId,

                VolunteerApplicationId =
                    volunteerApplicationId,

                OriginalFileName =
                    originalFileName,

                StoredFileName =
                    storedFileName,

                ContentType =
                    expectedContentType,

                FileSize =
                    fileSize,

                StoragePath =
                    storagePath,

                DownloadUrl =
                    blobUrl,

                DocumentType =
                    documentType.Trim(),

                UploadedAt =
                    DateTime.UtcNow,

                UploadedByUserId =
                    volunteerUserId
            };


        // ==================================================
        // SAVE FIRESTORE METADATA
        // ==================================================

        try
        {
            await _documentRepository
                .CreateAsync(
                    document);

            return MapToResponse(
                document);
        }
        catch
        {
            // ------------------------------------------
            // Roll back Azure file if Firestore fails
            // ------------------------------------------

            try
            {
                await _storageService
                    .DeleteFileAsync(
                        storagePath);
            }
            catch
            {
                // Preserve original exception.
            }

            throw;
        }
    }


    // ==================================================
    // GET MY DOCUMENTS
    // ==================================================

    public async Task<List<VolunteerDocumentResponse>>
        GetMyDocumentsAsync(
            string volunteerUserId)
    {
        List<VolunteerDocument> documents =
            await _documentRepository
                .GetByVolunteerUserIdAsync(
                    volunteerUserId);

        return documents
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET DOCUMENTS BY APPLICATION
    // ==================================================

    public async Task<List<VolunteerDocumentResponse>>
        GetByApplicationAsync(
            string volunteerApplicationId,
            string volunteerUserId)
    {
        VolunteerApplication? application =
            await _applicationRepository
                .GetByIdAsync(
                    volunteerApplicationId);

        if (application == null)
        {
            throw new KeyNotFoundException(
                "Volunteer application could not be found.");
        }


        if (application.VolunteerUserId !=
            volunteerUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view documents for this application.");
        }


        List<VolunteerDocument> documents =
            await _documentRepository
                .GetByApplicationIdAsync(
                    volunteerApplicationId);

        return documents
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET DOCUMENT BY ID
    // ==================================================

    public async Task<VolunteerDocumentResponse?>
        GetByIdAsync(
            string documentId,
            string volunteerUserId)
    {
        VolunteerDocument? document =
            await _documentRepository
                .GetByIdAsync(
                    documentId);

        if (document == null)
        {
            return null;
        }


        if (document.VolunteerUserId !=
            volunteerUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view this document.");
        }


        return MapToResponse(
            document);
    }


    // ==================================================
    // DELETE DOCUMENT
    // ==================================================

    public async Task DeleteAsync(
        string documentId,
        string volunteerUserId)
    {
        VolunteerDocument? document =
            await _documentRepository
                .GetByIdAsync(
                    documentId);

        if (document == null)
        {
            throw new KeyNotFoundException(
                "Volunteer document could not be found.");
        }


        if (document.VolunteerUserId !=
            volunteerUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to delete this document.");
        }


        // ------------------------------------------
        // Delete Azure Blob
        // ------------------------------------------

        await _storageService
            .DeleteFileAsync(
                document.StoragePath);


        // ------------------------------------------
        // Delete Firestore metadata
        // ------------------------------------------

        await _documentRepository
            .DeleteAsync(
                documentId);
    }


    // ==================================================
    // MAP ENTITY TO RESPONSE
    // ==================================================

    private static VolunteerDocumentResponse
        MapToResponse(
            VolunteerDocument document)
    {
        return new VolunteerDocumentResponse
        {
            Id =
                document.Id,

            VolunteerUserId =
                document.VolunteerUserId,

            VolunteerApplicationId =
                document.VolunteerApplicationId,

            OriginalFileName =
                document.OriginalFileName,

            StoredFileName =
                document.StoredFileName,

            ContentType =
                document.ContentType,

            FileSize =
                document.FileSize,

            StoragePath =
                document.StoragePath,

            DownloadUrl =
                document.DownloadUrl,

            DocumentType =
                document.DocumentType,

            UploadedAt =
                document.UploadedAt,

            UploadedByUserId =
                document.UploadedByUserId
        };
    }
}