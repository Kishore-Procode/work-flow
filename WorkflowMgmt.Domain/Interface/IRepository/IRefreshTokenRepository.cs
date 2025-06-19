using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Auth;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByUserIdAsync(Guid userId);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
        Task<Guid> CreateAsync(RefreshToken refreshToken);
        Task<bool> UpdateAsync(RefreshToken refreshToken);
        Task<bool> RevokeTokenAsync(string token, string reason);
        Task<bool> RevokeAllUserTokensAsync(Guid userId, string reason);
        Task<bool> DeleteExpiredTokensAsync();
    }
}
