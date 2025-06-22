using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowRoleMapping
{
    public class GetDepartmentRoleUsersCommandHandler : IRequestHandler<GetDepartmentRoleUsersCommand, ApiResponse<List<WorkflowRoleMappingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentRoleUsersCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowRoleMappingDto>>> Handle(GetDepartmentRoleUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mappings = await _unitOfWork.WorkflowRoleMappingRepository.GetDepartmentRoleUsers();
                return ApiResponse<List<WorkflowRoleMappingDto>>.SuccessResponse(mappings, "Department role users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowRoleMappingDto>>.ErrorResponse($"Error during fetching department role users: {ex.Message}");
            }
        }
    }

    public class GetDepartmentRoleUsersByDepartmentCommandHandler : IRequestHandler<GetDepartmentRoleUsersByDepartmentCommand, ApiResponse<List<WorkflowRoleMappingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentRoleUsersByDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowRoleMappingDto>>> Handle(GetDepartmentRoleUsersByDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mappings = await _unitOfWork.WorkflowRoleMappingRepository.GetDepartmentRoleUsersByDepartment(request.departmentId);
                return ApiResponse<List<WorkflowRoleMappingDto>>.SuccessResponse(mappings, "Department role users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowRoleMappingDto>>.ErrorResponse($"Error during fetching department role users: {ex.Message}");
            }
        }
    }

    public class UpdateDepartmentRoleUsersCommandHandler : IRequestHandler<UpdateDepartmentRoleUsersCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentRoleUsersCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDepartmentRoleUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate that each role has only one primary user
                var roleGroups = request.request.assignments.GroupBy(a => a.role_id);
                foreach (var roleGroup in roleGroups)
                {
                    var primaryCount = roleGroup.Count(a => a.is_primary);
                    if (primaryCount > 1)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Role ID {roleGroup.Key} cannot have more than one primary user.");
                    }
                }

                var success = await _unitOfWork.WorkflowRoleMappingRepository.UpdateDepartmentRoleUsers(
                    request.request.department_id, 
                    request.request.assignments);

                if (success)
                    _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(success, "Department role users updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating department role users: {ex.Message}");
            }
        }
    }
}
