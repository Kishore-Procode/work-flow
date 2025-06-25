using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WorkflowMgmt.Application.Features.WorkflowAction;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkflowActionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkflowActionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Process a workflow action (approve, reject, etc.)
        /// </summary>
        /// <param name="command">The workflow action command</param>
        /// <returns>Updated document workflow</returns>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessWorkflowAction([FromBody] ProcessWorkflowActionCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound("Document workflow not found");
                }
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while processing the workflow action", details = ex.Message });
            }
        }

        /// <summary>
        /// Get available actions for a document workflow
        /// </summary>
        /// <param name="documentWorkflowId">The document workflow ID</param>
        /// <returns>List of available workflow actions</returns>
        [HttpGet("actions/{documentWorkflowId}")]
        public async Task<IActionResult> GetWorkflowActions(Guid documentWorkflowId)
        {
            try
            {
                var query = new GetWorkflowActionsQuery { DocumentWorkflowId = documentWorkflowId };
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving workflow actions", details = ex.Message });
            }
        }
    }
}
