using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FilesController> _logger;
        private readonly IConfiguration _configuration;

        public FilesController(IWebHostEnvironment environment, ILogger<FilesController> logger, IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
        }

        private bool ValidateTokenFromQuery(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("syllabi/{syllabusId}/{fileName}")]
        [AllowAnonymous] // Allow anonymous access but validate token manually
        public async Task<IActionResult> GetSyllabusFile(Guid syllabusId, string fileName, [FromQuery] string? token = null)
        {
            try
            {
                // Check authentication - either from header or query parameter
                bool isAuthenticated = User.Identity?.IsAuthenticated == true;
                if (!isAuthenticated && !string.IsNullOrEmpty(token))
                {
                    isAuthenticated = ValidateTokenFromQuery(token);
                }

                if (!isAuthenticated)
                {
                    return Unauthorized(new { success = false, message = "Authentication required." });
                }

                // Validate file name to prevent directory traversal attacks
                if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest(new { success = false, message = "Invalid file name." });
                }

                // Construct the file path
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "syllabi", syllabusId.ToString());
                var filePath = Path.Combine(uploadsPath, fileName);

                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { success = false, message = "File not found." });
                }

                // Get file info
                var fileInfo = new FileInfo(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

                // Determine content type
                var contentType = GetContentType(fileExtension);

                // Read file content
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Log file access
                _logger.LogInformation("File accessed: {FilePath} by user {UserId}", 
                    filePath, User?.Identity?.Name ?? "Unknown");

                // Return file with appropriate headers
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing file: syllabi/{SyllabusId}/{FileName}", syllabusId, fileName);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpGet("syllabi/{syllabusId}/{fileName}/download")]
        public async Task<IActionResult> DownloadSyllabusFile(Guid syllabusId, string fileName)
        {
            try
            {
                // Validate file name to prevent directory traversal attacks
                if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest(new { success = false, message = "Invalid file name." });
                }

                // Construct the file path
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "syllabi", syllabusId.ToString());
                var filePath = Path.Combine(uploadsPath, fileName);

                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { success = false, message = "File not found." });
                }

                // Get file info
                var fileInfo = new FileInfo(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

                // Determine content type
                var contentType = GetContentType(fileExtension);

                // Read file content
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Log file download
                _logger.LogInformation("File downloaded: {FilePath} by user {UserId}", 
                    filePath, User?.Identity?.Name ?? "Unknown");

                // Return file as download with appropriate headers
                return File(fileBytes, contentType, fileName, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: syllabi/{SyllabusId}/{FileName}", syllabusId, fileName);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpGet("syllabi/{syllabusId}/{fileName}/info")]
        public IActionResult GetSyllabusFileInfo(Guid syllabusId, string fileName)
        {
            try
            {
                // Validate file name to prevent directory traversal attacks
                if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest(new { success = false, message = "Invalid file name." });
                }

                // Construct the file path
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "syllabi", syllabusId.ToString());
                var filePath = Path.Combine(uploadsPath, fileName);

                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { success = false, message = "File not found." });
                }

                // Get file info
                var fileInfo = new FileInfo(filePath);

                var result = new
                {
                    success = true,
                    data = new
                    {
                        fileName = fileName,
                        fileSize = fileInfo.Length,
                        createdDate = fileInfo.CreationTime,
                        modifiedDate = fileInfo.LastWriteTime,
                        extension = Path.GetExtension(fileName),
                        contentType = GetContentType(Path.GetExtension(fileName).ToLowerInvariant())
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file info: syllabi/{SyllabusId}/{FileName}", syllabusId, fileName);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        private static string GetContentType(string fileExtension)
        {
            return fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                ".rtf" => "application/rtf",
                ".md" => "text/markdown",
                ".csv" => "text/csv",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".7z" => "application/x-7z-compressed",
                _ => "application/octet-stream"
            };
        }
    }
}
