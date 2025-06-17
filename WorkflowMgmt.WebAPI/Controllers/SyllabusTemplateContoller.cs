using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.SyllabusTemplate;
using WorkflowMgmt.Domain.Entities.SyllabusTemplate;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class SyllabusTemplateController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllSyllabusTemplates()
        {
            var result = await Mediator.Send(new GetSyllabusTemplatesCommand());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSyllabusTemplateById(Guid id)
        {
            var result = await Mediator.Send(new GetSyllabusTemplateByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSyllabusTemplate([FromBody] SyllabusTemplateDto template)
        {
            template.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateSyllabusTemplateCommand(template));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSyllabusTemplate(Guid id, [FromBody] SyllabusTemplateDto template)
        {
            template.Id = id;
            template.ModifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new UpdateSyllabusTemplateCommand(template));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteSyllabusTemplate(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "admin";
            var result = await Mediator.Send(new DeleteOrRestoreSyllabusTemplateCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreSyllabusTemplate(Guid id)
        {
            var modifiedBy = User?.Identity?.Name ?? "admin";
            var result = await Mediator.Send(new DeleteOrRestoreSyllabusTemplateCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }
    }
}
