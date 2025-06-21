using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowStageRoles
{
    public class GetWorkflowStageRolesQueryHandler : IRequestHandler<GetWorkflowStageRolesQuery, ApiResponse<List<WorkflowStageRoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowStageRoleDto>>> Handle(GetWorkflowStageRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _unitOfWork.WorkflowStageRoleRepository.GetByStageIdAsync(request.WorkflowStageId);
                return ApiResponse<List<WorkflowStageRoleDto>>.SuccessResponse(roles, "Workflow stage roles retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowStageRoleDto>>.ErrorResponse($"Error retrieving workflow stage roles: {ex.Message}");
            }
        }
    }

    public class GetActiveRolesQueryHandler : IRequestHandler<GetActiveRolesQuery, ApiResponse<List<RoleOptionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<RoleOptionDto>>> Handle(GetActiveRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _unitOfWork.WorkflowStageDetailsRepository.GetActiveRolesAsync();
                return ApiResponse<List<RoleOptionDto>>.SuccessResponse(roles, "Active roles retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<RoleOptionDto>>.ErrorResponse($"Error retrieving active roles: {ex.Message}");
            }
        }
    }

    public class GetWorkflowStageDetailsQueryHandler : IRequestHandler<GetWorkflowStageDetailsQuery, ApiResponse<List<WorkflowStageDetailsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageDetailsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowStageDetailsDto>>> Handle(GetWorkflowStageDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var stageDetails = await _unitOfWork.WorkflowStageDetailsRepository.GetByWorkflowTemplateIdAsync(request.WorkflowTemplateId);
                return ApiResponse<List<WorkflowStageDetailsDto>>.SuccessResponse(stageDetails, "Workflow stage details retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowStageDetailsDto>>.ErrorResponse($"Error retrieving workflow stage details: {ex.Message}");
            }
        }
    }

    public class GetWorkflowStageDetailsByIdQueryHandler : IRequestHandler<GetWorkflowStageDetailsByIdQuery, ApiResponse<WorkflowStageDetailsDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageDetailsByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WorkflowStageDetailsDto?>> Handle(GetWorkflowStageDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var stageDetails = await _unitOfWork.WorkflowStageDetailsRepository.GetByStageIdAsync(request.StageId);
                if (stageDetails == null)
                {
                    return ApiResponse<WorkflowStageDetailsDto?>.ErrorResponse("Workflow stage not found");
                }
                return ApiResponse<WorkflowStageDetailsDto?>.SuccessResponse(stageDetails, "Workflow stage details retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<WorkflowStageDetailsDto?>.ErrorResponse($"Error retrieving workflow stage details: {ex.Message}");
            }
        }
    }

    public class UpdateWorkflowStageRolesCommandHandler : IRequestHandler<UpdateWorkflowStageRolesCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowStageRolesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateWorkflowStageRolesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.WorkflowStageRoleRepository.UpdateStageRolesAsync(request.StageId, request.Roles);
                if (success)
                {
                    await _unitOfWork.SaveAsync();
                }
                return ApiResponse<bool>.SuccessResponse(success, "Workflow stage roles updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating workflow stage roles: {ex.Message}");
            }
        }
    }

    public class AddWorkflowStageRoleCommandHandler : IRequestHandler<AddWorkflowStageRoleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddWorkflowStageRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(AddWorkflowStageRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if role already exists for this stage
                var exists = await _unitOfWork.WorkflowStageRoleRepository.ExistsAsync(request.StageId, request.RoleCode);
                if (exists)
                {
                    return ApiResponse<bool>.ErrorResponse($"Role '{request.RoleCode}' is already assigned to this stage.");
                }

                var createDto = new CreateWorkflowStageRoleDto
                {
                    RoleCode = request.RoleCode,
                    IsRequired = request.IsRequired
                };

                var success = await _unitOfWork.WorkflowStageRoleRepository.CreateAsync(request.StageId, createDto);
                if (success)
                {
                    await _unitOfWork.SaveAsync();
                }
                return ApiResponse<bool>.SuccessResponse(success, "Role added to workflow stage successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error adding role to workflow stage: {ex.Message}");
            }
        }
    }

    public class RemoveWorkflowStageRoleCommandHandler : IRequestHandler<RemoveWorkflowStageRoleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveWorkflowStageRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(RemoveWorkflowStageRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // For now, we'll delete all roles and recreate without the specified one
                // This is a simplified approach - in production you might want a more targeted delete
                var currentRoles = await _unitOfWork.WorkflowStageRoleRepository.GetByStageIdAsync(request.StageId);
                var rolesToKeep = currentRoles.Where(r => r.RoleCode != request.RoleCode)
                                             .Select(r => new UpdateRoleDto
                                             {
                                                 RoleCode = r.RoleCode,
                                                 IsRequired = r.IsRequired
                                             }).ToList();

                var success = await _unitOfWork.WorkflowStageRoleRepository.UpdateStageRolesAsync(request.StageId, rolesToKeep);
                if (success)
                {
                    await _unitOfWork.SaveAsync();
                }
                return ApiResponse<bool>.SuccessResponse(success, "Role removed from workflow stage successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error removing role from workflow stage: {ex.Message}");
            }
        }
    }
}
