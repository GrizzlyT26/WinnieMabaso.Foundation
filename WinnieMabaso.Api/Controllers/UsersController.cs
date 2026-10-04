using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinnieMabaso.Application.DTOs.Users;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService)
    {
        _userService = userService;
    }

    // ==================================================
    // GET CURRENT USER PROFILE
    // GET: /api/Users/me
    // ==================================================

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            // ----------------------------------------------
            // Get application user ID from authenticated
            // Firebase token claims
            // ----------------------------------------------

            string? userId =
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "Authenticated user ID could not be determined."
                });
            }

            // ----------------------------------------------
            // Get user profile
            // ----------------------------------------------

            UserDto? user =
                await _userService.GetByIdAsync(
                    userId);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "User profile could not be found."
                });
            }

            // ----------------------------------------------
            // Return profile
            // ----------------------------------------------

            return Ok(new
            {
                success = true,
                message = "User profile retrieved successfully.",

                user = new
                {
                    user.Id,
                    user.FirebaseUid,
                    user.FullName,
                    user.Email,
                    user.MobileNumber,
                    user.Role,
                    user.Status,
                    user.PrivacyNoticeAccepted,
                    user.CreatedAt
                }
            });
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while retrieving the user profile."
                });
        }
    }

    // ==================================================
    // GET USER BY ID
    // GET: /api/Users/{id}
    //
    // Protected endpoint.
    // This is primarily useful for administrative
    // functionality and will be restricted further
    // when we implement full RBAC.
    // ==================================================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User ID is required."
                });
            }

            UserDto? user =
                await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "User profile could not be found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "User profile retrieved successfully.",

                user = new
                {
                    user.Id,
                    user.FirebaseUid,
                    user.FullName,
                    user.Email,
                    user.MobileNumber,
                    user.Role,
                    user.Status,
                    user.PrivacyNoticeAccepted,
                    user.CreatedAt
                }
            });
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while retrieving the user profile."
                });
        }
    }

    // ==================================================
    // UPDATE CURRENT USER PROFILE
    // PUT: /api/Users/me
    // ==================================================

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request)
    {
        try
        {
            // ----------------------------------------------
            // Get authenticated application user ID
            // ----------------------------------------------

            string? userId =
                User.FindFirst("userId")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message =
                        "Authenticated user ID could not be determined."
                });
            }

            // ----------------------------------------------
            // Get existing profile
            // ----------------------------------------------

            UserDto? existingUser =
                await _userService.GetByIdAsync(
                    userId);

            if (existingUser == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "User profile could not be found."
                });
            }

            // ----------------------------------------------
            // Only update fields that the user is allowed
            // to change.
            // ----------------------------------------------

            existingUser.FullName =
                request.FullName.Trim();

            existingUser.MobileNumber =
                request.MobileNumber.Trim();

            await _userService.UpdateAsync(
                existingUser);

            // ----------------------------------------------
            // Return updated profile
            // ----------------------------------------------

            UserDto? updatedUser =
                await _userService.GetByIdAsync(
                    userId);

            if (updatedUser == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Updated user profile could not be found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "User profile updated successfully.",

                user = new
                {
                    updatedUser.Id,
                    updatedUser.FirebaseUid,
                    updatedUser.FullName,
                    updatedUser.Email,
                    updatedUser.MobileNumber,
                    updatedUser.Role,
                    updatedUser.Status,
                    updatedUser.PrivacyNoticeAccepted,
                    updatedUser.CreatedAt
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
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
        catch
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "An error occurred while updating the user profile."
                });
        }
    }
}