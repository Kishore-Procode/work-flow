using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.DocumentLifecycle;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class DocumentLifecycleController : BaseApiController
    {

        [HttpGet("assigned")]
        public async Task<IActionResult> GetDocumentsAssignedToUser([FromQuery] string? documentType = null)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var query = new GetDocumentsAssignedToUserQuery
            {
                UserId = userId,
                DocumentType = documentType
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{documentId}/lifecycle")]
        public async Task<IActionResult> GetDocumentLifecycle(Guid documentId, [FromQuery] string documentType = "syllabus")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var query = new GetDocumentLifecycleQuery
            {
                DocumentId = documentId,
                DocumentType = documentType,
                UserId = userId
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{documentId}/actions")]
        public async Task<IActionResult> GetAvailableActions(Guid documentId, [FromQuery] string documentType = "syllabus")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var query = new GetAvailableActionsQuery
            {
                UserId = userId,
                DocumentId = documentId,
                DocumentType = documentType
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{documentId}/actions/{actionId}/can-perform")]
        public async Task<IActionResult> CanUserPerformAction(Guid documentId, Guid actionId, [FromQuery] string documentType = "syllabus")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var query = new CanUserPerformActionQuery
            {
                UserId = userId,
                DocumentId = documentId,
                DocumentType = documentType,
                ActionId = actionId
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{documentId}/actions/{actionId}/process")]
        public async Task<IActionResult> ProcessDocumentAction(Guid documentId, Guid actionId, [FromBody] ProcessDocumentActionRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new ProcessDocumentActionCommand
            {
                DocumentId = documentId,
                DocumentType = request.DocumentType,
                ActionId = actionId,
                ProcessedBy = userId,
                Comments = request.Comments,
                FeedbackType = request.FeedbackType
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{documentId}/feedback")]
        public async Task<IActionResult> GetDocumentFeedback(Guid documentId, [FromQuery] string documentType = "syllabus")
        {
            var query = new GetDocumentFeedbackQuery
            {
                DocumentId = documentId,
                DocumentType = documentType
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{documentId}/feedback")]
        public async Task<IActionResult> CreateDocumentFeedback(Guid documentId, [FromBody] CreateDocumentFeedbackRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new CreateDocumentFeedbackCommand
            {
                DocumentId = documentId,
                DocumentType = request.DocumentType,
                WorkflowStageId = request.WorkflowStageId,
                FeedbackProvider = userId,
                FeedbackText = request.FeedbackText,
                FeedbackType = request.FeedbackType
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("feedback/{feedbackId}")]
        public async Task<IActionResult> UpdateDocumentFeedback(Guid feedbackId, [FromBody] UpdateDocumentFeedbackRequest request)
        {
            var command = new UpdateDocumentFeedbackCommand
            {
                Id = feedbackId,
                FeedbackText = request.FeedbackText,
                FeedbackType = request.FeedbackType,
                IsAddressed = request.IsAddressed
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("feedback/{feedbackId}/mark-addressed")]
        public async Task<IActionResult> MarkFeedbackAsAddressed(Guid feedbackId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User not authenticated");
            }

            var command = new MarkFeedbackAsAddressedCommand
            {
                FeedbackId = feedbackId,
                AddressedBy = userId
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    // Request models
    public class ProcessDocumentActionRequest
    {
        public string DocumentType { get; set; } = "syllabus";
        public string? Comments { get; set; }
        public string? FeedbackType { get; set; } = "general";
    }

    public class CreateDocumentFeedbackRequest
    {
        public string DocumentType { get; set; } = "syllabus";
        public Guid? WorkflowStageId { get; set; }
        public string FeedbackText { get; set; } = string.Empty;
        public string FeedbackType { get; set; } = "general";
    }

    public class UpdateDocumentFeedbackRequest
    {
        public string? FeedbackText { get; set; }
        public string? FeedbackType { get; set; }
        public bool? IsAddressed { get; set; }
    }
}
