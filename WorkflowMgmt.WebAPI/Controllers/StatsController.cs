using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Stats;
using WorkflowMgmt.Application.Features.DocumentWorkflow;
using WorkflowMgmt.Application.Features.Syllabus;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class StatsController : BaseApiController
    {
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartmentStats()
        {
            var query = new GetDepartmentStatsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourseStats()
        {
            var query = new GetCourseStatsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesterStats()
        {
            var query = new GetSemesterStatsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("workflows")]
        public async Task<IActionResult> GetWorkflowStats()
        {
            var query = new GetWorkflowStatsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("syllabi")]
        public async Task<IActionResult> GetSyllabusStats()
        {
            var query = new GetSyllabusStatsQuery();
            var result = await Mediator.Send(query);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var query = new GetUserStatsQuery();
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving user statistics", error = ex.Message });
            }
        }
    }
}
