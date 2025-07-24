using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.AcademicYear
{
    public record GetAcademicYearCommand() : IRequest<ApiResponse<List<AcademicYearDTO>>>;
    public record GetAcademicYearsByLevelCommand(int levelId) : IRequest<ApiResponse<List<AcademicYearDTO>>>;
    public record GetAcademicYearByIdCommand(int id) : IRequest<ApiResponse<AcademicYearDTO>>;
    public record CreateAcademicYearCommand(AcademicYearDTO AcademicYear) : IRequest<ApiResponse<int>>;
    public record UpdateAcademicYearCommand(AcademicYearDTO AcademicYear) : IRequest<ApiResponse<bool>>;
    public record DeleteOrRestoreAcademicYearCommand(int id, string modifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
}
