using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Application.Features.Auth;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Interface.JwtToken;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Department
{
    public class DepartmentCommandHandler : IRequestHandler<GetDepartmentCommand, ApiResponse<List<DepartmentDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DepartmentDTO>>> Handle(GetDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var departments = await _unitOfWork.DepartmentRepository.GetAllDepartments();

                return ApiResponse<List<DepartmentDTO>>.SuccessResponse(departments, "Departments retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DepartmentDTO>>.ErrorResponse($"Error during fetching departments: {ex.Message}");
            }
        }
    }

    public class GetDepartmentsByLevelCommandHandler : IRequestHandler<GetDepartmentsByLevelCommand, ApiResponse<List<DepartmentDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentsByLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DepartmentDTO>>> Handle(GetDepartmentsByLevelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var departments = await _unitOfWork.DepartmentRepository.GetDepartmentsByLevelId(request.levelId);

                return ApiResponse<List<DepartmentDTO>>.SuccessResponse(departments, "Departments retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DepartmentDTO>>.ErrorResponse($"Error during fetching departments: {ex.Message}");
            }
        }
    }
    public class GetDepartmentByIdCommandHandler : IRequestHandler<GetDepartmentByIdCommand, ApiResponse<DepartmentDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DepartmentDTO>> Handle(GetDepartmentByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _unitOfWork.DepartmentRepository.GetDepartmentById(request.id);

                if (department == null)
                    return ApiResponse<DepartmentDTO>.ErrorResponse($"Department with ID {request.id} not found");

                return ApiResponse<DepartmentDTO>.SuccessResponse(department, "Department retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DepartmentDTO>.ErrorResponse($"Error fetching department: {ex.Message}");
            }
        }
    }
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ApiResponse<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var departmentId = await _unitOfWork.DepartmentRepository.InsertDepartment(request.Department);
                if (departmentId > 0)
                    _unitOfWork.Commit();
                return ApiResponse<int>.SuccessResponse(departmentId, "Department created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating department: {ex.Message}");
            }
        }
    }
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.DepartmentRepository.UpdateDepartment(request.Department);
                if (success)
                    _unitOfWork.Commit();
                return ApiResponse<bool>.SuccessResponse(success, "Department updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating department: {ex.Message}");
            }
        }
    }
    public class DeleteOrRestoreDepartmentCommandHandler : IRequestHandler<DeleteOrRestoreDepartmentCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _unitOfWork.DepartmentRepository
                    .DeleteOrRestoreDepartment(request.id, request.modifiedBy, request.isRestore);
                if (updated)
                    _unitOfWork.Commit();
                return ApiResponse<bool>.SuccessResponse(updated, "Department restored successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Operation failed: {ex.Message}");
            }
        }
    }
}
