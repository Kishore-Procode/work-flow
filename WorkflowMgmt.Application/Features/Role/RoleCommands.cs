using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.WorkflowManagement;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowRole
{
    // Queries
    public class GetAllRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
    {
    }

    public class GetActiveRolesQuery : IRequest<ApiResponse<List<RoleDto>>>
    {
    }

    public class GetRoleByIdQuery : IRequest<ApiResponse<RoleDto?>>
    {
        public int Id { get; set; }
    }

    public class GetRoleByCodeQuery : IRequest<ApiResponse<RoleDto?>>
    {
        public string Code { get; set; } = string.Empty;
    }

    // Commands
    public class CreateRoleCommand : IRequest<ApiResponse<RoleDto>>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int HierarchyLevel { get; set; }

        public string? Permissions { get; set; }
    }

    public class UpdateRoleCommand : IRequest<ApiResponse<RoleDto?>>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int HierarchyLevel { get; set; }

        public string? Permissions { get; set; }
    }

    public class DeleteRoleCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; set; }
    }

    public class ToggleRoleActiveCommand : IRequest<ApiResponse<RoleDto?>>
    {
        public int Id { get; set; }
    }
}
