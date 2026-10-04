using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WinnieMabaso.Application.DTOs.Programmes;
using WinnieMabaso.Application.Interfaces;


namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgrammesController : ControllerBase
{
    private readonly IProgrammeService _programmeService;


    public ProgrammesController(
        IProgrammeService programmeService)
    {
        _programmeService = programmeService;
    }


    // ==================================================
    // GET ACTIVE PROGRAMMES
    // Beneficiary / Volunteer / Donor
    // ==================================================

    [HttpGet("active")]
    [Authorize]
    public async Task<IActionResult> GetActive()
    {
        var programmes =
            await _programmeService.GetActiveAsync();

        return Ok(programmes);
    }


    // ==================================================
    // GET ALL PROGRAMMES
    // Administrator / SuperAdministrator
    // ==================================================

    [HttpGet]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> GetAll()
    {
        var programmes =
            await _programmeService.GetAllAsync();

        return Ok(programmes);
    }


    // ==================================================
    // GET PROGRAMME BY ID
    // Authenticated users
    // ==================================================

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(
        string id)
    {
        var programme =
            await _programmeService.GetByIdAsync(id);

        if (programme == null)
        {
            return NotFound(
                new
                {
                    message =
                        "Programme could not be found."
                });
        }


        // ----------------------------------------------
        // Non-administrators can only view active
        // programmes.
        // ----------------------------------------------

        bool isAdministrator =
            User.IsInRole("Administrator") ||
            User.IsInRole("SuperAdministrator");


        if (!isAdministrator &&
            !programme.IsActive)
        {
            return NotFound(
                new
                {
                    message =
                        "Programme could not be found."
                });
        }


        return Ok(programme);
    }


    // ==================================================
    // CREATE PROGRAMME
    // Administrator / SuperAdministrator
    // ==================================================

    [HttpPost]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProgramme request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(
                ModelState);
        }


        string? userId =
            User.FindFirstValue("userId");


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            var programme =
                await _programmeService.CreateAsync(
                    userId,
                    request);


            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = programme.Id
                },
                programme);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==================================================
    // UPDATE PROGRAMME
    // Administrator / SuperAdministrator
    // ==================================================

    [HttpPut("{id}")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] CreateProgramme request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(
                ModelState);
        }


        string? userId =
            User.FindFirstValue("userId");


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            await _programmeService.UpdateAsync(
                id,
                userId,
                request);


            var updatedProgramme =
                await _programmeService.GetByIdAsync(
                    id);


            return Ok(updatedProgramme);
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
    }


    // ==================================================
    // DEACTIVATE PROGRAMME
    // Administrator / SuperAdministrator
    // ==================================================

    [HttpPut("{id}/deactivate")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> Deactivate(
        string id)
    {
        string? userId =
            User.FindFirstValue("userId");


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            await _programmeService.DeactivateAsync(
                id,
                userId);


            return Ok(
                new
                {
                    message =
                        "Programme deactivated successfully."
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
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==================================================
    // ACTIVATE PROGRAMME
    // Administrator / SuperAdministrator
    // ==================================================

    [HttpPut("{id}/activate")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> Activate(
        string id)
    {
        string? userId =
            User.FindFirstValue("userId");


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            await _programmeService.ActivateAsync(
                id,
                userId);


            return Ok(
                new
                {
                    message =
                        "Programme activated successfully."
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
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==================================================
    // DELETE PROGRAMME
    // Administrator / SuperAdministrator
    // ==================================================

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult> Delete(
        string id)
    {
        try
        {
            await _programmeService.DeleteAsync(
                id);


            return Ok(
                new
                {
                    message =
                        "Programme deleted successfully."
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
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }
}