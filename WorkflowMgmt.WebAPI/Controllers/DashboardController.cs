using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkflowMgmt.Application.Features.Dashboard;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class DashboardController : BaseApiController
    {

        /// <summary>
        /// Get dashboard data based on user role
        /// </summary>
        /// <returns>Role-specific dashboard data including stats, activities, deadlines, and workflow status</returns>
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var query = new GetDashboardDataQuery(userId);
            var result = await Mediator.Send(query);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
