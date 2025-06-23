using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{
    public class UserDto
    {
        public Guid id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public int role_id { get; set; }
        public int? department_id { get; set; }
        public string? phone { get; set; }
        public string? profile_image_url { get; set; }
        public int[]? allowed_departments { get; set; }
        public int[]? allowed_roles { get; set; }
        
        // Joined data
        public string? role_name { get; set; }
        public string? role_code { get; set; }
        public string? department_name { get; set; }
        public string? department_code { get; set; }
        
        // Computed properties
        public string full_name => $"{first_name} {last_name}".Trim();
    }

    public class UserFilterDto
    {
        public int? department_id { get; set; }
        public int? role_id { get; set; }
        public bool? is_active { get; set; } = true;
    }
}
