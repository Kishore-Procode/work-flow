using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WorkflowMgmt.Application.Features.Session;
using WorkflowMgmt.Domain.Models.Session;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            try
            {
                var query = new GetSessionsQuery();
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(Guid id)
        {
            try
            {
                var query = new GetSessionByIdQuery(id);
                var result = await _mediator.Send(query);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Session not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetSessionsByStatus(string status)
        {
            try
            {
                var query = new GetSessionsByStatusQuery(status);
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("instructor/{instructor}")]
        public async Task<IActionResult> GetSessionsByInstructor(string instructor)
        {
            try
            {
                var query = new GetSessionsByInstructorQuery(instructor);
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetSessionsByDepartment(int departmentId)
        {
            try
            {
                var query = new GetSessionsByDepartmentQuery(departmentId);
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("lesson-plan/{lessonPlanId}")]
        public async Task<IActionResult> GetSessionsByLessonPlan(Guid lessonPlanId)
        {
            try
            {
                var query = new GetSessionsByLessonPlanQuery(lessonPlanId);
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetSessionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var query = new GetSessionsByDateRangeQuery(startDate, endDate);
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetSessionsStats()
        {
            try
            {
                var query = new GetSessionsStatsQuery();
                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromForm] CreateSessionCommand command)
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
                command.Instructor = !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)
                    ? $"{firstName} {lastName}"
                    : userName ?? "Unknown User";

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetSession), new { id = result.Id }, 
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating the session.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(Guid id, [FromForm] UpdateSessionCommand command)
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

                command.Id = id;
                command.FacultyId = Guid.Parse(userId);
                command.Instructor = !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)
                    ? $"{firstName} {lastName}"
                    : userName ?? "Unknown User";

                var result = await _mediator.Send(command);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the session.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(Guid id)
        {
            try
            {
                var command = new DeleteSessionCommand(id);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return NotFound(new { success = false, message = "Session not found." });
                }

                return Ok(new { success = true, message = "Session deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleSessionActive(Guid id)
        {
            try
            {
                var command = new ToggleSessionActiveCommand(id);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return NotFound(new { success = false, message = "Session not found." });
                }

                return Ok(new { success = true, message = "Session status toggled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
