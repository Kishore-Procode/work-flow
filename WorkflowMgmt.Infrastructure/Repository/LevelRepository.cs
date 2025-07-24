using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class LevelRepository : RepositoryTranBase, ILevelRepository
    {
        public LevelRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<LevelDTO>> GetAllLevels()
        {
            var levels = await Connection.QueryAsync<LevelDTO>(
                "SELECT * FROM workflowmgmt.levels WHERE is_active = true ORDER BY sort_order, name", Transaction);

            return levels.ToList();
        }

        public async Task<LevelDTO?> GetLevelById(int id)
        {
            var sql = "SELECT * FROM workflowmgmt.levels WHERE id = @Id";
            var level = await Connection.QueryFirstOrDefaultAsync<LevelDTO>(sql, new { Id = id }, Transaction);
            return level;
        }

        public async Task<int> InsertLevel(LevelDTO level)
        {
            var sql = @"
                INSERT INTO workflowmgmt.levels (
                    name,
                    code,
                    description,
                    sort_order,
                    is_active,
                    created_date,
                    created_by
                ) VALUES (
                    @name,
                    @code,
                    @description,
                    @sort_order,
                    @is_active,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            var levelId = await Connection.ExecuteScalarAsync<int>(sql, level, Transaction);
            return levelId;
        }

        public async Task<bool> UpdateLevel(LevelDTO level)
        {
            var sql = @"
                UPDATE workflowmgmt.levels SET
                name = @name,
                code = @code,
                description = @description,
                sort_order = @sort_order,
                is_active = @is_active,
                modified_date = NOW(),
                modified_by = @ModifiedBy
                WHERE id = @id;
           ";

            var rowsAffected = await Connection.ExecuteAsync(sql, level, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreLevel(int id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.levels
        SET is_active = @IsActive,
            modified_by = @ModifiedBy,
            modified_date = NOW()
        WHERE id = @Id AND is_active != @IsActive";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore
            }, Transaction);

            return result > 0;
        }
    }
}
