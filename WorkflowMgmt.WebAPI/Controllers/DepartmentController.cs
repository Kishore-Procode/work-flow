using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Auth;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Domain.Entities;

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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var result = await Mediator.Send(new GetDepartmentByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDTO department)
        {
            department.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateDepartmentCommand(department));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentDTO department)
        {
            DepartmentDTO dept = department;
            dept.Id = id;
            dept.ModifiedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateDepartmentCommand(dept));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteDepartment(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "admin";
            var result = await Mediator.Send(new DeleteOrRestoreDepartmentCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreDepartment(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "admin";
            var result = await Mediator.Send(new DeleteOrRestoreDepartmentCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }

    }
}
