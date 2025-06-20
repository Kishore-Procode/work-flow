using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Syllabus;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ISyllabusTemplateRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<SyllabusTemplateDto>> GetAllAsync();
        Task<SyllabusTemplateDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateSyllabusTemplateDto template);
        Task<bool> UpdateAsync(Guid id, UpdateSyllabusTemplateDto template);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);

        // Query operations
        Task<IEnumerable<SyllabusTemplateDto>> GetActiveAsync();
        Task<IEnumerable<SyllabusTemplateDto>> GetByTypeAsync(string templateType);

        // Validation operations
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
        Task<bool> ExistsByNameAndTypeAsync(string name, string templateType, Guid? excludeId = null);
    }
}
