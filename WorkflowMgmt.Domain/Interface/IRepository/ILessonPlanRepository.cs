using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.LessonPlan;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ILessonPlanRepository
    {
        Task<List<LessonPlanDto>> GetAllLessonPlans();
        Task<LessonPlanDto?> GetLessonPlanById(Guid id);
        Task<Guid> InsertLessonPlan(LessonPlanDto lessonPlan);
        Task<bool> UpdateLessonPlan(LessonPlanDto lessonPlan);
        Task<bool> DeleteOrRestoreLessonPlan(Guid id, string modifiedBy, bool isRestore);
    }
}
