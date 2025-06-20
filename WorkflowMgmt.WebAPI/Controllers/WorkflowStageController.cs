using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowStage;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/workflow-stages")]
    public class WorkflowStageController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkflowStagesByTemplateId([FromQuery] Guid workflowTemplateId)
        {
            if (workflowTemplateId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Workflow template ID is required." });
            }

            var query = new GetWorkflowStagesByTemplateIdQuery { TemplateId = workflowTemplateId };
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("by-role")]
        public async Task<IActionResult> GetWorkflowStagesByRole([FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(new { success = false, message = "Role is required." });
            }

            var query = new GetWorkflowStagesByRoleQuery { Role = role };
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflowStage(Guid id)
        {
            var query = new GetWorkflowStageByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Workflow stage not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflowStage([FromBody] CreateWorkflowStageCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetWorkflowStage), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkflowStage(Guid id, [FromBody] UpdateWorkflowStageCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Workflow stage not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflowStage(Guid id)
        {
            var command = new DeleteWorkflowStageCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Workflow stage not found." });
            }

            return Ok(new { success = true, message = "Workflow stage deleted successfully." });
        }

        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderWorkflowStages([FromBody] ReorderWorkflowStagesCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to reorder workflow stages." });
                }

                return Ok(new { success = true, message = "Workflow stages reordered successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
