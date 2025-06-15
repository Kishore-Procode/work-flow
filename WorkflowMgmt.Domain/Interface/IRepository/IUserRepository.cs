using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.IRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserByUserName(string userName);
        Task UpdateLastLoginAsync(Guid userId);
        Task<Role?> GetRoleByRoleId(int roleId);
        Task<DepartmentDTO?> GetDepartmentByDepartmentId(int? departmentId);
    }
}
