using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.SyllabusTemplate;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/syllabus-templates")]
    public class SyllabusTemplatesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetSyllabusTemplates(
            [FromQuery] string? template_type = null,
            [FromQuery] bool? is_active = null)
        {
            if (!string.IsNullOrWhiteSpace(template_type))
            {
                var query = new GetSyllabusTemplatesByTypeQuery { TemplateType = template_type };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (is_active.HasValue && is_active.Value)
            {
                var query = new GetActiveSyllabusTemplatesQuery();
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            // Default: get all templates
            var allQuery = new GetAllSyllabusTemplatesQuery();
            var allResult = await Mediator.Send(allQuery);
            return Ok(new { success = true, data = allResult });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSyllabusTemplate(Guid id)
        {
            var query = new GetSyllabusTemplateByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Syllabus template not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSyllabusTemplate([FromBody] CreateSyllabusTemplateCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetSyllabusTemplate), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSyllabusTemplate(Guid id, [FromBody] UpdateSyllabusTemplateCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus template not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSyllabusTemplate(Guid id)
        {
            var command = new DeleteSyllabusTemplateCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Syllabus template not found." });
            }

            return Ok(new { success = true, message = "Syllabus template deleted successfully." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleSyllabusTemplateActive(Guid id)
        {
            var command = new ToggleSyllabusTemplateActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Syllabus template not found." });
            }

            return Ok(new { success = true, data = result });
        }
    }
}
