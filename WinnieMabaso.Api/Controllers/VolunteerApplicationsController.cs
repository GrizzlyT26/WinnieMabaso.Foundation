using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinnieMabaso.Application.DTOs.VolunteerApplications;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VolunteerApplicationsController
    : ControllerBase
{
    private readonly IVolunteerApplicationService
        _service;

    public VolunteerApplicationsController(
        IVolunteerApplicationService service)
    {
        _service = service;
    }


    // ==================================================
    // CREATE VOLUNTEER APPLICATION
    // Volunteer only
    // POST: /api/VolunteerApplications
    // ==================================================

    [HttpPost]
    [Authorize(Policy = "VolunteerOnly")]
    public async Task<IActionResult> Create(
        [FromBody] CreateVolunteerApplication request)
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Application user ID could not be determined."
                    });
            }

            VolunteerApplicationResponse response =
                await _service.CreateAsync(
                    userId,
                    request);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = response.Id
                },
                response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "An error occurred while creating the volunteer application."
                });
        }
    }


    // ==================================================
    // GET MY APPLICATIONS
    // Volunteer only
    // GET: /api/VolunteerApplications/my
    // ==================================================

    [HttpGet("my")]
    [Authorize(Policy = "VolunteerOnly")]
    public async Task<IActionResult> GetMyApplications()
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Application user ID could not be determined."
                    });
            }

            List<VolunteerApplicationResponse> applications =
                await _service.GetMyApplicationsAsync(
                    userId);

            return Ok(applications);
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "An error occurred while retrieving your volunteer applications."
                });
        }
    }


    // ==================================================
    // GET APPLICATION BY ID
    // Volunteer owner OR Administrator
    // GET: /api/VolunteerApplications/{id}
    // ==================================================

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(
        string id)
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Application user ID could not be determined."
                    });
            }


            // --------------------------------------------------
            // Administrator / SuperAdministrator
            // --------------------------------------------------

            bool isAdministrator =
                User.IsInRole("Administrator")
                ||
                User.IsInRole("SuperAdministrator");

            if (isAdministrator)
            {
                VolunteerApplicationResponse? adminResponse =
                    await _service.GetAllAsync()
                        .ContinueWith(task =>
                            task.Result.FirstOrDefault(
                                application =>
                                    application.Id == id));

                if (adminResponse == null)
                {
                    return NotFound(
                        new
                        {
                            message =
                                "Volunteer application could not be found."
                        });
                }

                return Ok(adminResponse);
            }


            // --------------------------------------------------
            // Volunteer owner
            // --------------------------------------------------

            if (!User.IsInRole("Volunteer"))
            {
                return Forbid();
            }

            VolunteerApplicationResponse? response =
                await _service.GetByIdAsync(
                    id,
                    userId);

            if (response == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Volunteer application could not be found."
                    });
            }

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "An error occurred while retrieving the volunteer application."
                });
        }
    }


    // ==================================================
    // GET ALL APPLICATIONS
    // Administrator / SuperAdministrator only
    // GET: /api/VolunteerApplications
    // ==================================================

    [HttpGet]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<VolunteerApplicationResponse> applications =
                await _service.GetAllAsync();

            return Ok(applications);
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "An error occurred while retrieving volunteer applications."
                });
        }
    }


    // ==================================================
    // UPDATE APPLICATION STATUS
    // Administrator / SuperAdministrator only
    // PUT: /api/VolunteerApplications/{id}/status
    // ==================================================

    [HttpPut("{id}/status")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> UpdateStatus(
        string id,
        [FromBody] UpdateVolunteerApplicationStatus request)
    {
        try
        {
            string? userId =
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Administrator user ID could not be determined."
                    });
            }

            if (request == null)
            {
                return BadRequest(
                    new
                    {
                        message =
                            "Request body is required."
                    });
            }

            await _service.UpdateStatusAsync(
                id,
                request.Status,
                request.AdminNotes ?? string.Empty,
                userId);

            return Ok(
                new
                {
                    message =
                        "Volunteer application status updated successfully."
                });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "An error occurred while updating the volunteer application."
                });
        }
    }


    // ==================================================
    // DTO FOR STATUS UPDATE
    // ==================================================

    public class UpdateVolunteerApplicationStatus
    {
        public string Status { get; set; } =
            string.Empty;

        public string? AdminNotes { get; set; }
    }
}