using Microsoft.AspNetCore.Mvc;
using WinnieMabaso.Application.DTOs.Auth;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // --------------------------------------------------
    // Register User
    // POST: api/Auth/register
    // --------------------------------------------------

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        try
        {
            // Validate full name
            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Full name is required."
                });
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email address is required."
                });
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Password is required."
                });
            }

            // Validate privacy notice
            if (!request.PrivacyNoticeAccepted)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Privacy notice must be accepted."
                });
            }

            // Register user
            var user = await _authService.RegisterAsync(request);

            // Return successful response
            return Ok(new
            {
                success = true,
                message = "User registered successfully.",
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
                    user.CreatedAt,
                    user.UpdatedAt
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while registering the user.",
                error = ex.Message
            });
        }
    }

    // --------------------------------------------------
    // Login User
    // POST: api/Auth/login
    // --------------------------------------------------

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        try
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email address is required."
                });
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Password is required."
                });
            }

            // Authenticate user
            var loginResult =
                await _authService.LoginAsync(request);

            // Return successful login response
            return Ok(new
            {
                success = true,
                message = "Login successful.",

                // Firebase authentication tokens
                idToken = loginResult.IdToken,
                refreshToken = loginResult.RefreshToken,
                expiresIn = loginResult.ExpiresIn,

                // Application user profile
                user = new
                {
                    loginResult.User.Id,
                    loginResult.User.FirebaseUid,
                    loginResult.User.FullName,
                    loginResult.User.Email,
                    loginResult.User.MobileNumber,
                    loginResult.User.Role,
                    loginResult.User.Status
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while logging in.",
                error = ex.Message
            });
        }
    }
}