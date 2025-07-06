using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Auth;
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

    public class GetUserManagementByIdCommandHandler : IRequestHandler<GetUserManagementById, ApiResponse<UserDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserManagementByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserDTO>> Handle(GetUserManagementById request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserManagementRepository.GetUserManagementById(request.id);

                if (user == null)
                    return ApiResponse<UserDTO>.ErrorResponse($"User with ID {request.id} not found");

                return ApiResponse<UserDTO>.SuccessResponse(user, "User detail retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDTO>.ErrorResponse($"Error fetching user details: {ex.Message}");
            }
        }
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
             try
             {
                 var userId = await _unitOfWork.UserManagementRepository.InsertUser(request.User);
                 if (userId != Guid.Empty)
                        _unitOfWork.Commit();
                 return ApiResponse<Guid>.SuccessResponse(userId, "User created successfully");
             }
             catch (Exception ex)
             {
                 return ApiResponse<Guid>.ErrorResponse($"Error creating user: {ex.Message}");
             }
        }
    }
        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<bool>>
         {
                private readonly IUnitOfWork _unitOfWork;

                public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
                {
                    _unitOfWork = unitOfWork;
                }

                public async Task<ApiResponse<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
                {
                    try
                    {
                        var success = await _unitOfWork.UserManagementRepository.UpdateUser(request.user);
                        if (success)
                            _unitOfWork.Commit();
                        return ApiResponse<bool>.SuccessResponse(success, "User updated successfully");
                    }
                    catch (Exception ex)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Error updating user: {ex.Message}");
                    }
                }
        }

        public class DeleteOrRestoreUserCommandHandler : IRequestHandler<DeleteOrRestoreUserCommand, ApiResponse<bool>>
        {
                private readonly IUnitOfWork _unitOfWork;

                public DeleteOrRestoreUserCommandHandler(IUnitOfWork unitOfWork)
                {
                    _unitOfWork = unitOfWork;
                }

                public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreUserCommand request, CancellationToken cancellationToken)
                {
                    try
                    {
                        var updated = await _unitOfWork.UserManagementRepository
                            .DeleteOrRestoreUser(request.id, request.modifiedBy, request.isRestore);
                        if (updated)
                            _unitOfWork.Commit();
                        return ApiResponse<bool>.SuccessResponse(updated, "User restored successfully");
                    }
                    catch (Exception ex)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Operation failed: {ex.Message}");
                    }
                }
            
        }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.UserManagementRepository.UpdatePassword(request.user);
                if (success)
                    _unitOfWork.Commit();
                return ApiResponse<bool>.SuccessResponse(success, "Password updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating password: {ex.Message}");
            }
        }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the profile
                var success = await _unitOfWork.UserManagementRepository.UpdateProfile(request.profile);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update profile");
                }

                _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(true, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<bool>.ErrorResponse($"Error updating profile: {ex.Message}");
            }
        }
    }

}
