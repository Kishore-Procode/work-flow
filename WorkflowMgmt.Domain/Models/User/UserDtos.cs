using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.WorkflowManagement
{
    // DTOs for API responses
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int RoleId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class UserWithDetailsDto : UserDto
    {
        public string? DepartmentName { get; set; }
        public string? RoleName { get; set; }
        public string? RoleCode { get; set; }
        public int? RoleHierarchyLevel { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Permissions { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class StageAssigneeDto
    {
        public Guid Id { get; set; }
        public Guid WorkflowStageId { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsDefault { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
    }

    // DTOs for API requests
    public class CreateUserDto
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

    public class UpdateUserDto
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
        public int DepartmentId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public bool? EmailVerified { get; set; }
    }

    public class CreateRoleDto
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

    public class UpdateRoleDto
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

    public class AssignUserToStageDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsDefault { get; set; } = false;
    }

    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int RecentLogins { get; set; }
        public UsersByRoleDto[] UsersByRole { get; set; } = Array.Empty<UsersByRoleDto>();
        public UsersByDepartmentDto[] UsersByDepartment { get; set; } = Array.Empty<UsersByDepartmentDto>();
    }

    public class UsersByRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class UsersByDepartmentDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
