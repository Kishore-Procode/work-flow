using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Application.Features.UserManagement;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class UserManagementController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUser(
            [FromQuery] int? departmentId = null,
            [FromQuery] string? roleCode = null,
            [FromQuery] bool? isActive = null)
        {
            // If specific filters are provided, use the appropriate query
            if (departmentId.HasValue)
            {
                // For now, return all users and filter on frontend
                // TODO: Implement department filtering in UserManagementRepository
            }

            if (!string.IsNullOrWhiteSpace(roleCode))
            {
                // For now, return all users and filter on frontend
                // TODO: Implement role filtering in UserManagementRepository
            }

            if (isActive.HasValue)
            {
                // For now, return all users and filter on frontend
                // TODO: Implement active filtering in UserManagementRepository
            }

            var result = await Mediator.Send(new GetUserManagementCommand());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserManagementById(Guid id)
        {
            var result = await Mediator.Send(new GetUserManagementById(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            user.created_by = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateUserCommand(user));
            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDTO user)
        {
            UserDTO userdetail = user;
            userdetail.id = id;
            userdetail.modified_by = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateUserCommand(userdetail));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreUserCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreUser(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreUserCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            var result = await Mediator.Send(new UpdatePasswordCommand(request));

            if (!result.Data)
                return BadRequest(new { success = false, message = "Old password is incorrect or update failed" });

            return Ok(new { success = true, message = "Password updated successfully" });
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest updateProfileRequest)
        {
            updateProfileRequest.id = id;
            var result = await Mediator.Send(new UpdateProfileCommand(updateProfileRequest));
            return Ok(result);
        }

    }

}
