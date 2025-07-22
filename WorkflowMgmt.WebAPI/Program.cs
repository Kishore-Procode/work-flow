using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using WorkflowMgmt.Application.DependencyInjection;
using WorkflowMgmt.Application.Hubs;
using WorkflowMgmt.Infrastructure.DependencyInjection;
using WorkflowMgmt.WebAPI.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Configure environment-specific settings
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Starting application in {environment} environment");

// Override configuration with environment variables if they exist
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPassword))
{
    var connectionString = $"Host={dbHost};Port={dbPort ?? "5432"};Database={dbName ?? "workflow"};Username={dbUser ?? "postgres"};Password={dbPassword}";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    Console.WriteLine($"Using database connection: Host={dbHost}, Database={dbName ?? "workflow"}");
}

// Override JWT settings from environment variables
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Configuration["Jwt:Key"] = jwtKey;
    Console.WriteLine("JWT Key loaded from environment variable");
}
if (!string.IsNullOrEmpty(jwtIssuer))
{
    builder.Configuration["Jwt:Issuer"] = jwtIssuer;
}
if (!string.IsNullOrEmpty(jwtAudience))
{
    builder.Configuration["Jwt:Audience"] = jwtAudience;
}

// Configure Serilog based on environment
var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext();

// Environment-specific logging configuration
if (builder.Environment.IsDevelopment())
{
    loggerConfig
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
}
else
{
    loggerConfig
        .WriteTo.Console()
        .WriteTo.File("/app/logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            fileSizeLimitBytes: 10485760,
            rollOnFileSizeLimit: true);
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Education Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        // Configure JWT for SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // If the request is for our hub and we have a token, use it
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token is valid.");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
// Add CORS with environment-specific configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow any origin for easier testing
            corsBuilder.SetIsOriginAllowed(origin => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
        else
        {
            // Production: Use specific allowed origins from configuration
            var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>()
                               ?? ["https://act.conprg.com"];

            corsBuilder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

//builder.Services.AddMediatR(typeof(UserCommandHandler).Assembly);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Log application startup
Log.Information("Application built successfully");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
Log.Information("Content Root: {ContentRoot}", app.Environment.ContentRootPath);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkFlow Management API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
    app.UseDeveloperExceptionPage();

    // Add request logging for development
    app.Use(async (context, next) =>
    {
        var startTime = DateTime.UtcNow;
        Log.Information("HTTP {Method} {Path} started", context.Request.Method, context.Request.Path);

        await next();

        var duration = DateTime.UtcNow - startTime;
        Log.Information("HTTP {Method} {Path} responded {StatusCode} in {Duration}ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, duration.TotalMilliseconds);
    });
}
else
{
    // Production: Only enable Swagger if explicitly configured
    var enableSwaggerInProduction = builder.Configuration.GetValue<bool>("EnableSwaggerInProduction", false);
    if (enableSwaggerInProduction)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkFlow Management API v1");
            c.RoutePrefix = "api-docs"; // Different route for production
        });
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// Enable static file serving from wwwroot
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Map SignalR Hub
app.MapHub<NotificationHub>("/notificationHub").RequireAuthorization();

try
{
    Log.Information("Starting WorkFlow Management API");
    // Log before starting the application
Log.Information("Starting WorkFlow Management API...");
Log.Information("API will be available at: {Urls}", string.Join(", ", app.Urls));

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
