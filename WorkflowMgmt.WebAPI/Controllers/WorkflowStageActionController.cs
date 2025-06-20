using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowStageAction;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/workflow-stage-actions")]
    public class WorkflowStageActionController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkflowStageActionsByStageId([FromQuery] Guid workflowStageId)
        {
            if (workflowStageId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Workflow stage ID is required." });
            }

            var query = new GetWorkflowStageActionsByStageIdQuery { StageId = workflowStageId };
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflowStageAction(Guid id)
        {
            var query = new GetWorkflowStageActionByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Workflow stage action not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflowStageAction([FromBody] CreateWorkflowStageActionCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetWorkflowStageAction), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkflowStageAction(Guid id, [FromBody] UpdateWorkflowStageActionCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Workflow stage action not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflowStageAction(Guid id)
        {
            var command = new DeleteWorkflowStageActionCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Workflow stage action not found." });
            }

            return Ok(new { success = true, message = "Workflow stage action deleted successfully." });
        }
    }
}
