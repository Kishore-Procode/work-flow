using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Logout()
        {
            // TODO: Implement logout functionality (invalidate tokens)
            return Ok(new { message = "Logout successful" });
        }
    }
}
