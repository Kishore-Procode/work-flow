using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.DocumentFeedback;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DocumentFeedbackController : BaseApiController
    {
        [HttpGet("by-documents")]
        public async Task<IActionResult> GetFeedbackByDocuments([FromQuery] List<Guid> documentIds, [FromQuery] string? documentType = null)
        {
            if (documentIds == null || !documentIds.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Document IDs are required."));
            }

            var query = new GetFeedbackByDocumentsQuery 
            { 
                DocumentIds = documentIds,
                DocumentType = documentType
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentFeedback(Guid documentId, [FromQuery] string documentType = "syllabus")
        {
            if (documentId == Guid.Empty)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Document ID is required."));
            }

            var query = new GetDocumentFeedbackQuery 
            { 
                DocumentId = documentId,
                DocumentType = documentType
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentFeedback([FromQuery] string? documentType = null, [FromQuery] int limit = 10)
        {
            var query = new GetRecentFeedbackQuery 
            { 
                DocumentType = documentType,
                Limit = limit
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
