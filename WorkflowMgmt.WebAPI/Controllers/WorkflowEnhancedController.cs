using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowStageRoles;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/workflow")]
    public class WorkflowEnhancedController : BaseApiController
    {

        [HttpGet("active-roles")]
        public async Task<IActionResult> GetActiveRoles()
        {
            var query = new GetActiveRolesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("workflow-stage-details")]
        public async Task<IActionResult> GetWorkflowStageDetails([FromQuery] Guid workflow_template_id)
        {
            if (workflow_template_id == Guid.Empty)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("workflow_template_id is required."));
            }

            var query = new GetWorkflowStageDetailsQuery { WorkflowTemplateId = workflow_template_id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("workflow-stage-details/{stageId}")]
        public async Task<IActionResult> GetWorkflowStageDetailsById(Guid stageId)
        {
            if (stageId == Guid.Empty)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid stage ID."));
            }

            var query = new GetWorkflowStageDetailsByIdQuery { StageId = stageId };
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
