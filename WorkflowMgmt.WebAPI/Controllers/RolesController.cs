using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowRole;
using WorkflowMgmt.Domain.Models.WorkflowManagement;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/roles")]
    public class RolesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetRoles([FromQuery] bool? isActive = null)
        {
            if (isActive.HasValue && isActive.Value)
            {
                var result = await Mediator.Send(new GetActiveRolesQuery());
                return Ok(result);
            }

            // Default: get all roles
            var allResult = await Mediator.Send(new GetAllRolesQuery());
            return Ok(allResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var query = new GetRoleByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var command = new DeleteRoleCommand { Id = id };
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleRoleActive(int id)
        {
            var command = new ToggleRoleActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
