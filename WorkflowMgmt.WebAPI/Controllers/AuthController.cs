using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        //[HttpPost("refresh")]
        //public async Task<IActionResult> RefreshToken()
        //{
        //    // TODO: Implement refresh token functionality
        //    return Ok(new { message = "Refresh token endpoint - to be implemented" });
        //}

        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    // TODO: Implement logout functionality (invalidate tokens)
        //    return Ok(new { message = "Logout successful" });
        //}
    }
}
