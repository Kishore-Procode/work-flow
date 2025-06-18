using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Semester;
using WorkflowMgmt.Domain.Entities.Semesters;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class SemesterController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllSemester()
        {
            var result = await Mediator.Send(new GetSemesterCommand());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemesterById(int id)
        {
            var result = await Mediator.Send(new GetSemesterByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterDTO semester)
        {
            semester.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateSemesterCommand(semester));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterDTO semester)
        {
            SemesterDTO sem = semester;
            sem.Id = id;
            sem.ModifiedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateSemesterCommand(sem));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteSemester(int id)
        {

            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreSemesterCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreSemester(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreSemesterCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }
    }
}
