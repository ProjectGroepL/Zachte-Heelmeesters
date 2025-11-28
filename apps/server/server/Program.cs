using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ZhmApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file in development
if (builder.Environment.IsDevelopment())
{
    Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
}

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

// Register ApiContext with DI
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("CONNECTION_STRING environment variable or DefaultConnection in appsettings is required");

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        policy => policy
            .WithOrigins(
                "http://localhost:5173", // Vue dev server HTTP
                "https://localhost:7048" // API HTTPS redirect
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


var app = builder.Build();

// Configure forwarded headers for proxy support
app.UseForwardedHeaders();

// Use CORS
app.UseCors("AllowLocalhost5173");

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
app.UseAuthorization();

app.MapControllers();

app.Run();
