using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.LessonPlanTemplate;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ILessonPlanTemplateRepository
    {
        Task<List<LessonPlanTemplateDto>> GetAllLessonPlanTemplates();
        Task<LessonPlanTemplateDto?> GetLessonPlanTemplateById(Guid id);
        Task<Guid> InsertLessonPlanTemplate(LessonPlanTemplateDto template);
        Task<bool> UpdateLessonPlanTemplate(LessonPlanTemplateDto template);
        Task<bool> DeleteOrRestoreLessonPlanTemplate(Guid id, string modifiedBy, bool isRestore);
    }
}
