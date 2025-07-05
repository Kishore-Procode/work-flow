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
using WorkflowMgmt.Domain.Models.User;

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

    public class GetSemesterStatsQueryHandler : IRequestHandler<GetSemesterStatsQuery, ApiResponse<SemesterStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSemesterStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<SemesterStatsDto>> Handle(GetSemesterStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var stats = await _unitOfWork.StatsRepository.GetSemesterStatsAsync();
                return ApiResponse<SemesterStatsDto>.SuccessResponse(stats, "Semester statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<SemesterStatsDto>.ErrorResponse($"Error retrieving semester statistics: {ex.Message}");
            }
        }
    }

    public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, ApiResponse<UserStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserStatsDto>> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var stats = await _unitOfWork.StatsRepository.GetUserStatsAsync();
                return ApiResponse<UserStatsDto>.SuccessResponse(stats, "User statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserStatsDto>.ErrorResponse($"Error retrieving user statistics: {ex.Message}");
            }
        }
    }
}
