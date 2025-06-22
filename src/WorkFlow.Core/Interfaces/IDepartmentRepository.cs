using WorkFlow.Core.Entities;

namespace WorkFlow.Core.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetDepartmentsWithTemplatesAsync();
        Task<bool> UpdateDepartmentDefaultTemplateAsync(int departmentId, Guid templateId);
        Task<bool> DepartmentExistsAsync(int id);
    }
}
