using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.User;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await Mediator.Send(new GetActiveUsersCommand());
            return Ok(result);
        }

        [HttpGet("active/department/{departmentId}")]
        public async Task<IActionResult> GetActiveUsersByDepartment(int departmentId)
        {
            var result = await Mediator.Send(new GetActiveUsersByDepartmentCommand(departmentId));
            return Ok(result);
        }

        [HttpGet("active/allowed-department/{departmentId}")]
        public async Task<IActionResult> GetActiveUsersByAllowedDepartment(int departmentId)
        {
            var result = await Mediator.Send(new GetActiveUsersByAllowedDepartmentCommand(departmentId));
            return Ok(result);
        }
    }
}
