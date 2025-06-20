using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.WorkflowManagement
{
    public class WorkflowUser : BaseEntityGuid
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? ProfilePicture { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool EmailVerified { get; set; } = false;

        // Navigation properties
        public virtual WorkflowRole Role { get; set; } = null!;
    }
}
