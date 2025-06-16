using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Auth;
using WorkflowMgmt.Application.Features.Course;
using WorkflowMgmt.Application.Features.Department;

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

    }
}
