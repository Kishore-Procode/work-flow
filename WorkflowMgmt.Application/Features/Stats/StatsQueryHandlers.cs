using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Stats;

namespace WorkflowMgmt.Application.Features.Stats
{
    public class GetDepartmentStatsQueryHandler : IRequestHandler<GetDepartmentStatsQuery, ApiResponse<DepartmentStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DepartmentStatsDto>> Handle(GetDepartmentStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var stats = await _unitOfWork.StatsRepository.GetDepartmentStatsAsync();
                return ApiResponse<DepartmentStatsDto>.SuccessResponse(stats, "Department statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DepartmentStatsDto>.ErrorResponse($"Error retrieving department statistics: {ex.Message}");
            }
        }
    }

    public class GetCourseStatsQueryHandler : IRequestHandler<GetCourseStatsQuery, ApiResponse<CourseStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<CourseStatsDto>> Handle(GetCourseStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var stats = await _unitOfWork.StatsRepository.GetCourseStatsAsync();
                return ApiResponse<CourseStatsDto>.SuccessResponse(stats, "Course statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseStatsDto>.ErrorResponse($"Error retrieving course statistics: {ex.Message}");
            }
        }
    }
}
