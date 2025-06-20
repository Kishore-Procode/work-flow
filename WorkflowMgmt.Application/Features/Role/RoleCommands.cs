using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.WorkflowManagement;

namespace WorkflowMgmt.Application.Features.WorkflowRole
{
    // Queries
    public class GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
    }

    public class GetActiveRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
    }

    public class GetRoleByIdQuery : IRequest<RoleDto?>
    {
        public int Id { get; set; }
    }

    public class GetRoleByCodeQuery : IRequest<RoleDto?>
    {
        public string Code { get; set; } = string.Empty;
    }

    // Commands
    public class CreateRoleCommand : IRequest<RoleDto>
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

    public class UpdateRoleCommand : IRequest<RoleDto?>
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

    public class DeleteRoleCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class ToggleRoleActiveCommand : IRequest<RoleDto?>
    {
        public int Id { get; set; }
    }
}
