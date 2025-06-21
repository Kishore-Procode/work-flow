using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStageDetailsRepository
    {
        // Enhanced stage details with roles
        Task<List<WorkflowStageDetailsDto>> GetByWorkflowTemplateIdAsync(Guid workflowTemplateId);
        Task<WorkflowStageDetailsDto?> GetByStageIdAsync(Guid stageId);

        // Role options for dropdowns
        Task<List<RoleOptionDto>> GetActiveRolesAsync();
    }
}
