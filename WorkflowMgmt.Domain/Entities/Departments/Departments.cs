using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{   
    public class DepartmentDTO : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string? description { get; set; }
        public string? head_of_department { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int? established_year { get; set; }
        public string? programs_offered { get; set; }
        public string? accreditation { get; set; }
        public string status { get; set; } = "Active";
        public Guid? default_template_id { get; set; }
        public bool email_notify { get; set; } = false;
        public bool sms_notify { get; set; } = false;
        public bool in_app_notify { get; set; } = false;
        public string? digest_frequency { get; set; }
    }

    public class DepartmentWithTemplateDTO
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public Guid? default_template_id { get; set; }
        public string? default_template_name { get; set; }
    }

    public class DepartmentStatsDto
    {
        public int TotalDepartments { get; set; }
        public int ActiveDepartments { get; set; }
        public int TotalPrograms { get; set; }
        public int NbaAccredited { get; set; }
    }

    public class WorkflowRoleMappingDto : BaseEntity
    {
        public int department_id { get; set; }
        public int role_id { get; set; }
        public Guid user_id { get; set; }
        public bool isprimary { get; set; }

        // Joined data
        public string? department_name { get; set; }
        public string? role_name { get; set; }
        public string? role_code { get; set; }
        public string? user_name { get; set; }
        public string? user_email { get; set; }
    }

    public class DepartmentRoleUserDto
    {
        public int department_id { get; set; }
        public string department_name { get; set; } = string.Empty;
        public int role_id { get; set; }
        public string role_name { get; set; } = string.Empty;
        public string role_code { get; set; } = string.Empty;
        public Guid user_id { get; set; }
        public string user_name { get; set; } = string.Empty;
        public string user_email { get; set; } = string.Empty;
        public bool is_primary { get; set; }
    }

    public class DepartmentRoleUserAssignmentDto
    {
        public Guid user_id { get; set; }
        public int role_id { get; set; }
        public bool is_primary { get; set; }
    }

    public class UpdateDepartmentRoleUsersRequest
    {
        public int department_id { get; set; }
        public List<DepartmentRoleUserAssignmentDto> assignments { get; set; } = new List<DepartmentRoleUserAssignmentDto>();
    }
}
