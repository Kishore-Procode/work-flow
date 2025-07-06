using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IUserManagementRepository
    {
        Task<List<UserDTO>> GetAllUsers();

        Task<UserDTO?> GetUserManagementById(Guid id);

        Task<Guid> InsertUser(UserDTO user);

        Task<bool> UpdateUser(UserDTO user);

        Task<bool> DeleteOrRestoreUser(Guid id, string modifiedBy, bool isRestore);

        Task<bool> UpdatePassword(UpdatePasswordRequest updateuser);

        Task<bool> UpdateProfile(UpdateProfileRequest profile);
    }
}
