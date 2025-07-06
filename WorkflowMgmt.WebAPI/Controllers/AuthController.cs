using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using WorkflowMgmt.Application.Features.Auth;

namespace WorkflowMgmt.WebAPI.Controllers
{
    public class AuthController : BaseApiController
    {
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        //{
        //    var result = await Mediator.Send(command);
        //    return Ok(result);
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            if (command == null) return BadRequest("Login data is required");
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var query = new GetProfileQuery(userId);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var command = new LogoutCommand(userId, request?.RefreshToken);
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Logout failed: {ex.Message}" });
            }
        }
    }

    public class LogoutRequest
    {
        public string? RefreshToken { get; set; }
    }
}
