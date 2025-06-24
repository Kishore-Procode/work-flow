using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Syllabus;
using WorkflowMgmt.Domain.Models.Syllabus;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/syllabi")]
    public class SyllabiController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetSyllabi(
            [FromQuery] int? department_id = null,
            [FromQuery] string? status = null,
            [FromQuery] string? faculty_name = null,
            [FromQuery] Guid? template_id = null)
        {
            if (department_id.HasValue)
            {
                var query = new GetSyllabiByDepartmentQuery { DepartmentId = department_id.Value };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var query = new GetSyllabiByStatusQuery { Status = status };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (!string.IsNullOrWhiteSpace(faculty_name))
            {
                var query = new GetSyllabiByFacultyQuery { FacultyName = faculty_name };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            if (template_id.HasValue)
            {
                var query = new GetSyllabiByTemplateQuery { TemplateId = template_id.Value };
                var result = await Mediator.Send(query);
                return Ok(new { success = true, data = result });
            }

            // Default: get all syllabi
            var allQuery = new GetAllSyllabiQuery();
            var allResult = await Mediator.Send(allQuery);
            return Ok(new { success = true, data = allResult });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSyllabus(Guid id)
        {
            var query = new GetSyllabusByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Syllabus not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSyllabus([FromForm] CreateSyllabusCommand command)
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
                return CreatedAtAction(nameof(GetSyllabus), new { id = result.Id },
                    new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSyllabus(Guid id, [FromBody] UpdateSyllabusCommand command)
        {
            command.Id = id;
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSyllabus(Guid id)
        {
            var command = new DeleteSyllabusCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Syllabus not found." });
            }

            return Ok(new { success = true, message = "Syllabus deleted successfully." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleSyllabusActive(Guid id)
        {
            var command = new ToggleSyllabusActiveCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { success = false, message = "Syllabus not found." });
            }

            return Ok(new { success = true, data = result });
        }

        // Workflow actions
        [HttpPost("{id}/submit-for-review")]
        public async Task<IActionResult> SubmitSyllabusForReview(Guid id)
        {
            var command = new SubmitSyllabusForReviewCommand { Id = id };
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveSyllabus(Guid id, [FromBody] SyllabusWorkflowActionDto? request = null)
        {
            var command = new ApproveSyllabusCommand 
            { 
                Id = id, 
                Comments = request?.Comments 
            };
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectSyllabus(Guid id, [FromBody] SyllabusWorkflowActionDto request)
        {
            if (string.IsNullOrWhiteSpace(request?.Reason))
            {
                return BadRequest(new { success = false, message = "Reason is required for rejection." });
            }

            var command = new RejectSyllabusCommand 
            { 
                Id = id, 
                Reason = request.Reason 
            };
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishSyllabus(Guid id)
        {
            var command = new PublishSyllabusCommand { Id = id };
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/reprocess-file")]
        public async Task<IActionResult> ReprocessSyllabusFile(Guid id)
        {
            var command = new ReprocessSyllabusFileCommand { Id = id };
            
            try
            {
                var result = await Mediator.Send(command);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Syllabus not found." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadSyllabusDocument(Guid id)
        {
            var query = new GetSyllabusDocumentQuery { Id = id };
            
            try
            {
                var result = await Mediator.Send(query);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Document not found." });
                }

                return File(result.Content, result.ContentType, result.FileName);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
