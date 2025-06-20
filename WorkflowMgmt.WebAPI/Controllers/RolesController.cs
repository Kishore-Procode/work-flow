using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowRole;

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
                var query = new GetActiveRolesQuery();
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            // Default: get all roles
            var allQuery = new GetAllRolesQuery();
            var allResult = await Mediator.Send(allQuery);
            return Ok(new { success = true, data = allResult });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var query = new GetRoleByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Role not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetRole), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Role not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var command = new DeleteRoleCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Role not found." });
            }

            return Ok(new { success = true, message = "Role deleted successfully." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleRoleActive(int id)
        {
            var command = new ToggleRoleActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Role not found." });
            }

            return Ok(new { success = true, data = result });
        }
    }
}
