using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowDepartmentDocumentMappingRepository
    {
        Task<List<DocumentTypeWorkflowMappingDto>> GetDepartmentDocumentMappings(int departmentId);
        Task<WorkflowDepartmentDocumentMappingDto?> GetByDepartmentAndDocumentTypeAsync(int departmentId, string documentType);
        Task<bool> UpdateDepartmentDocumentMappings(int departmentId, List<DepartmentDocumentMappingAssignmentDto> mappings);
    }
}
