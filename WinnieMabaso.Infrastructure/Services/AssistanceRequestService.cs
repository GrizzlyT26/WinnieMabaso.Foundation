using WinnieMabaso.Application.DTOs.AssistanceRequests;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Infrastructure.Services;

public class AssistanceRequestService
    : IAssistanceRequestService
{
    private readonly IAssistanceRequestRepository _repository;

    public AssistanceRequestService(
        IAssistanceRequestRepository repository)
    {
        _repository = repository;
    }


    // ==================================================
    // CREATE ASSISTANCE REQUEST
    // ==================================================

    public async Task<AssistanceRequestResponse>
        CreateAsync(
            string beneficiaryUserId,
            CreateAssistanceRequest request)
    {
        if (string.IsNullOrWhiteSpace(
            beneficiaryUserId))
        {
            throw new InvalidOperationException(
                "Beneficiary ID is required.");
        }

        if (request == null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(
            request.Category))
        {
            throw new InvalidOperationException(
                "Category is required.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Description))
        {
            throw new InvalidOperationException(
                "Description is required.");
        }

        if (request.HouseholdSize < 1 ||
            request.HouseholdSize > 50)
        {
            throw new InvalidOperationException(
                "Household size must be between 1 and 50.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Urgency))
        {
            throw new InvalidOperationException(
                "Urgency is required.");
        }


        DateTime currentUtcTime =
            DateTime.UtcNow;


        string reference =
            await GenerateReferenceAsync();


        AssistanceRequest assistanceRequest =
            new AssistanceRequest
            {
                Id =
                    Guid.NewGuid().ToString(),

                Reference =
                    reference,

                BeneficiaryUserId =
                    beneficiaryUserId,

                Category =
                    request.Category.Trim(),

                Description =
                    request.Description.Trim(),

                HouseholdSize =
                    request.HouseholdSize,

                Urgency =
                    request.Urgency.Trim(),

                ProgrammeId =
                    string.IsNullOrWhiteSpace(
                        request.ProgrammeId)
                        ? null
                        : request.ProgrammeId.Trim(),

                Status =
                    AssistanceRequestStatus.Pending,

                DecisionReason =
                    null,

                CapturedByUserId =
                    beneficiaryUserId,

                CreatedAt =
                    currentUtcTime,

                UpdatedAt =
                    currentUtcTime
            };


        await _repository.CreateAsync(
            assistanceRequest);


        return MapToResponse(
            assistanceRequest);
    }


    // ==================================================
    // GET MY REQUESTS
    // ==================================================

    public async Task<List<AssistanceRequestResponse>>
        GetMyRequestsAsync(
            string beneficiaryUserId)
    {
        if (string.IsNullOrWhiteSpace(
            beneficiaryUserId))
        {
            return new List<AssistanceRequestResponse>();
        }


        List<AssistanceRequest> requests =
            await _repository
                .GetByBeneficiaryAsync(
                    beneficiaryUserId);


        return requests
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET REQUEST BY ID
    // ==================================================

    public async Task<AssistanceRequestResponse?>
        GetByIdAsync(
            string requestId,
            string beneficiaryUserId)
    {
        if (string.IsNullOrWhiteSpace(
            requestId))
        {
            return null;
        }


        if (string.IsNullOrWhiteSpace(
            beneficiaryUserId))
        {
            throw new InvalidOperationException(
                "Beneficiary ID is required.");
        }


        AssistanceRequest? request =
            await _repository.GetByIdAsync(
                requestId);


        if (request == null)
        {
            return null;
        }


        if (request.BeneficiaryUserId !=
            beneficiaryUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to access this assistance request.");
        }


        return MapToResponse(
            request);
    }


    // ==================================================
    // GET ALL REQUESTS
    // ==================================================

    public async Task<List<AssistanceRequestResponse>>
        GetAllAsync()
    {
        List<AssistanceRequest> requests =
            await _repository.GetAllAsync();


        return requests
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // UPDATE REQUEST STATUS
    // ==================================================

    public async Task UpdateStatusAsync(
        string requestId,
        string status,
        string decisionReason,
        string processedByUserId)
    {
        if (string.IsNullOrWhiteSpace(
            requestId))
        {
            throw new InvalidOperationException(
                "Assistance request ID is required.");
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
                out AssistanceRequestStatus
                    parsedStatus))
        {
            throw new InvalidOperationException(
                "Invalid assistance request status.");
        }


        AssistanceRequest? request =
            await _repository.GetByIdAsync(
                requestId);


        if (request == null)
        {
            throw new InvalidOperationException(
                "Assistance request could not be found.");
        }


        request.Status =
            parsedStatus;


        request.DecisionReason =
            string.IsNullOrWhiteSpace(
                decisionReason)
                ? null
                : decisionReason.Trim();


        request.CapturedByUserId =
            processedByUserId;


        request.UpdatedAt =
            DateTime.UtcNow;


        await _repository.UpdateAsync(
            request);
    }


    // ==================================================
    // GENERATE UNIQUE REFERENCE
    // ==================================================

    private async Task<string>
        GenerateReferenceAsync()
    {
        string reference;

        do
        {
            reference =
                $"AMF-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
                $"{Random.Shared.Next(1000, 9999)}";

        }
        while (
            await _repository
                .GetByReferenceAsync(
                    reference) != null);


        return reference;
    }


    // ==================================================
    // MAP ENTITY TO RESPONSE
    // ==================================================

    private static AssistanceRequestResponse
        MapToResponse(
            AssistanceRequest request)
    {
        return new AssistanceRequestResponse
        {
            Id =
                request.Id,

            Reference =
                request.Reference,

            BeneficiaryUserId =
                request.BeneficiaryUserId,

            Category =
                request.Category,

            Description =
                request.Description,

            HouseholdSize =
                request.HouseholdSize,

            Urgency =
                request.Urgency,

            ProgrammeId =
                request.ProgrammeId,

            Status =
                request.Status,

            DecisionReason =
                request.DecisionReason,

            CapturedByUserId =
                request.CapturedByUserId,

            CreatedAt =
                request.CreatedAt,

            UpdatedAt =
                request.UpdatedAt
        };
    }
}