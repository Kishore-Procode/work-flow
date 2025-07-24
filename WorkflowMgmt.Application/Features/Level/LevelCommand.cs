using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Level
{
    public record GetLevelCommand() : IRequest<ApiResponse<List<LevelDTO>>>;
    public record GetLevelByIdCommand(int id) : IRequest<ApiResponse<LevelDTO>>;
    public record CreateLevelCommand(LevelDTO Level) : IRequest<ApiResponse<int>>;
    public record UpdateLevelCommand(LevelDTO Level) : IRequest<ApiResponse<bool>>;
    public record DeleteOrRestoreLevelCommand(int id, string modifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
}
