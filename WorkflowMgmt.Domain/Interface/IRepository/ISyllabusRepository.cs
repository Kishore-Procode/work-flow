using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Syllabus;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ISyllabusRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<SyllabusWithDetailsDto>> GetAllAsync();
        Task<SyllabusWithDetailsDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateSyllabusDto syllabus);
        Task<bool> UpdateAsync(Guid id, UpdateSyllabusDto syllabus);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);

        // Query operations
        Task<IEnumerable<SyllabusWithDetailsDto>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<SyllabusWithDetailsDto>> GetByStatusAsync(string status);
        Task<IEnumerable<SyllabusWithDetailsDto>> GetByFacultyAsync(string facultyName);
        Task<IEnumerable<SyllabusWithDetailsDto>> GetByTemplateAsync(Guid templateId);
        Task<IEnumerable<SyllabusWithDetailsDto>> GetByCourseAsync(int courseId);
        Task<IEnumerable<SyllabusWithDetailsDto>> GetBySemesterAsync(int semesterId);
        Task<SyllabusWithDetailsDto?> GetByLessonPlanAsync(Guid lessonPlanId);

        // Workflow operations
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> UpdateWorkflowIdAsync(Guid id, Guid workflowId);

        // File operations
        Task<bool> UpdateDocumentUrlAsync(Guid id, string documentUrl);
        Task<bool> UpdateFileProcessingStatusAsync(Guid id, string status, string? notes = null);
        Task<SyllabusDocumentDto?> GetDocumentAsync(Guid id);

        // Validation operations
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByTitleAndDepartmentAsync(string title, int departmentId, Guid? excludeId = null);

        // Statistics
        Task<SyllabusStatsDto> GetStatsAsync();
    }
}
