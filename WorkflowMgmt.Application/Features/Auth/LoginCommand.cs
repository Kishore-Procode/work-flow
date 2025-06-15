using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Auth
{
    public record LoginCommand(
    string Username,
    string Password
) : IRequest<ApiResponse<LoginResponse>>;
}
