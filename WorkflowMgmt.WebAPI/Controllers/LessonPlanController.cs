using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.LessonPlan;
using WorkflowMgmt.Domain.Models.LessonPlan;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/lesson-plans")]
    public class LessonPlanController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetLessonPlans(
            [FromQuery] string? status = null,
            [FromQuery] string? faculty_name = null,
            [FromQuery] Guid? template_id = null,
            [FromQuery] Guid? syllabus_id = null)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                var query = new GetLessonPlansByStatusQuery(status);
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (!string.IsNullOrWhiteSpace(faculty_name))
            {
                var query = new GetLessonPlansByFacultyQuery(faculty_name);
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (template_id.HasValue)
            {
                var query = new GetLessonPlansByTemplateQuery(template_id.Value);
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (syllabus_id.HasValue)
            {
                var query = new GetLessonPlansBySyllabusQuery(syllabus_id.Value);
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            // Default: get all lesson plans
            var allQuery = new GetLessonPlansQuery();
            var allResult = await Mediator.Send(allQuery);
            return Ok(new { success = true, data = allResult });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonPlan(Guid id)
        {
            var query = new GetLessonPlanByIdQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { success = false, message = "Lesson plan not found" });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessonPlan([FromForm] CreateLessonPlanCommand command)
        {
            try
            {
                // Set faculty information from logged-in user
                var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var firstName = User?.FindFirst("firstName")?.Value;
                var lastName = User?.FindFirst("lastName")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { success = false, message = "User not authenticated." });
                }

                command.FacultyId = Guid.Parse(userId);
                command.FacultyName = !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)
                    ? $"{firstName} {lastName}"
                    : userName ?? "Unknown User";

                var result = await Mediator.Send(command);
                return Ok(new { success = true, data = result, message = "Lesson plan created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating lesson plan: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "An error occurred while creating the lesson plan", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessonPlan(Guid id, [FromForm] UpdateLessonPlanCommand command)
        {
            command.Id = id;

            // Always set faculty information from logged-in user
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var firstName = User?.FindFirst("firstName")?.Value;
            var lastName = User?.FindFirst("lastName")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { success = false, message = "User not authenticated." });
            }

            command.FacultyId = Guid.Parse(userId);
            command.FacultyName = !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)
                ? $"{firstName} {lastName}"
                : userName ?? "Unknown User";
            try
            {
                var result = await Mediator.Send(command);
                return Ok(new { success = true, data = result, message = "Lesson plan updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the lesson plan", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLessonPlan(Guid id)
        {
            try
            {
                var command = new DeleteLessonPlanCommand(id);
                var result = await Mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { success = false, message = "Lesson plan not found" });
                }

                return Ok(new { success = true, message = "Lesson plan deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the lesson plan", error = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleLessonPlanActive(Guid id)
        {
            try
            {
                var command = new ToggleLessonPlanActiveCommand(id);
                var result = await Mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { success = false, message = "Lesson plan not found" });
                }

                return Ok(new { success = true, message = "Lesson plan status toggled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while toggling lesson plan status", error = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetLessonPlanStats()
        {
            try
            {
                var query = new GetLessonPlansStatsQuery();
                var result = await Mediator.Send(query);

                // Ensure we always return valid stats even if null
                if (result == null)
                {
                    result = new LessonPlanStatsDto
                    {
                        TotalLessonPlans = 0,
                        DraftLessonPlans = 0,
                        PublishedLessonPlans = 0,
                        UnderReviewLessonPlans = 0,
                        ApprovedLessonPlans = 0,
                        RejectedLessonPlans = 0,
                        AverageDuration = 0,
                        TotalSessions = 0,
                        TotalTemplates = 0,
                        ActiveTemplates = 0
                    };
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving lesson plan statistics", error = ex.Message });
            }
        }
    }
}
