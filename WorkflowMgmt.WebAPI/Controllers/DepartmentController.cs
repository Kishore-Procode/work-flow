using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Auth;
using WorkflowMgmt.Application.Features.Department;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class DepartmentController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllDepartment()
        {
            var result = await Mediator.Send(new GetDepartmentCommand());
            return Ok(result);
        }

    }
}
