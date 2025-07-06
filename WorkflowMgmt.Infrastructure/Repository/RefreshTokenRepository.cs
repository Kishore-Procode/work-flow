using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Auth;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class RefreshTokenRepository : RepositoryTranBase, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await Connection.QueryFirstOrDefaultAsync<RefreshToken>(
                "SELECT Id,token,user_id as UserId,expires_at as ExpiresAt,created_at as CreatedAt,is_revoked as IsRevoked," +
                "revoked_reason as RevokedReason, revoked_at as RevokedAt, replaced_by_token as ReplacedByToken " +
                "FROM workflowmgmt.refresh_tokens WHERE token = @Token AND is_revoked = false",
                new { Token = token },
                Transaction);
        }

        public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
        {
            return await Connection.QueryFirstOrDefaultAsync<RefreshToken>(
                "SELECT Id,token,user_id as UserId,expires_at as ExpiresAt,created_at as CreatedAt,is_revoked as IsRevoked," +
                "revoked_reason as RevokedReason, revoked_at as RevokedAt, replaced_by_token as ReplacedByToken " +
                "FROM workflowmgmt.refresh_tokens WHERE user_id = @UserId AND is_revoked = false AND expires_at > @Now ORDER BY created_at DESC",
                new { UserId = userId, Now = DateTime.Now },
                Transaction);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            var tokens = await Connection.QueryAsync<RefreshToken>(
                "SELECT Id,token,user_id as UserId,expires_at as ExpiresAt,created_at as CreatedAt,is_revoked as IsRevoked," +
                "revoked_reason as RevokedReason, revoked_at as RevokedAt, replaced_by_token as ReplacedByToken " +
                "FROM workflowmgmt.refresh_tokens WHERE user_id = @UserId AND is_revoked = false AND expires_at > @Now",
                new { UserId = userId, Now = DateTime.Now },
                Transaction);

            return tokens.ToList();
        }

        public async Task<Guid> CreateAsync(RefreshToken refreshToken)
        {
            var sql = @"
                INSERT INTO workflowmgmt.refresh_tokens 
                (id, token, user_id, expires_at, created_at, is_revoked) 
                VALUES 
                (@Id, @Token, @UserId, @ExpiresAt, @CreatedAt, @IsRevoked)
                RETURNING id";

            var id = await Connection.QuerySingleAsync<Guid>(sql, refreshToken, Transaction);
            return id;
        }

        public async Task<bool> UpdateAsync(RefreshToken refreshToken)
        {
            var sql = @"
                UPDATE workflowmgmt.refresh_tokens 
                SET token = @Token, expires_at = @ExpiresAt, is_revoked = @IsRevoked, 
                    revoked_reason = @RevokedReason, revoked_at = @RevokedAt, 
                    replaced_by_token = @ReplacedByToken
                WHERE id = @id";

            var rowsAffected = await Connection.ExecuteAsync(sql, refreshToken, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> RevokeTokenAsync(string token, string reason)
        {
            var sql = @"
                UPDATE workflowmgmt.refresh_tokens 
                SET is_revoked = true, revoked_reason = @Reason, revoked_at = @RevokedAt
                WHERE token = @Token";

            var rowsAffected = await Connection.ExecuteAsync(sql, 
                new { Token = token, Reason = reason, RevokedAt = DateTime.Now }, 
                Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> RevokeAllUserTokensAsync(Guid userId, string reason)
        {
            var sql = @"
                UPDATE workflowmgmt.refresh_tokens 
                SET is_revoked = true, revoked_reason = @Reason, revoked_at = @RevokedAt
                WHERE user_id = @UserId AND is_revoked = false";

            var rowsAffected = await Connection.ExecuteAsync(sql, 
                new { UserId = userId, Reason = reason, RevokedAt = DateTime.Now }, 
                Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteExpiredTokensAsync()
        {
            var sql = @"
                DELETE FROM workflowmgmt.refresh_tokens 
                WHERE expires_at < @Now OR (is_revoked = true AND revoked_at < @CutoffDate)";

            var cutoffDate = DateTime.Now.AddDays(-30); // Keep revoked tokens for 30 days for audit
            var rowsAffected = await Connection.ExecuteAsync(sql, 
                new { Now = DateTime.Now, CutoffDate = cutoffDate }, 
                Transaction);
            return rowsAffected > 0;
        }
    }
}
