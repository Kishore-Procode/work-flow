using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Application.Features.UserManagement;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class UserManagementController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await Mediator.Send(new GetUserManagementCommand());
            return Ok(result);
        }
    }
}
