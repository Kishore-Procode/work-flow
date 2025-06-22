using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkflowMgmt.Application.Features.WorkflowDepartmentDocumentMapping;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    public class WorkflowDepartmentDocumentMappingController : BaseApiController
    {
        [HttpGet("department/{departmentId}/document-mappings")]
        public async Task<IActionResult> GetDepartmentDocumentMappings(int departmentId)
        {
            var result = await Mediator.Send(new GetDepartmentDocumentMappingsCommand(departmentId));
            return Ok(result);
        }

        [HttpPut("department-document-mappings")]
        public async Task<IActionResult> UpdateDepartmentDocumentMappings([FromBody] UpdateDepartmentDocumentMappingRequest request)
        {
            var result = await Mediator.Send(new UpdateDepartmentDocumentMappingsCommand(request));
            return Ok(result);
        }
    }
}
