using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.LessonPlan;
using WorkflowMgmt.Domain.Entities.LessonPlan;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class LessonPlanController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllLessonPlans()
        {
            var result = await Mediator.Send(new GetLessonPlansCommand());
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonPlanById(Guid id)
        {
            var result = await Mediator.Send(new GetLessonPlanByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessonPlan([FromBody] LessonPlanDto lessonPlan)
        {
            lessonPlan.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateLessonPlanCommand(lessonPlan));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessonPlan(Guid id, [FromBody] LessonPlanDto lessonPlan)
        {
            LessonPlanDto plan = lessonPlan;
            plan.Id = id;
            plan.ModifiedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateLessonPlanCommand(plan));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteLessonPlan(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreLessonPlanCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreLessonPlan(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreLessonPlanCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }
    }
}
