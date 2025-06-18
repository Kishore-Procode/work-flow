using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.LessonPlanTemplate;
using WorkflowMgmt.Domain.Entities.LessonPlanTemplate;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class LessonPlanTemplateController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllLessonPlanTemplates()
        {
            var result = await Mediator.Send(new GetLessonPlanTemplatesCommand());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonPlanTemplateById(Guid id)
        {
            var result = await Mediator.Send(new GetLessonPlanTemplateByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessonPlanTemplate([FromBody] LessonPlanTemplateDto template)
        {
            template.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateLessonPlanTemplateCommand(template));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLessonPlanTemplate(Guid id, [FromBody] LessonPlanTemplateDto template)
        {
            template.Id = id;
            template.ModifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new UpdateLessonPlanTemplateCommand(template));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteLessonPlanTemplate(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreLessonPlanTemplateCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreLessonPlanTemplate(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreLessonPlanTemplateCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }
    }
}
