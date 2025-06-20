using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowStagePermissions;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/workflow-stage-permissions")]
    public class WorkflowStagePermissionsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkflowStagePermissions([FromQuery] Guid workflow_stage_id)
        {
            if (workflow_stage_id == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "workflow_stage_id is required." });
            }

            var query = new GetWorkflowStagePermissionsQuery { WorkflowStageId = workflow_stage_id };
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("{stageId}")]
        public async Task<IActionResult> UpdateWorkflowStagePermissions(Guid stageId, [FromBody] UpdateStagePermissionsRequest request)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid stage ID." });
            }

            try
            {
                var command = new UpdateWorkflowStagePermissionsCommand 
                { 
                    StageId = stageId, 
                    Permissions = request.Permissions 
                };

                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to update workflow stage permissions." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{stageId}/permissions")]
        public async Task<IActionResult> AddWorkflowStagePermission(Guid stageId, [FromBody] AddPermissionRequest request)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid stage ID." });
            }

            if (string.IsNullOrWhiteSpace(request.PermissionName))
            {
                return BadRequest(new { success = false, message = "Permission name is required." });
            }

            try
            {
                var command = new AddWorkflowStagePermissionCommand 
                { 
                    StageId = stageId, 
                    PermissionName = request.PermissionName,
                    IsRequired = request.IsRequired
                };

                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to add permission to workflow stage." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{stageId}/permissions/{permissionName}")]
        public async Task<IActionResult> RemoveWorkflowStagePermission(Guid stageId, string permissionName)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid stage ID." });
            }

            if (string.IsNullOrWhiteSpace(permissionName))
            {
                return BadRequest(new { success = false, message = "Permission name is required." });
            }

            try
            {
                var command = new RemoveWorkflowStagePermissionCommand 
                { 
                    StageId = stageId, 
                    PermissionName = permissionName
                };

                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to remove permission from workflow stage." });
                }

                return Ok(new { success = true, message = "Permission removed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // Request DTOs
    public class AddPermissionRequest
    {
        public string PermissionName { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
    }
}
