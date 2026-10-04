using WinnieMabaso.Application.DTOs.VolunteerApplications;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Infrastructure.Services;

public class VolunteerApplicationService
    : IVolunteerApplicationService
{
    private readonly IVolunteerApplicationRepository
        _repository;

    public VolunteerApplicationService(
        IVolunteerApplicationRepository repository)
    {
        _repository = repository;
    }


    // ==================================================
    // CREATE VOLUNTEER APPLICATION
    // ==================================================

    public async Task<VolunteerApplicationResponse>
        CreateAsync(
            string volunteerUserId,
            CreateVolunteerApplication request)
    {
        if (string.IsNullOrWhiteSpace(
            volunteerUserId))
        {
            throw new InvalidOperationException(
                "Volunteer user ID is required.");
        }

        if (request == null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(
            request.FullName))
        {
            throw new InvalidOperationException(
                "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Email))
        {
            throw new InvalidOperationException(
                "Email address is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.MobileNumber))
        {
            throw new InvalidOperationException(
                "Mobile number is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Skills))
        {
            throw new InvalidOperationException(
                "Skills are required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Availability))
        {
            throw new InvalidOperationException(
                "Availability is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Motivation))
        {
            throw new InvalidOperationException(
                "Motivation is required.");
        }


        // ----------------------------------------------
        // Prevent duplicate active applications
        // ----------------------------------------------

        List<VolunteerApplication> existingApplications =
            await _repository
                .GetByVolunteerUserIdAsync(
                    volunteerUserId);

        bool hasPendingApplication =
            existingApplications.Any(
                application =>
                    application.Status ==
                        VolunteerApplicationStatus.Pending
                    ||
                    application.Status ==
                        VolunteerApplicationStatus.UnderReview);


        if (hasPendingApplication)
        {
            throw new InvalidOperationException(
                "You already have a volunteer application under review.");
        }


        DateTime currentUtcTime =
            DateTime.UtcNow;


        VolunteerApplication application =
            new VolunteerApplication
            {
                Id =
                    Guid.NewGuid().ToString(),

                VolunteerUserId =
                    volunteerUserId,

                FullName =
                    request.FullName.Trim(),

                Email =
                    request.Email.Trim(),

                MobileNumber =
                    request.MobileNumber.Trim(),

                Skills =
                    request.Skills.Trim(),

                Availability =
                    request.Availability.Trim(),

                Motivation =
                    request.Motivation.Trim(),

                Status =
                    VolunteerApplicationStatus.Pending,

                AdminNotes =
                    null,

                ProcessedByUserId =
                    null,

                ProcessedAt =
                    null,

                CreatedAt =
                    currentUtcTime,

                UpdatedAt =
                    currentUtcTime
            };


        await _repository.CreateAsync(
            application);


        return MapToResponse(
            application);
    }


    // ==================================================
    // GET MY APPLICATIONS
    // ==================================================

    public async Task<List<VolunteerApplicationResponse>>
        GetMyApplicationsAsync(
            string volunteerUserId)
    {
        if (string.IsNullOrWhiteSpace(
            volunteerUserId))
        {
            return new List<VolunteerApplicationResponse>();
        }


        List<VolunteerApplication> applications =
            await _repository
                .GetByVolunteerUserIdAsync(
                    volunteerUserId);


        return applications
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET APPLICATION BY ID
    // ==================================================

    public async Task<VolunteerApplicationResponse?>
        GetByIdAsync(
            string applicationId,
            string volunteerUserId)
    {
        if (string.IsNullOrWhiteSpace(
            applicationId))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(
            volunteerUserId))
        {
            throw new InvalidOperationException(
                "Volunteer user ID is required.");
        }


        VolunteerApplication? application =
            await _repository.GetByIdAsync(
                applicationId);


        if (application == null)
        {
            return null;
        }


        if (application.VolunteerUserId !=
            volunteerUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to access this volunteer application.");
        }


        return MapToResponse(
            application);
    }


    // ==================================================
    // GET ALL APPLICATIONS
    // ==================================================

    public async Task<List<VolunteerApplicationResponse>>
        GetAllAsync()
    {
        List<VolunteerApplication> applications =
            await _repository.GetAllAsync();


        return applications
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // UPDATE APPLICATION STATUS
    // ==================================================

    public async Task UpdateStatusAsync(
        string applicationId,
        string status,
        string adminNotes,
        string processedByUserId)
    {
        if (string.IsNullOrWhiteSpace(
            applicationId))
        {
            throw new InvalidOperationException(
                "Volunteer application ID is required.");
        }

        if (string.IsNullOrWhiteSpace(
            status))
        {
            throw new InvalidOperationException(
                "Status is required.");
        }

        if (string.IsNullOrWhiteSpace(
            processedByUserId))
        {
            throw new InvalidOperationException(
                "Processing administrator is required.");
        }


        if (!Enum.TryParse(
                status,
                true,
                out VolunteerApplicationStatus
                    parsedStatus))
        {
            throw new InvalidOperationException(
                "Invalid volunteer application status.");
        }


        VolunteerApplication? application =
            await _repository.GetByIdAsync(
                applicationId);


        if (application == null)
        {
            throw new InvalidOperationException(
                "Volunteer application could not be found.");
        }


        application.Status =
            parsedStatus;


        application.AdminNotes =
            string.IsNullOrWhiteSpace(
                adminNotes)
                ? null
                : adminNotes.Trim();


        application.ProcessedByUserId =
            processedByUserId;


        application.ProcessedAt =
            DateTime.UtcNow;


        application.UpdatedAt =
            DateTime.UtcNow;


        await _repository.UpdateAsync(
            application);
    }


    // ==================================================
    // MAP ENTITY TO RESPONSE
    // ==================================================

    private static VolunteerApplicationResponse
        MapToResponse(
            VolunteerApplication application)
    {
        return new VolunteerApplicationResponse
        {
            Id =
                application.Id,

            VolunteerUserId =
                application.VolunteerUserId,

            FullName =
                application.FullName,

            Email =
                application.Email,

            MobileNumber =
                application.MobileNumber,

            Skills =
                application.Skills,

            Availability =
                application.Availability,

            Motivation =
                application.Motivation,

            Status =
                application.Status,

            AdminNotes =
                application.AdminNotes,

            ProcessedByUserId =
                application.ProcessedByUserId,

            ProcessedAt =
                application.ProcessedAt,

            CreatedAt =
                application.CreatedAt,

            UpdatedAt =
                application.UpdatedAt
        };
    }
}