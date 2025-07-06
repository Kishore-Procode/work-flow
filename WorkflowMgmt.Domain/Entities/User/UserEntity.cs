using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{

    public class User
    {
        public Guid id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public int role_id { get; set; }
        public int? department_id { get; set; }
        public string? phone { get; set; }
        public string? profile_image_url { get; set; }
        public DateTime? last_login { get; set; }
        public string full_name => $"{first_name} {last_name}".Trim();
        public string DisplayName => !string.IsNullOrEmpty(full_name) ? full_name : username;
    }

    public class UserDTO
    {
        public Guid id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public int role_id { get; set; }
        public int? department_id { get; set; }
        public string? phone { get; set; }
        public string? profile_image_url { get; set; }
        public DateTime? last_login { get; set; }
        public string full_name => $"{first_name} {last_name}".Trim();
        public string DisplayName => !string.IsNullOrEmpty(full_name) ? full_name : username;

        // Joined data from roles and departments tables
        public string? role_name { get; set; }
        public string? role_code { get; set; }
        public string? role_description { get; set; }
        public string? department_name { get; set; }
        public string? department_code { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? modified_date { get; set; }
        public string? created_by { get; set; }
        public string? modified_by { get; set; }
        public bool is_active { get; set; } = true;
        public int[]? allowed_departments { get; set; }
        public int[]? allowed_roles { get; set; }
    }

    public class UpdatePasswordRequest
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        public Guid id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? phone { get; set; }
    }

}
