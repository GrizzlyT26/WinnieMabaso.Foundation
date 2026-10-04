using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    // ==================================================
    // PUBLIC ENDPOINT
    // Does not require authentication
    // ==================================================

    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok(new
        {
            success = true,
            message = "This is a public endpoint.",
            authenticated =
                User.Identity?.IsAuthenticated ?? false
        });
    }

    // ==================================================
    // PROTECTED ENDPOINT
    // Requires Firebase authentication
    // ==================================================

    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        string? firebaseUid =
            User.FindFirst("firebaseUid")?.Value;

        string? userId =
            User.FindFirst("userId")?.Value;

        string? fullName =
            User.FindFirst(ClaimTypes.Name)?.Value;

        string? email =
            User.FindFirst(ClaimTypes.Email)?.Value;

        string? role =
            User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            success = true,
            message = "You are authenticated.",

            firebaseUid = firebaseUid,

            userId = userId,

            fullName = fullName,

            email = email,

            role = role
        });
    }

    // ==================================================
    // ADMIN-ONLY ENDPOINT
    // ==================================================

    [HttpGet("admin")]
    [Authorize(Roles = "Administrator")]
    public IActionResult Admin()
    {
        return Ok(new
        {
            success = true,
            message =
                "You are authenticated as an Administrator."
        });
    }
}