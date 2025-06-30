using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Dashboard;

namespace WorkflowMgmt.Application.Features.Dashboard
{
    public class GetDashboardDataQuery : IRequest<ApiResponse<RoleDashboardDto>>
    {
        public string UserId { get; set; } = string.Empty;

        public GetDashboardDataQuery(string userId)
        {
            UserId = userId;
        }
    }
}
