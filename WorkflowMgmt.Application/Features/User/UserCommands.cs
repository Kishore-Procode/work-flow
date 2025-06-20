using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.WorkflowManagement;

namespace WorkflowMgmt.Application.Features.WorkflowUser
{
    // Queries
    public class GetAllUsersQuery : IRequest<IEnumerable<UserWithDetailsDto>>
    {
    }

    public class GetUsersByDepartmentQuery : IRequest<IEnumerable<UserWithDetailsDto>>
    {
        public int DepartmentId { get; set; }
    }

    public class GetUsersByRoleQuery : IRequest<IEnumerable<UserWithDetailsDto>>
    {
        public int RoleId { get; set; }
    }

    public class GetUsersByRoleCodeQuery : IRequest<IEnumerable<UserWithDetailsDto>>
    {
        public string RoleCode { get; set; } = string.Empty;
    }

    public class GetActiveUsersQuery : IRequest<IEnumerable<UserWithDetailsDto>>
    {
    }

    public class GetUserByIdQuery : IRequest<UserWithDetailsDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByUsernameQuery : IRequest<UserWithDetailsDto?>
    {
        public string Username { get; set; } = string.Empty;
    }

    public class GetUserByEmailQuery : IRequest<UserWithDetailsDto?>
    {
        public string Email { get; set; } = string.Empty;
    }

    public class GetUserStatsQuery : IRequest<UserStatsDto>
    {
    }

    public class GetEligibleUsersForStageQuery : IRequest<IEnumerable<UserWithDetailsDto>>
    {
        public Guid StageId { get; set; }
        public int? DepartmentId { get; set; }
    }

    // Commands
    public class CreateUserCommand : IRequest<UserDto>
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }

    public class UpdateUserCommand : IRequest<UserDto?>
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public bool? EmailVerified { get; set; }
    }

    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ToggleUserActiveCommand : IRequest<UserDto?>
    {
        public Guid Id { get; set; }
    }

    public class ChangeUserPasswordCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class AssignUserToStageCommand : IRequest<StageAssigneeDto>
    {
        public Guid StageId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsDefault { get; set; } = false;
    }

    public class RemoveUserFromStageCommand : IRequest<bool>
    {
        public Guid StageId { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetStageAssigneesQuery : IRequest<IEnumerable<StageAssigneeDto>>
    {
        public Guid StageId { get; set; }
    }

    public class GetUserStageAssignmentsQuery : IRequest<IEnumerable<StageAssigneeDto>>
    {
        public Guid UserId { get; set; }
    }
}
