using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.LessonPlan;
using WorkflowMgmt.Domain.Models.LessonPlan;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/lesson-plan-templates")]
    public class LessonPlanTemplatesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetLessonPlanTemplates(
            [FromQuery] string? template_type = null,
            [FromQuery] bool? active = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(template_type))
                {
                    var query = new GetLessonPlanTemplatesByTypeQuery(template_type);
                    var result = await Mediator.Send(query);
                    return Ok(new { success = true, data = result });
                }

                if (active.HasValue && active.Value)
                {
                    var query = new GetActiveLessonPlanTemplatesQuery();
                    var result = await Mediator.Send(query);
                    return Ok(new { success = true, data = result });
                }

                // Default: get all templates
                var allQuery = new GetLessonPlanTemplatesQuery();
                var allResult = await Mediator.Send(allQuery);
                return Ok(new { success = true, data = allResult });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving lesson plan templates", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonPlanTemplate(Guid id)
        {
            try
            {
                var query = new GetLessonPlanTemplateByIdQuery(id);
                var result = await Mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { success = false, message = "Lesson plan template not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the lesson plan template", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessonPlanTemplate([FromBody] CreateLessonPlanTemplateCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(new { success = true, data = result, message = "Lesson plan template created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating the lesson plan template", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessonPlanTemplate(Guid id, [FromBody] UpdateLessonPlanTemplateCommand command)
        {
            try
            {
                command.Id = id;
                var result = await Mediator.Send(command);
                return Ok(new { success = true, data = result, message = "Lesson plan template updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the lesson plan template", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessonPlanTemplate(Guid id)
        {
            try
            {
                var command = new DeleteLessonPlanTemplateCommand(id);
                var result = await Mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { success = false, message = "Lesson plan template not found" });
                }

                return Ok(new { success = true, message = "Lesson plan template deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the lesson plan template", error = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleLessonPlanTemplateActive(Guid id)
        {
            try
            {
                var command = new ToggleLessonPlanTemplateActiveCommand(id);
                var result = await Mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { success = false, message = "Lesson plan template not found" });
                }

                return Ok(new { success = true, message = "Lesson plan template status toggled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while toggling lesson plan template status", error = ex.Message });
            }
        }
    }
}
