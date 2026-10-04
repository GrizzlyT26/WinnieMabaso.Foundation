using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinnieMabaso.Application.DTOs.AssistanceRequests;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssistanceRequestsController
    : ControllerBase
{
    private readonly IAssistanceRequestService
        _requestService;


    public AssistanceRequestsController(
        IAssistanceRequestService requestService)
    {
        _requestService =
            requestService;
    }


    // ==================================================
    // POST
    // api/AssistanceRequests
    // ==================================================

    [HttpPost]
    [Authorize(
        Policy = "BeneficiaryOnly")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssistanceRequest request)
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;


            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "User identity could not be determined."
                });
            }


            AssistanceRequestResponse result =
                await _requestService.CreateAsync(
                    userId,
                    request);


            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = result.Id
                },
                new
                {
                    success = true,
                    message =
                        "Assistance request submitted successfully.",
                    request = result
                });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while submitting the assistance request."
                });
        }
    }


    // ==================================================
    // GET MY REQUESTS
    // api/AssistanceRequests/my
    // ==================================================

    [HttpGet("my")]
    [Authorize(
        Policy = "BeneficiaryOnly")]
    public async Task<IActionResult>
        GetMyRequests()
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;


            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "User identity could not be determined."
                });
            }


            List<AssistanceRequestResponse>
                requests =
                    await _requestService
                        .GetMyRequestsAsync(
                            userId);


            return Ok(new
            {
                success = true,

                message =
                    "Assistance requests retrieved successfully.",

                count =
                    requests.Count,

                requests
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while retrieving assistance requests."
                });
        }
    }


    // ==================================================
    // GET BY ID
    // api/AssistanceRequests/{id}
    // ==================================================

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult>
        GetById(string id)
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;


            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "User identity could not be determined."
                });
            }


            string? role =
                User.FindFirst(
                    ClaimTypes.Role)?.Value;


            bool isAdministration =
                role ==
                    UserRole.Administrator.ToString()
                ||
                role ==
                    UserRole.SuperAdministrator.ToString();


            AssistanceRequestResponse? request;


            if (isAdministration)
            {
                request =
                    await GetRequestForAdministrator(
                        id);
            }
            else
            {
                request =
                    await _requestService.GetByIdAsync(
                        id,
                        userId);
            }


            if (request == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Assistance request was not found."
                });
            }


            return Ok(new
            {
                success = true,
                request
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while retrieving the assistance request."
                });
        }
    }


    // ==================================================
    // GET ALL
    // Administration only
    // ==================================================

    [HttpGet]
    [Authorize(
        Policy = "AdministrationOnly")]
    public async Task<IActionResult>
        GetAll()
    {
        try
        {
            List<AssistanceRequestResponse>
                requests =
                    await _requestService
                        .GetAllAsync();


            return Ok(new
            {
                success = true,

                message =
                    "Assistance requests retrieved successfully.",

                count =
                    requests.Count,

                requests
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while retrieving assistance requests."
                });
        }
    }


    // ==================================================
    // UPDATE STATUS
    // Administration only
    // ==================================================

    [HttpPut("{id}/status")]
    [Authorize(
        Policy = "AdministrationOnly")]
    public async Task<IActionResult>
        UpdateStatus(
            string id,
            [FromBody]
            UpdateAssistanceRequestStatus request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Request body is required."
                });
            }


            string? processedByUserId =
                User.FindFirst("userId")?.Value;


            if (string.IsNullOrWhiteSpace(
                processedByUserId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "Administrator identity could not be determined."
                });
            }


            await _requestService.UpdateStatusAsync(
                id,
                request.Status,
                request.DecisionReason ?? string.Empty,
                processedByUserId);


            return Ok(new
            {
                success = true,

                message =
                    "Assistance request status updated successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while updating the assistance request."
                });
        }
    }


    // ==================================================
    // ADMINISTRATOR GET BY ID
    // ==================================================

    private async Task<AssistanceRequestResponse?>
        GetRequestForAdministrator(
            string id)
    {
        List<AssistanceRequestResponse>
            requests =
                await _requestService.GetAllAsync();


        return requests.FirstOrDefault(
            request =>
                request.Id == id);
    }
}


// ==================================================
// UPDATE STATUS REQUEST
// ==================================================

public class UpdateAssistanceRequestStatus
{
    public string Status { get; set; } =
        string.Empty;


    public string? DecisionReason { get; set; }
}