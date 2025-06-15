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

}
