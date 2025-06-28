using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Session;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ISessionRepository
    {
        Task<IEnumerable<SessionDto>> GetAllAsync();
        Task<SessionWithDetailsDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<SessionDto>> GetByStatusAsync(string status);
        Task<IEnumerable<SessionDto>> GetByInstructorAsync(string instructor);
        Task<IEnumerable<SessionDto>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<SessionDto>> GetByLessonPlanAsync(Guid lessonPlanId);
        Task<IEnumerable<SessionDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<SessionStatsDto> GetStatsAsync();
        Task<Guid> CreateAsync(CreateSessionDto session);
        Task<bool> UpdateAsync(Guid id, UpdateSessionDto session);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByTitleAndDepartmentAsync(string title, int departmentId);
        Task<bool> UpdateDocumentUrlAsync(Guid id, string documentUrl);
        Task<bool> UpdateStatusAsync(Guid id, string status);
    }
}
