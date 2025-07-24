using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ILevelRepository
    {
        Task<List<LevelDTO>> GetAllLevels();
        Task<LevelDTO?> GetLevelById(int id);
        Task<int> InsertLevel(LevelDTO level);
        Task<bool> UpdateLevel(LevelDTO level);
        Task<bool> DeleteOrRestoreLevel(int id, string modifiedBy, bool isRestore);
    }
}
