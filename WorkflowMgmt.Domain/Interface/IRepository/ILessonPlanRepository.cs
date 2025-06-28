using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.LessonPlan;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ILessonPlanRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<LessonPlanDto>> GetAllAsync();
        Task<LessonPlanWithDetailsDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateLessonPlanDto lessonPlan);
        Task<bool> UpdateAsync(Guid id, UpdateLessonPlanDto lessonPlan);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);

        // Query operations
        Task<IEnumerable<LessonPlanDto>> GetByStatusAsync(string status);
        Task<IEnumerable<LessonPlanDto>> GetByFacultyAsync(string facultyName);
        Task<IEnumerable<LessonPlanDto>> GetByTemplateAsync(Guid templateId);
        Task<IEnumerable<LessonPlanDto>> GetBySyllabusAsync(Guid syllabusId);

        // Validation operations
        Task<bool> ExistsByTitleAndTemplateAsync(string title, Guid templateId);

        // Stats operations
        Task<LessonPlanStatsDto> GetStatsAsync();

        // Template operations
        Task<IEnumerable<LessonPlanTemplateDto>> GetAllTemplatesAsync();
        Task<LessonPlanTemplateDto?> GetTemplateByIdAsync(Guid id);
        Task<IEnumerable<LessonPlanTemplateDto>> GetActiveTemplatesAsync();
        Task<IEnumerable<LessonPlanTemplateDto>> GetTemplatesByTypeAsync(string templateType);
        Task<Guid> CreateTemplateAsync(CreateLessonPlanTemplateDto template);
        Task<bool> UpdateTemplateAsync(Guid id, UpdateLessonPlanTemplateDto template);
        Task<bool> DeleteTemplateAsync(Guid id);
        Task<bool> ToggleTemplateActiveAsync(Guid id);
        Task<bool> TemplateExistsByNameAsync(string name, Guid? excludeId = null);
    }
}
