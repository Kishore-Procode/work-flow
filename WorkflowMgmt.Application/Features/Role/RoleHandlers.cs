using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.WorkflowManagement;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowRole
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ApiResponse<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _unitOfWork.WorkflowRoleRepository.GetAllAsync();
                return ApiResponse<List<RoleDto>>.SuccessResponse(roles, "Roles retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<RoleDto>>.ErrorResponse($"Error retrieving roles: {ex.Message}");
            }
        }
    }

    public class GetActiveRolesQueryHandler : IRequestHandler<GetActiveRolesQuery, ApiResponse<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<RoleDto>>> Handle(GetActiveRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _unitOfWork.WorkflowRoleRepository.GetActiveAsync();
                return ApiResponse<List<RoleDto>>.SuccessResponse(roles, "Active roles retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<RoleDto>>.ErrorResponse($"Error retrieving active roles: {ex.Message}");
            }
        }
    }

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto?>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.WorkflowRoleRepository.GetByIdAsync(request.Id);
                if (role == null)
                {
                    return ApiResponse<RoleDto?>.ErrorResponse("Role not found");
                }
                return ApiResponse<RoleDto?>.SuccessResponse(role, "Role retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto?>.ErrorResponse($"Error retrieving role: {ex.Message}");
            }
        }
    }

    public class GetRoleByCodeQueryHandler : IRequestHandler<GetRoleByCodeQuery, ApiResponse<RoleDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoleByCodeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto?>> Handle(GetRoleByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _unitOfWork.WorkflowRoleRepository.GetByCodeAsync(request.Code);
                if (role == null)
                {
                    return ApiResponse<RoleDto?>.ErrorResponse("Role not found");
                }
                return ApiResponse<RoleDto?>.SuccessResponse(role, "Role retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto?>.ErrorResponse($"Error retrieving role: {ex.Message}");
            }
        }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if role code already exists
                var existingRole = await _unitOfWork.WorkflowRoleRepository.GetByCodeAsync(request.Code);
                if (existingRole != null)
                {
                    return ApiResponse<RoleDto>.ErrorResponse("Role with this code already exists");
                }

                var createDto = new CreateRoleDto
                {
                    Name = request.Name,
                    Code = request.Code,
                    Description = request.Description,
                    HierarchyLevel = request.HierarchyLevel,
                    Permissions = request.Permissions
                };

                var roleId = await _unitOfWork.WorkflowRoleRepository.CreateAsync(createDto);
                await _unitOfWork.SaveAsync();

                var createdRole = await _unitOfWork.WorkflowRoleRepository.GetByIdAsync(roleId);
                return ApiResponse<RoleDto>.SuccessResponse(createdRole!, "Role created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto>.ErrorResponse($"Error creating role: {ex.Message}");
            }
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto?>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if role exists
                var existingRole = await _unitOfWork.WorkflowRoleRepository.GetByIdAsync(request.Id);
                if (existingRole == null)
                {
                    return ApiResponse<RoleDto?>.ErrorResponse("Role not found");
                }

                // Check if code is being changed and if new code already exists
                if (existingRole.Code != request.Code)
                {
                    var roleWithCode = await _unitOfWork.WorkflowRoleRepository.GetByCodeAsync(request.Code);
                    if (roleWithCode != null)
                    {
                        return ApiResponse<RoleDto?>.ErrorResponse("Role with this code already exists");
                    }
                }

                var updateDto = new UpdateRoleDto
                {
                    Name = request.Name,
                    Code = request.Code,
                    Description = request.Description,
                    HierarchyLevel = request.HierarchyLevel,
                    Permissions = request.Permissions
                };

                var success = await _unitOfWork.WorkflowRoleRepository.UpdateAsync(request.Id, updateDto);
                if (!success)
                {
                    return ApiResponse<RoleDto?>.ErrorResponse("Failed to update role");
                }

                await _unitOfWork.SaveAsync();

                var updatedRole = await _unitOfWork.WorkflowRoleRepository.GetByIdAsync(request.Id);
                return ApiResponse<RoleDto?>.SuccessResponse(updatedRole, "Role updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto?>.ErrorResponse($"Error updating role: {ex.Message}");
            }
        }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _unitOfWork.WorkflowRoleRepository.ExistsAsync(request.Id);
                if (!exists)
                {
                    return ApiResponse<bool>.ErrorResponse("Role not found");
                }

                var success = await _unitOfWork.WorkflowRoleRepository.DeleteAsync(request.Id);
                if (success)
                {
                    await _unitOfWork.SaveAsync();
                }

                return ApiResponse<bool>.SuccessResponse(success, "Role deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting role: {ex.Message}");
            }
        }
    }

    public class ToggleRoleActiveCommandHandler : IRequestHandler<ToggleRoleActiveCommand, ApiResponse<RoleDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleRoleActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto?>> Handle(ToggleRoleActiveCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _unitOfWork.WorkflowRoleRepository.ExistsAsync(request.Id);
                if (!exists)
                {
                    return ApiResponse<RoleDto?>.ErrorResponse("Role not found");
                }

                var success = await _unitOfWork.WorkflowRoleRepository.ToggleActiveAsync(request.Id);
                if (!success)
                {
                    return ApiResponse<RoleDto?>.ErrorResponse("Failed to toggle role status");
                }

                await _unitOfWork.SaveAsync();

                var updatedRole = await _unitOfWork.WorkflowRoleRepository.GetByIdAsync(request.Id);
                return ApiResponse<RoleDto?>.SuccessResponse(updatedRole, "Role status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDto?>.ErrorResponse($"Error toggling role status: {ex.Message}");
            }
        }
    }
}
