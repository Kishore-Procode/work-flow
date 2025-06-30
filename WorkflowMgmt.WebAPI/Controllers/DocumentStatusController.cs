using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.DocumentStatus;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class DocumentStatusController : BaseApiController
    {
        [HttpGet("user-documents")]
        public async Task<IActionResult> GetUserDocumentStatus([FromQuery] string? documentType = null)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new GetUserDocumentStatusCommand(userId, documentType);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{documentId}/detail")]
        public async Task<IActionResult> GetDocumentStatusDetail(Guid documentId, [FromQuery] string documentType = "syllabus")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new GetDocumentStatusDetailCommand(documentId, documentType, userId);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("workflow-roadmap/{workflowTemplateId}")]
        public async Task<IActionResult> GetWorkflowRoadmap(Guid workflowTemplateId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new GetWorkflowRoadmapCommand(workflowTemplateId);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("user-stats")]
        public async Task<IActionResult> GetUserDocumentStats()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new GetUserDocumentStatsCommand(userId);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}
