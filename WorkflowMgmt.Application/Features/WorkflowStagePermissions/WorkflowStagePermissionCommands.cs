using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStagePermissions
{
    // Queries
    public class GetWorkflowStagePermissionsQuery : IRequest<IEnumerable<WorkflowStagePermissionDto>>
    {
        [Required]
        public Guid WorkflowStageId { get; set; }
    }

    public class GetAvailablePermissionsQuery : IRequest<IEnumerable<AvailablePermissionDto>>
    {
    }

    // Commands
    public class UpdateWorkflowStagePermissionsCommand : IRequest<bool>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        public List<UpdatePermissionDto> Permissions { get; set; } = new List<UpdatePermissionDto>();
    }

    public class AddWorkflowStagePermissionCommand : IRequest<bool>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = false;
    }

    public class RemoveWorkflowStagePermissionCommand : IRequest<bool>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; } = string.Empty;
    }
}
