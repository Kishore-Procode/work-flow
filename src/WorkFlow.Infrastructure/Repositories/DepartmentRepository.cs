using Microsoft.EntityFrameworkCore;
using WorkFlow.Core.Entities;
using WorkFlow.Core.Interfaces;
using WorkFlow.Infrastructure.Data;

namespace WorkFlow.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly WorkFlowDbContext _context;

        public DepartmentRepository(WorkFlowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetDepartmentsWithTemplatesAsync()
        {
            return await _context.Departments
                .Include(d => d.DefaultTemplate)
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<bool> UpdateDepartmentDefaultTemplateAsync(int departmentId, Guid templateId)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == departmentId && d.IsActive);

            if (department == null)
                return false;

            // Verify that the template exists and is active
            var templateExists = await _context.WorkflowTemplates
                .AnyAsync(t => t.Id == templateId && t.IsActive);

            if (!templateExists)
                return false;

            department.DefaultTemplateId = templateId;
            department.ModifiedDate = DateTime.UtcNow;
            department.ModifiedBy = "system"; // You can pass this as parameter if needed

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == id && d.IsActive);
        }
    }
}
