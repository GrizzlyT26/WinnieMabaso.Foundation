using System.Security.Claims;

using Azure.Storage.Blobs;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Enums;
using WinnieMabaso.Infrastructure.Azure;
using WinnieMabaso.Infrastructure.Firebase;
using WinnieMabaso.Infrastructure.Repositories;
using WinnieMabaso.Infrastructure.Services;


// ==================================================
// CREATE APPLICATION BUILDER
// ==================================================

var builder = WebApplication.CreateBuilder(args);


// ==================================================
// ADD SERVICES TO THE CONTAINER
// ==================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


// ==================================================
// SWAGGER
// Includes Firebase JWT Bearer Authentication
// ==================================================

builder.Services.AddSwaggerGen(options =>
{
    // --------------------------------------------------
    // Define Bearer Authentication
    // --------------------------------------------------

    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type =
                Microsoft.OpenApi.Models.SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In =
                Microsoft.OpenApi.Models.ParameterLocation.Header,

            Description =
                "Enter your Firebase ID token. " +
                "Format: Bearer {your Firebase ID token}"
        });


    // --------------------------------------------------
    // Apply Bearer Authentication to Swagger requests
    // --------------------------------------------------

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


// ==================================================
// HTTP CLIENT
// Used by Firebase Authentication REST API
// ==================================================

builder.Services.AddHttpClient();


// ==================================================
// FIREBASE INITIALIZATION
// ==================================================

var firebaseInitializer =
    new FirebaseInitializer();

builder.Services.AddSingleton(
    firebaseInitializer.Firestore);

builder.Services.AddSingleton<FirestoreService>();


// ==================================================
// AZURE BLOB STORAGE
// ==================================================

string azureStorageConnectionString =
    builder.Configuration[
        "AzureStorage:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Azure Storage connection string is not configured.");

builder.Services.AddSingleton(
    new BlobServiceClient(
        azureStorageConnectionString));

builder.Services.AddScoped<
    IAzureBlobStorageService,
    AzureBlobStorageService>();


// ==================================================
// REPOSITORIES
// ==================================================

builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();


builder.Services.AddScoped<
    IAssistanceRequestRepository,
    AssistanceRequestRepository>();


// --------------------------------------------------
// VOLUNTEER APPLICATION REPOSITORY
// --------------------------------------------------

builder.Services.AddScoped<
    IVolunteerApplicationRepository,
    VolunteerApplicationRepository>();


// --------------------------------------------------
// VOLUNTEER DOCUMENT REPOSITORY
// --------------------------------------------------

builder.Services.AddScoped<
    IVolunteerDocumentRepository,
    VolunteerDocumentRepository>();


// --------------------------------------------------
// PROGRAMME REPOSITORY
// --------------------------------------------------

builder.Services.AddScoped<
    IProgrammeRepository,
    ProgrammeRepository>();


// --------------------------------------------------
// EVENT REPOSITORY
// --------------------------------------------------

builder.Services.AddScoped<
    IEventRepository,
    EventRepository>();


// ==================================================
// APPLICATION SERVICES
// ==================================================

builder.Services.AddScoped<
    IAuthService,
    FirebaseAuthService>();


builder.Services.AddScoped<
    IUserService,
    UserService>();


builder.Services.AddScoped<
    IAssistanceRequestService,
    AssistanceRequestService>();


// --------------------------------------------------
// VOLUNTEER APPLICATION SERVICE
// --------------------------------------------------

builder.Services.AddScoped<
    IVolunteerApplicationService,
    VolunteerApplicationService>();


// --------------------------------------------------
// VOLUNTEER DOCUMENT SERVICE
// --------------------------------------------------

builder.Services.AddScoped<
    IVolunteerDocumentService,
    VolunteerDocumentService>();


// --------------------------------------------------
// PROGRAMME SERVICE
// --------------------------------------------------

builder.Services.AddScoped<
    IProgrammeService,
    ProgrammeService>();


// --------------------------------------------------
// EVENT SERVICE
// --------------------------------------------------

builder.Services.AddScoped<
    IEventService,
    EventService>();


// ==================================================
// FIREBASE BEARER AUTHENTICATION
// ==================================================

string firebaseProjectId =
    builder.Configuration["Firebase:ProjectId"]
    ?? "winnie-mabaso-foundation";


builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ----------------------------------------------
        // Firebase token authority
        // ----------------------------------------------

        options.Authority =
            $"https://securetoken.google.com/{firebaseProjectId}";


        // ----------------------------------------------
        // Firebase project ID / audience
        // ----------------------------------------------

        options.Audience =
            firebaseProjectId;

        options.RequireHttpsMetadata = true;


        // ----------------------------------------------
        // Token validation
        // ----------------------------------------------

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidIssuer =
                    $"https://securetoken.google.com/{firebaseProjectId}",

                ValidateAudience = true,

                ValidAudience =
                    firebaseProjectId,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                NameClaimType =
                    ClaimTypes.NameIdentifier,

                RoleClaimType =
                    ClaimTypes.Role
            };


        // ==================================================
        // TOKEN VALIDATED
        // Load application user from Firestore
        // ==================================================

        options.Events =
            new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    try
                    {
                        // ------------------------------------------
                        // Get Firebase UID from verified token
                        // ------------------------------------------

                        string? firebaseUid =
                            context.Principal?
                                .FindFirst("user_id")?.Value
                            ??
                            context.Principal?
                                .FindFirst(
                                    ClaimTypes.NameIdentifier)?.Value
                            ??
                            context.Principal?
                                .FindFirst("sub")?.Value;


                        if (string.IsNullOrWhiteSpace(
                            firebaseUid))
                        {
                            context.Fail(
                                "Firebase UID could not be found in the token.");

                            return;
                        }


                        // ------------------------------------------
                        // Get Firestore user repository
                        // ------------------------------------------

                        IUserRepository userRepository =
                            context.HttpContext
                                .RequestServices
                                .GetRequiredService<
                                    IUserRepository>();


                        // ------------------------------------------
                        // Find application user
                        // ------------------------------------------

                        var user =
                            await userRepository
                                .GetByFirebaseUidAsync(
                                    firebaseUid);


                        if (user == null)
                        {
                            context.Fail(
                                "Application user profile was not found.");

                            return;
                        }


                        // ------------------------------------------
                        // Check account status
                        // ------------------------------------------

                        if (user.Status != UserStatus.Active)
                        {
                            context.Fail(
                                "User account is not active.");

                            return;
                        }


                        // ------------------------------------------
                        // Get Claims Identity
                        // ------------------------------------------

                        ClaimsIdentity identity =
                            context.Principal?.Identity
                            as ClaimsIdentity
                            ?? new ClaimsIdentity();


                        // ------------------------------------------
                        // Firebase UID
                        // ------------------------------------------

                        if (!identity.HasClaim(
                            c => c.Type == "firebaseUid"))
                        {
                            identity.AddClaim(
                                new Claim(
                                    "firebaseUid",
                                    user.FirebaseUid));
                        }


                        // ------------------------------------------
                        // Application User ID
                        // ------------------------------------------

                        if (!identity.HasClaim(
                            c => c.Type == "userId"))
                        {
                            identity.AddClaim(
                                new Claim(
                                    "userId",
                                    user.Id));
                        }


                        // ------------------------------------------
                        // Full Name
                        // ------------------------------------------

                        if (!identity.HasClaim(
                            c => c.Type == ClaimTypes.Name))
                        {
                            identity.AddClaim(
                                new Claim(
                                    ClaimTypes.Name,
                                    user.FullName));
                        }


                        // ------------------------------------------
                        // Email
                        // ------------------------------------------

                        if (!identity.HasClaim(
                            c => c.Type == ClaimTypes.Email))
                        {
                            identity.AddClaim(
                                new Claim(
                                    ClaimTypes.Email,
                                    user.Email));
                        }


                        // ------------------------------------------
                        // Application Role
                        // ------------------------------------------

                        if (!identity.HasClaim(
                            c => c.Type == ClaimTypes.Role))
                        {
                            identity.AddClaim(
                                new Claim(
                                    ClaimTypes.Role,
                                    user.Role.ToString()));
                        }
                    }
                    catch
                    {
                        context.Fail(
                            "Unable to load the application user profile.");
                    }
                }
            };
    });


// ==================================================
// ROLE-BASED ACCESS CONTROL
// ==================================================

builder.Services.AddAuthorization(options =>
{
    // ==================================================
    // BENEFICIARY
    // ==================================================

    options.AddPolicy(
        "BeneficiaryOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.Beneficiary.ToString());
        });


    // ==================================================
    // VOLUNTEER
    // ==================================================

    options.AddPolicy(
        "VolunteerOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.Volunteer.ToString());
        });


    // ==================================================
    // DONOR
    // ==================================================

    options.AddPolicy(
        "DonorOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.Donor.ToString());
        });


    // ==================================================
    // ADMINISTRATOR
    // ==================================================

    options.AddPolicy(
        "AdministratorOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.Administrator.ToString());
        });


    // ==================================================
    // SUPER ADMINISTRATOR
    // ==================================================

    options.AddPolicy(
        "SuperAdministratorOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.SuperAdministrator.ToString());
        });


    // ==================================================
    // ADMINISTRATION
    // Administrator OR SuperAdministrator
    // ==================================================

    options.AddPolicy(
        "AdministrationOnly",
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.Administrator.ToString(),
                UserRole.SuperAdministrator.ToString());
        });
});


// ==================================================
// BUILD APPLICATION
// ==================================================

var app = builder.Build();


// ==================================================
// SWAGGER
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// ==================================================
// HTTPS
// ==================================================

app.UseHttpsRedirection();


// ==================================================
// AUTHENTICATION
// IMPORTANT:
// Authentication must come before Authorization
// ==================================================

app.UseAuthentication();


// ==================================================
// AUTHORIZATION
// ==================================================

app.UseAuthorization();


// ==================================================
// MAP CONTROLLERS
// ==================================================

app.MapControllers();


// ==================================================
// RUN APPLICATION
// ==================================================

app.Run();