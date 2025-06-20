using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStageRoles
{
    // Queries
    public class GetWorkflowStageRolesQuery : IRequest<IEnumerable<WorkflowStageRoleDto>>
    {
        [Required]
        public Guid WorkflowStageId { get; set; }
    }

    public class GetActiveRolesQuery : IRequest<IEnumerable<RoleOptionDto>>
    {
    }

    public class GetWorkflowStageDetailsQuery : IRequest<IEnumerable<WorkflowStageDetailsDto>>
    {
        [Required]
        public Guid WorkflowTemplateId { get; set; }
    }

    public class GetWorkflowStageDetailsByIdQuery : IRequest<WorkflowStageDetailsDto?>
    {
        [Required]
        public Guid StageId { get; set; }
    }

    // Commands
    public class UpdateWorkflowStageRolesCommand : IRequest<bool>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        public List<UpdateRoleDto> Roles { get; set; } = new List<UpdateRoleDto>();
    }

    public class AddWorkflowStageRoleCommand : IRequest<bool>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = true;
    }

    public class RemoveWorkflowStageRoleCommand : IRequest<bool>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;
    }
}
