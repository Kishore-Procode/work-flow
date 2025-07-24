using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.Level;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class LevelController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllLevels()
        {
            var result = await Mediator.Send(new GetLevelCommand());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLevelById(int id)
        {
            var result = await Mediator.Send(new GetLevelByIdCommand(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLevel([FromBody] LevelDTO level)
        {
            level.CreatedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new CreateLevelCommand(level));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLevel(int id, [FromBody] LevelDTO level)
        {
            LevelDTO lvl = level;
            lvl.Id = id;
            lvl.ModifiedBy = User?.Identity?.Name ?? "unknown";

            var result = await Mediator.Send(new UpdateLevelCommand(lvl));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteLevel(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreLevelCommand(id, modifiedBy, isRestore: false));
            return Ok(result);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreLevel(int id)
        {
            var modifiedBy = User?.Identity?.Name ?? "unknown";
            var result = await Mediator.Send(new DeleteOrRestoreLevelCommand(id, modifiedBy, isRestore: true));
            return Ok(result);
        }
    }
}
