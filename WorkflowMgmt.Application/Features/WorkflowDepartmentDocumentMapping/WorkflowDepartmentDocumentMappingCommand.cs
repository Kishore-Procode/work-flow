using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowDepartmentDocumentMapping
{
    public record GetDepartmentDocumentMappingsCommand(int departmentId) : IRequest<ApiResponse<List<DocumentTypeWorkflowMappingDto>>>;
    public record UpdateDepartmentDocumentMappingsCommand(UpdateDepartmentDocumentMappingRequest request) : IRequest<ApiResponse<bool>>;
}
