using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowTemplate;
using WorkflowMgmt.Domain.Models.Workflow;

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
            return Ok(new { success = true, data = result });
        }

        [HttpGet("by-document-type")]
        public async Task<IActionResult> GetWorkflowTemplatesByDocumentType([FromQuery] string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return BadRequest(new { success = false, message = "Document type is required." });
            }

            var query = new GetWorkflowTemplatesByDocumentTypeQuery { DocumentType = documentType };
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveWorkflowTemplates()
        {
            var query = new GetActiveWorkflowTemplatesQuery();
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflowTemplate(Guid id)
        {
            var query = new GetWorkflowTemplateByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Workflow template not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkflowTemplate([FromBody] CreateWorkflowTemplateCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetWorkflowTemplate), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkflowTemplate(Guid id, [FromBody] UpdateWorkflowTemplateCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Workflow template not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflowTemplate(Guid id)
        {
            var command = new DeleteWorkflowTemplateCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Workflow template not found." });
            }

            return Ok(new { success = true, message = "Workflow template deleted successfully." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleWorkflowTemplateActive(Guid id)
        {
            var command = new ToggleWorkflowTemplateActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Workflow template not found." });
            }

            return Ok(new { success = true, data = result });
        }
    }
}
