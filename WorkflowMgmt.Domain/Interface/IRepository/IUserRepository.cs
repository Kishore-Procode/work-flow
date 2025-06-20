using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models.WorkflowManagement;

namespace WorkflowMgmt.Domain.IRepository
{
    public interface IWorkflowUserRepository
    {
        // New methods
        Task<IEnumerable<UserWithDetailsDto>> GetAllAsync();
        Task<IEnumerable<UserWithDetailsDto>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<UserWithDetailsDto>> GetByRoleAsync(int roleId);
        Task<IEnumerable<UserWithDetailsDto>> GetByRoleCodeAsync(string roleCode);
        Task<IEnumerable<UserWithDetailsDto>> GetActiveAsync();
        Task<UserWithDetailsDto?> GetByIdAsync(Guid id);
        Task<UserWithDetailsDto?> GetByUsernameAsync(string username);
        Task<UserWithDetailsDto?> GetByEmailAsync(string email);
        Task<Guid> CreateAsync(CreateUserDto user);
        Task<bool> UpdateAsync(Guid id, UpdateUserDto user);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null);
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);
        Task<bool> UpdatePasswordAsync(Guid id, string passwordHash);
        Task<UserStatsDto> GetUserStatsAsync();
        Task<IEnumerable<UserWithDetailsDto>> GetEligibleUsersForStageAsync(Guid stageId, int? departmentId = null);
        Task<UserWithDetailsDto?> GetDefaultAssigneeForStageAsync(Guid stageId, int departmentId);
    }

    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserByUserName(string userName);
        Task<User?> GetUserById(Guid userId);
        Task UpdateLastLoginAsync(Guid userId);
        Task<Role?> GetRoleByRoleId(int roleId);
        Task<DepartmentDTO?> GetDepartmentByDepartmentId(int? departmentId);
    }
}
