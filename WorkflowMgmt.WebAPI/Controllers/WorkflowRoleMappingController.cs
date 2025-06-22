using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowRoleMapping;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class WorkflowRoleMappingController : BaseApiController
    {
        [HttpGet("department-role-users")]
        public async Task<IActionResult> GetDepartmentRoleUsers()
        {
            var result = await Mediator.Send(new GetDepartmentRoleUsersCommand());
            return Ok(result);
        }

        [HttpGet("department-role-users/{departmentId}")]
        public async Task<IActionResult> GetDepartmentRoleUsersByDepartment(int departmentId)
        {
            var result = await Mediator.Send(new GetDepartmentRoleUsersByDepartmentCommand(departmentId));
            return Ok(result);
        }

        [HttpPut("department-role-users")]
        public async Task<IActionResult> UpdateDepartmentRoleUsers([FromBody] UpdateDepartmentRoleUsersRequest request)
        {
            var result = await Mediator.Send(new UpdateDepartmentRoleUsersCommand(request));
            return Ok(result);
        }
    }
}
