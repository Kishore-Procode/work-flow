using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Auth;
using WorkflowMgmt.Application.Features.Course;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Domain.Entities.Courses;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class CourseController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await Mediator.Send(new GetCourseCommand());
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var result = await Mediator.Send(new GetCourseByIdCommand(id));
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDTO course)
        {
            course.CreatedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new CreateCourseCommand(course));
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDTO course)
        {
            CourseDTO courseToUpdate = course;
            courseToUpdate.Id = id;
            courseToUpdate.ModifiedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateCourseCommand(courseToUpdate));
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteCourse(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "admin";
            var result = await Mediator.Send(new DeleteOrRestoreCourseCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreCourse(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "admin";
            var result = await Mediator.Send(new DeleteOrRestoreCourseCommand(id, modifiedBy, isRestore: true));
            return Ok(result);

        }

    }
}
