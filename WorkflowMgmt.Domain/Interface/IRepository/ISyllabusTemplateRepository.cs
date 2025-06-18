using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.SyllabusTemplate;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ISyllabusTemplateRepository
    {
        Task<List<SyllabusTemplateDto>> GetAllSyllabusTemplates();
        Task<SyllabusTemplateDto?> GetSyllabusTemplateById(Guid id);
        Task<Guid> InsertSyllabusTemplate(SyllabusTemplateDto template);
        Task<bool> UpdateSyllabusTemplate(SyllabusTemplateDto template);
        Task<bool> DeleteOrRestoreSyllabusTemplate(Guid id, string modifiedBy, bool isRestore);
    }
}
