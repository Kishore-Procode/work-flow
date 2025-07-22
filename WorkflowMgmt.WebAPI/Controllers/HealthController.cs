using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace WorkflowMgmt.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public HealthController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = _environment.EnvironmentName,
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString()
        });
    }

    /// <summary>
    /// Detailed health check with system information
    /// </summary>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    public IActionResult GetDetailedHealth()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = _environment.EnvironmentName,
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            ApplicationName = _configuration["ApplicationName"] ?? "WorkFlow Management API",
            ApplicationVersion = _configuration["ApplicationVersion"] ?? "1.0.0",
            MachineName = Environment.MachineName,
            ProcessorCount = Environment.ProcessorCount,
            WorkingSet = Environment.WorkingSet,
            OSVersion = Environment.OSVersion.ToString(),
            CLRVersion = Environment.Version.ToString(),
            Uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime(),
            Database = new
            {
                Status = "Connected", // You can add actual database health check here
                Provider = "PostgreSQL"
            },
            Features = new
            {
                JWT = "Enabled",
                CORS = "Enabled",
                SignalR = "Enabled",
                Swagger = _environment.IsDevelopment() ? "Enabled" : "Disabled"
            }
        };

        return Ok(healthInfo);
    }
}
