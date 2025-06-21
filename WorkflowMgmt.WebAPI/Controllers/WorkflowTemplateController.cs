using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowTemplate;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/workflow-templates")]
    public class WorkflowTemplateController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllWorkflowTemplates()
        {
            var query = new GetAllWorkflowTemplatesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("by-document-type")]
        public async Task<IActionResult> GetWorkflowTemplatesByDocumentType([FromQuery] string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Document type is required."));
            }

            var query = new GetWorkflowTemplatesByDocumentTypeQuery { DocumentType = documentType };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveWorkflowTemplates()
        {
            var query = new GetActiveWorkflowTemplatesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflowTemplate(Guid id)
        {
            var query = new GetWorkflowTemplateByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflowTemplate([FromBody] CreateWorkflowTemplateCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkflowTemplate(Guid id, [FromBody] UpdateWorkflowTemplateCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflowTemplate(Guid id)
        {
            var command = new DeleteWorkflowTemplateCommand { Id = id };
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleWorkflowTemplateActive(Guid id)
        {
            var command = new ToggleWorkflowTemplateActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
