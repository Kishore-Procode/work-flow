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

                return ApiResponse<List<DepartmentDTO>>.SuccessResponse(departments, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DepartmentDTO>>.ErrorResponse($"Error during login: {ex.Message}");
            }
        }

    }
}
