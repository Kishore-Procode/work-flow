using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Stats;

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
    }
}
