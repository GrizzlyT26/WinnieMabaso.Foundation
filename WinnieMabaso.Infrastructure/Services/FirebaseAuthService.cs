using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using WinnieMabaso.Application.DTOs.Auth;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;
using WinnieMabaso.Domain.Enums;
using WinnieMabaso.Infrastructure.Repositories;

namespace WinnieMabaso.Infrastructure.Services;

public class FirebaseAuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FirebaseAuthService(
        IUserRepository userRepository,
        System.Net.Http.IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // ==================================================
    // CHECK IF USER EXISTS
    // ==================================================

    public async Task<bool> UserExistsAsync(string email)
    {
        try
        {
            await FirebaseAuth.DefaultInstance
                .GetUserByEmailAsync(email);

            return true;
        }
        catch (FirebaseAuthException)
        {
            return false;
        }
    }

    // ==================================================
    // REGISTER USER
    // ==================================================

    public async Task<User> RegisterAsync(
        RegisterRequest request)
    {
        if (await UserExistsAsync(request.Email))
        {
            throw new InvalidOperationException(
                "A user with this email address already exists.");
        }

        // --------------------------------------------------
        // Create user in Firebase Authentication
        // --------------------------------------------------

        UserRecordArgs firebaseUserArgs = new UserRecordArgs
        {
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.FullName,
            PhoneNumber = request.MobileNumber,
            Disabled = false
        };

        UserRecord firebaseUser =
            await FirebaseAuth.DefaultInstance
                .CreateUserAsync(firebaseUserArgs);

        // --------------------------------------------------
        // Determine application role
        // --------------------------------------------------

        UserRole role =
            Enum.IsDefined(
                typeof(UserRole),
                request.Role)
                    ? (UserRole)request.Role
                    : UserRole.Beneficiary;

        // --------------------------------------------------
        // Firestore requires UTC DateTime
        // --------------------------------------------------

        DateTime currentUtcTime =
            DateTime.UtcNow;

        // --------------------------------------------------
        // Create application user profile
        // --------------------------------------------------

        User user = new User
        {
            Id = Guid.NewGuid().ToString(),

            FirebaseUid = firebaseUser.Uid,

            FullName = request.FullName,

            Email = request.Email,

            MobileNumber = request.MobileNumber,

            Role = role,

            Status = UserStatus.Active,

            PrivacyNoticeAccepted =
                request.PrivacyNoticeAccepted,

            CreatedAt = currentUtcTime,

            UpdatedAt = currentUtcTime
        };

        // --------------------------------------------------
        // Save user profile to Firestore
        // --------------------------------------------------

        await _userRepository.CreateAsync(user);

        return user;
    }

    // ==================================================
    // LOGIN USER
    // ==================================================

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request)
    {
        // --------------------------------------------------
        // Validate email
        // --------------------------------------------------

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException(
                "Email address is required.");
        }

        // --------------------------------------------------
        // Validate password
        // --------------------------------------------------

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException(
                "Password is required.");
        }

        // --------------------------------------------------
        // Get Firebase Web API Key
        // --------------------------------------------------

        string? webApiKey =
            _configuration["Firebase:WebApiKey"];

        if (string.IsNullOrWhiteSpace(webApiKey))
        {
            throw new InvalidOperationException(
                "Firebase Web API key is not configured.");
        }

        // --------------------------------------------------
        // Firebase Authentication REST endpoint
        // --------------------------------------------------

        string firebaseLoginUrl =
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={webApiKey}";

        // --------------------------------------------------
        // Create Firebase login request
        // --------------------------------------------------

        FirebaseLoginRequest firebaseLoginRequest =
            new FirebaseLoginRequest
            {
                Email = request.Email,
                Password = request.Password,
                ReturnSecureToken = true
            };

        // --------------------------------------------------
        // Create HTTP client
        // --------------------------------------------------

        System.Net.Http.HttpClient httpClient =
            _httpClientFactory.CreateClient();

        // --------------------------------------------------
        // Send login request to Firebase
        // --------------------------------------------------

        HttpResponseMessage response =
            await httpClient.PostAsJsonAsync(
                firebaseLoginUrl,
                firebaseLoginRequest);

        // --------------------------------------------------
        // Handle Firebase authentication errors
        // --------------------------------------------------

        if (!response.IsSuccessStatusCode)
        {
            // Read the actual Firebase error response
            string firebaseError =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"Firebase authentication failed. " +
                $"HTTP {(int)response.StatusCode}: " +
                firebaseError);
        }

        // --------------------------------------------------
        // Read Firebase authentication response
        // --------------------------------------------------

        FirebaseLoginResponse? firebaseResponse =
            await response.Content
                .ReadFromJsonAsync<FirebaseLoginResponse>();

        if (firebaseResponse == null)
        {
            throw new InvalidOperationException(
                "Firebase returned an empty authentication response.");
        }

        if (string.IsNullOrWhiteSpace(
            firebaseResponse.IdToken))
        {
            throw new InvalidOperationException(
                "Firebase did not return a valid ID token.");
        }

        // --------------------------------------------------
        // Verify Firebase ID token
        // --------------------------------------------------

        FirebaseToken decodedToken;

        try
        {
            decodedToken =
                await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(
                        firebaseResponse.IdToken);
        }
        catch (FirebaseAuthException)
        {
            throw new InvalidOperationException(
                "Firebase authentication token could not be verified.");
        }

        // --------------------------------------------------
        // Get Firebase UID
        // --------------------------------------------------

        string firebaseUid =
            decodedToken.Uid;

        // --------------------------------------------------
        // Find application user in Firestore
        // --------------------------------------------------

        User? user =
            await _userRepository
                .GetByFirebaseUidAsync(
                    firebaseUid);

        if (user == null)
        {
            throw new InvalidOperationException(
                "User profile could not be found in Firestore.");
        }

        // --------------------------------------------------
        // Check account status
        // --------------------------------------------------

        if (user.Status != UserStatus.Active)
        {
            throw new InvalidOperationException(
                "This account is not active.");
        }

        // --------------------------------------------------
        // Return successful login response
        // --------------------------------------------------

        return new LoginResponse
        {
            IdToken =
                firebaseResponse.IdToken,

            RefreshToken =
                firebaseResponse.RefreshToken,

            ExpiresIn =
                firebaseResponse.ExpiresIn,

            User = user
        };
    }

    // ==================================================
    // FIREBASE LOGIN REQUEST
    // ==================================================

    private class FirebaseLoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken { get; set; }
    }

    // ==================================================
    // FIREBASE LOGIN RESPONSE
    // ==================================================

    private class FirebaseLoginResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("expiresIn")]
        public string ExpiresIn { get; set; } = string.Empty;

        [JsonPropertyName("localId")]
        public string LocalId { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}