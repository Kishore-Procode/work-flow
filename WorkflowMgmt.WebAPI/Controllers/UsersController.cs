using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowUser;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? departmentId = null,
            [FromQuery] string? roleCode = null,
            [FromQuery] bool? isActive = null)
        {
            if (departmentId.HasValue)
            {
                var query = new GetUsersByDepartmentQuery { DepartmentId = departmentId.Value };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (!string.IsNullOrWhiteSpace(roleCode))
            {
                var query = new GetUsersByRoleCodeQuery { RoleCode = roleCode };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (isActive.HasValue && isActive.Value)
            {
                var query = new GetActiveUsersQuery();
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            // Default: get all users
            var allQuery = new GetAllUsersQuery();
            var allResult = await Mediator.Send(allQuery);
            return Ok(new { success = true, data = allResult });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetUser), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "User not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            return Ok(new { success = true, message = "User deleted successfully." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(Guid id)
        {
            var command = new ToggleUserActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            return Ok(new { success = true, data = result });
        }
    }
}
