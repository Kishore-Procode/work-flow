using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.UserManagement
{
    public class UserManagementCommandHandler : IRequestHandler<GetUserManagementCommand, ApiResponse<List<UserDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserManagementCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<List<UserDTO>>> Handle(GetUserManagementCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.UserManagementRepository.GetAllUsers();

                return ApiResponse<List<UserDTO>>.SuccessResponse(users, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserDTO>>.ErrorResponse($"Error during fetching user management: {ex.Message}");
            }
        }
    }
}
