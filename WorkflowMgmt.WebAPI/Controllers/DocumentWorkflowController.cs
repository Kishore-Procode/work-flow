using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.DocumentWorkflow;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/document-workflows")]
    public class DocumentWorkflowController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetDocumentWorkflows(
            [FromQuery] string? documentType = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? initiatedBy = null,
            [FromQuery] string? role = null)
        {
            if (!string.IsNullOrWhiteSpace(documentType))
            {
                var query = new GetDocumentWorkflowsByDocumentTypeQuery { DocumentType = documentType };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var query = new GetDocumentWorkflowsByStatusQuery { Status = status };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (initiatedBy.HasValue)
            {
                var query = new GetDocumentWorkflowsByInitiatedByQuery { UserId = initiatedBy.Value };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var query = new GetDocumentWorkflowsByRoleQuery { Role = role };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            // Default: get all workflows
            var allQuery = new GetAllDocumentWorkflowsQuery();
            var allResult = await Mediator.Send(allQuery);
            return Ok(new { success = true, data = allResult });
        }

        [HttpGet("by-document-id")]
        public async Task<IActionResult> GetDocumentWorkflowByDocumentId([FromQuery] string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return BadRequest(new { success = false, message = "Document ID is required." });
            }

            var query = new GetDocumentWorkflowByDocumentIdQuery { DocumentId = documentId };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Document workflow not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentWorkflow(Guid id)
        {
            var query = new GetDocumentWorkflowByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Document workflow not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocumentWorkflow([FromBody] CreateDocumentWorkflowCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetDocumentWorkflow), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocumentWorkflow(Guid id, [FromBody] UpdateDocumentWorkflowCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Document workflow not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/advance")]
        public async Task<IActionResult> AdvanceDocumentWorkflow(Guid id, [FromBody] AdvanceDocumentWorkflowCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Document workflow not found or cannot be advanced." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteDocumentWorkflow(Guid id)
        {
            var command = new CompleteDocumentWorkflowCommand { Id = id };
            
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Document workflow not found or cannot be completed." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelDocumentWorkflow(Guid id, [FromBody] CancelDocumentWorkflowCommand command)
        {
            command.Id = id;
            
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Document workflow not found or cannot be cancelled." });
            }

            return Ok(new { success = true, data = result });
        }
    }
}
