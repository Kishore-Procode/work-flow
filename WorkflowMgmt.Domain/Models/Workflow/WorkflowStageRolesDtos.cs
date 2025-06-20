using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.Workflow
{
    // Workflow Stage Role DTOs
    public class WorkflowStageRoleDto
    {
        public string RoleCode { get; set; } = string.Empty;
        public string? RoleName { get; set; }
        public bool IsRequired { get; set; }
    }

    public class CreateWorkflowStageRoleDto
    {
        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = true;
    }

    public class UpdateStageRolesRequest
    {
        [Required]
        public List<UpdateRoleDto> Roles { get; set; } = new List<UpdateRoleDto>();
    }

    public class UpdateRoleDto
    {
        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = true;
    }

    // Workflow Stage Permission DTOs
    public class WorkflowStagePermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
    }

    public class CreateWorkflowStagePermissionDto
    {
        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = false;
    }

    public class UpdateStagePermissionsRequest
    {
        [Required]
        public List<UpdatePermissionDto> Permissions { get; set; } = new List<UpdatePermissionDto>();
    }

    public class UpdatePermissionDto
    {
        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = false;
    }

    // Available Permissions DTO
    public class AvailablePermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    // Enhanced Stage Details DTO
    public class WorkflowStageDetailsDto
    {
        public Guid StageId { get; set; }
        public Guid WorkflowTemplateId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public string? AssignedRole { get; set; } // Keep for backward compatibility
        public string? Description { get; set; }
        public bool IsRequired { get; set; }
        public bool AutoApprove { get; set; }
        public int? TimeoutDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        public List<WorkflowStageRoleDto> RequiredRoles { get; set; } = new List<WorkflowStageRoleDto>();
        public List<WorkflowStagePermissionDto> StagePermissions { get; set; } = new List<WorkflowStagePermissionDto>();
        public List<WorkflowStageActionDto> Actions { get; set; } = new List<WorkflowStageActionDto>();
    }

    // Role DTO for dropdowns (reusing existing roles)
    public class RoleOptionDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
