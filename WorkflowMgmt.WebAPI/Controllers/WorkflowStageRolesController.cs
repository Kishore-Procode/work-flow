using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowStageRoles;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/workflow-stage-roles")]
    public class WorkflowStageRolesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkflowStageRoles([FromQuery] Guid workflow_stage_id)
        {
            if (workflow_stage_id == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "workflow_stage_id is required." });
            }

            var query = new GetWorkflowStageRolesQuery { WorkflowStageId = workflow_stage_id };
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("{stageId}")]
        public async Task<IActionResult> UpdateWorkflowStageRoles(Guid stageId, [FromBody] UpdateStageRolesRequest request)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid stage ID." });
            }

            try
            {
                var command = new UpdateWorkflowStageRolesCommand 
                { 
                    StageId = stageId, 
                    Roles = request.Roles 
                };

                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to update workflow stage roles." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{stageId}/roles")]
        public async Task<IActionResult> AddWorkflowStageRole(Guid stageId, [FromBody] AddRoleRequest request)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid stage ID." });
            }

            if (string.IsNullOrWhiteSpace(request.RoleCode))
            {
                return BadRequest(new { success = false, message = "Role code is required." });
            }

            try
            {
                var command = new AddWorkflowStageRoleCommand 
                { 
                    StageId = stageId, 
                    RoleCode = request.RoleCode,
                    IsRequired = request.IsRequired
                };

                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to add role to workflow stage." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{stageId}/roles/{roleCode}")]
        public async Task<IActionResult> RemoveWorkflowStageRole(Guid stageId, string roleCode)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid stage ID." });
            }

            if (string.IsNullOrWhiteSpace(roleCode))
            {
                return BadRequest(new { success = false, message = "Role code is required." });
            }

            try
            {
                var command = new RemoveWorkflowStageRoleCommand 
                { 
                    StageId = stageId, 
                    RoleCode = roleCode
                };

                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to remove role from workflow stage." });
                }

                return Ok(new { success = true, message = "Role removed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // Request DTOs
    public class AddRoleRequest
    {
        public string RoleCode { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true;
    }
}
