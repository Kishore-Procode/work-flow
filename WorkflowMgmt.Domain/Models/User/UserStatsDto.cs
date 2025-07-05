using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Models.User
{
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
