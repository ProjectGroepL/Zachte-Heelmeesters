using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ZhmApi.Data;
using ZhmApi.Models;
using ZhmApi.Services;
using ZhmApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file in development
if (builder.Environment.IsDevelopment())
{
    Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
}

// Get configuration from environment variables (with fallbacks to appsettings)
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT_KEY environment variable or JWT:Key in appsettings is required");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT_ISSUER environment variable or JWT:Issuer in appsettings is required");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["JWT:Audience"] ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable or JWT:Audience in appsettings is required");
var refreshTokenExpirationDays = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_DAYS") ?? builder.Configuration["JWT:RefreshTokenExpirationDays"] ?? "7");

// Add JSON options to serialize enums as strings
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure forwarded headers to support reverse proxies
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Handle CORS
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS")?.Split(',') ?? ["http://localhost:5173", "https://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(corsOrigins) // From environment variable
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register ApiContext with DI
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("CONNECTION_STRING environment variable or DefaultConnection in appsettings is required");

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApiContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
        
    };
});

// Register JWT service
builder.Services.AddScoped<IJwtService, JwtService>();

// Register Token service
builder.Services.AddScoped<ITokenService, TokenService>();

// Register 2FA services
builder.Services.AddScoped<TwoFactorService>();

// Register Email services - configure from environment variables
builder.Services.Configure<EmailSettings>(options =>
{
    options.Host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? "sandbox.smtp.mailtrap.io";
    options.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587");
    options.Username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
    options.Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
    options.SenderName = Environment.GetEnvironmentVariable("EMAIL_SENDER_NAME") ?? "Zachte Heelmeesters";
    options.SenderEmail = Environment.GetEnvironmentVariable("EMAIL_SENDER_EMAIL") ?? "no-reply@zhm.local";
});

// register referralservice
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<MedicalDocumentService>(); // Die stond er waarschijnlijk al
builder.Services.AddScoped<AccessRequestService>();

builder.Services.AddScoped<AppointmentReportService>();
builder.Services.AddScoped<NotificationService>();

// Register email sender based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
    // builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
}

var app = builder.Build();

// Configure forwarded headers for proxy support
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI();

    //TODO: auto detect if using proxy, if so use this configuration instead
    // app.UseSwaggerUI(c =>
    // {
    //     c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "API V1");
    //     c.RoutePrefix = "swagger";
    // });
}

app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

// use middleware for automatic logging of what needs to be logged
app.UseMiddleware<AuditTrailMiddleware>();

app.MapControllers();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiContext>();
    context.Database.Migrate();
}

// new roles and users so that we can login more easily
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var roles = new[]
    {
        "Patient", "Specialist", "Huisarts",
        "Zorgverzekeraar", "Systeembeheerder", "Administratie"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = role,
                NormalizedName = role.ToUpper()
            });
        }
    }

    // Helper method to create users
    async Task CreateUser(string email, string password, string firstName, string lastName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                City = "Amsterdam",
                Country = "NL",
                ZipCode = "1000AA",
                Street = "Main",
                HouseNumber = "1",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }

    await CreateUser("admin@zhm.nl", "Admin123!", "System", "Admin", "Systeembeheerder");
    await CreateUser("huisarts@zhm.nl", "Doctor123!", "Harry", "Huisarts", "Huisarts");
    await CreateUser("specialist@zhm.nl", "Specialist123!", "Sam", "Specialist", "Specialist");
    await CreateUser("patient@zhm.nl", "Patient123!", "Pieter", "Patient", "Patient");
    await CreateUser("verzekeraar@zhm.nl", "Insurance123!", "Vera", "Verzekeraar", "Zorgverzekeraar");
    await CreateUser("admin-ziekenhuis@zhm.nl", "AdminMed123!", "Anja", "Admin", "Administratie");
}
app.Run();
