using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleTestController : ControllerBase
{
    // ==================================================
    // ANY AUTHENTICATED USER
    // ==================================================

    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult Authenticated()
    {
        return Ok(new
        {
            success = true,
            message = "You are authenticated.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }


    // ==================================================
    // BENEFICIARY ONLY
    // ==================================================

    [HttpGet("beneficiary")]
    [Authorize(Policy = "BeneficiaryOnly")]
    public IActionResult Beneficiary()
    {
        return Ok(new
        {
            success = true,
            message = "Beneficiary access granted.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }


    // ==================================================
    // VOLUNTEER ONLY
    // ==================================================

    [HttpGet("volunteer")]
    [Authorize(Policy = "VolunteerOnly")]
    public IActionResult Volunteer()
    {
        return Ok(new
        {
            success = true,
            message = "Volunteer access granted.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }


    // ==================================================
    // DONOR ONLY
    // ==================================================

    [HttpGet("donor")]
    [Authorize(Policy = "DonorOnly")]
    public IActionResult Donor()
    {
        return Ok(new
        {
            success = true,
            message = "Donor access granted.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }


    // ==================================================
    // ADMINISTRATOR ONLY
    // ==================================================

    [HttpGet("administrator")]
    [Authorize(Policy = "AdministratorOnly")]
    public IActionResult Administrator()
    {
        return Ok(new
        {
            success = true,
            message = "Administrator access granted.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }


    // ==================================================
    // SUPER ADMINISTRATOR ONLY
    // ==================================================

    [HttpGet("super-administrator")]
    [Authorize(Policy = "SuperAdministratorOnly")]
    public IActionResult SuperAdministrator()
    {
        return Ok(new
        {
            success = true,
            message = "Super Administrator access granted.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }


    // ==================================================
    // ADMINISTRATION
    // ADMINISTRATOR OR SUPER ADMINISTRATOR
    // ==================================================

    [HttpGet("administration")]
    [Authorize(Policy = "AdministrationOnly")]
    public IActionResult Administration()
    {
        return Ok(new
        {
            success = true,
            message = "Administration access granted.",
            role = User.FindFirst(
                ClaimTypes.Role)?.Value
        });
    }
}