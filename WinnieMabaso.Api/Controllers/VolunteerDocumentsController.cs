using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinnieMabaso.Application.DTOs.VolunteerDocuments;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VolunteerDocumentsController : ControllerBase
{
    private readonly IVolunteerDocumentService _service;

    public VolunteerDocumentsController(
        IVolunteerDocumentService service)
    {
        _service = service;
    }


    // ==================================================
    // UPLOAD DOCUMENT
    // Volunteer only
    // ==================================================

    [HttpPost]
    [Authorize(Policy = "VolunteerOnly")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        [FromForm] string volunteerApplicationId,
        [FromForm] string documentType,
        IFormFile file)
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
                            "User ID could not be determined."
                    });
            }

            VolunteerDocumentResponse response =
                await _service.UploadAsync(
                    userId,
                    volunteerApplicationId,
                    documentType,
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType,
                    file.Length);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = response.Id
                },
                response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    message = ex.Message
                });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }


    // ==================================================
    // GET MY DOCUMENTS
    // ==================================================

    [HttpGet("my")]
    [Authorize(Policy = "VolunteerOnly")]
    public async Task<IActionResult> GetMyDocuments()
    {
        string? userId =
            User.FindFirst("userId")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "User ID could not be determined."
                });
        }

        List<VolunteerDocumentResponse> documents =
            await _service.GetMyDocumentsAsync(
                userId);

        return Ok(documents);
    }


    // ==================================================
    // GET DOCUMENTS BY APPLICATION
    // ==================================================

    [HttpGet("application/{applicationId}")]
    [Authorize(Policy = "VolunteerOnly")]
    public async Task<IActionResult> GetByApplication(
        string applicationId)
    {
        string? userId =
            User.FindFirst("userId")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "User ID could not be determined."
                });
        }

        try
        {
            List<VolunteerDocumentResponse> documents =
                await _service.GetByApplicationAsync(
                    applicationId,
                    userId);

            return Ok(documents);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    message = ex.Message
                });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }


    // ==================================================
    // GET DOCUMENT BY ID
    // ==================================================

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(
        string id)
    {
        string? userId =
            User.FindFirst("userId")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "User ID could not be determined."
                });
        }


        bool isAdministrator =
            User.IsInRole("Administrator")
            ||
            User.IsInRole("SuperAdministrator");


        try
        {
            if (isAdministrator)
            {
                // Administrators will receive the document
                // metadata through the service once the
                // administrator document-review workflow
                // is implemented.
                return StatusCode(
                    StatusCodes.Status501NotImplemented,
                    new
                    {
                        message =
                            "Administrator document retrieval will be implemented with the administration workflow."
                    });
            }


            VolunteerDocumentResponse? response =
                await _service.GetByIdAsync(
                    id,
                    userId);

            if (response == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Volunteer document could not be found."
                    });
            }

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }


    // ==================================================
    // DELETE DOCUMENT
    // ==================================================

    [HttpDelete("{id}")]
    [Authorize(Policy = "VolunteerOnly")]
    public async Task<IActionResult> Delete(
        string id)
    {
        string? userId =
            User.FindFirst("userId")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "User ID could not be determined."
                });
        }

        try
        {
            await _service.DeleteAsync(
                id,
                userId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    message = ex.Message
                });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}