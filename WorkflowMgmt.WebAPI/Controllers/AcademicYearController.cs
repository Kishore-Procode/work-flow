using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.AcademicYear;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class AcademicYearController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAcademicYears()
        {
            var result = await Mediator.Send(new GetAcademicYearCommand());
            return Ok(result);
        }

        [HttpGet("by-level/{levelId}")]
        public async Task<IActionResult> GetAcademicYearsByLevel(int levelId)
        {
            var result = await Mediator.Send(new GetAcademicYearsByLevelCommand(levelId));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAcademicYearById(int id)
        {
            var result = await Mediator.Send(new GetAcademicYearByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAcademicYear([FromBody] AcademicYearDTO academicYear)
        {
            academicYear.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateAcademicYearCommand(academicYear));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAcademicYear(int id, [FromBody] AcademicYearDTO academicYear)
        {
            AcademicYearDTO ay = academicYear;
            ay.Id = id;
            ay.ModifiedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateAcademicYearCommand(ay));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAcademicYear(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreAcademicYearCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreAcademicYear(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreAcademicYearCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }
    }
}
