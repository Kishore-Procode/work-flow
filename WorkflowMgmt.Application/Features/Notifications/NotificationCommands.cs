using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Notifications;

namespace WorkflowMgmt.Application.Features.Notifications
{
    // Commands
    public class CreateNotificationCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid UserId { get; set; }
        public string NotificationTypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? DocumentId { get; set; }
        public string? DocumentType { get; set; }
        public Guid? RelatedUserId { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public int Priority { get; set; } = 1;
        public DateTime? ExpiresAt { get; set; }
    }

    public class CreateBulkNotificationsCommand : IRequest<ApiResponse<List<Guid>>>
    {
        public List<CreateNotificationDto> Notifications { get; set; } = new();
    }

    public class MarkNotificationAsReadCommand : IRequest<ApiResponse<bool>>
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }

    public class MarkAllNotificationsAsReadCommand : IRequest<ApiResponse<bool>>
    {
        public Guid UserId { get; set; }
    }

    public class DeleteNotificationCommand : IRequest<ApiResponse<bool>>
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteAllReadNotificationsCommand : IRequest<ApiResponse<bool>>
    {
        public Guid UserId { get; set; }
    }

    public class UpdateNotificationPreferenceCommand : IRequest<ApiResponse<bool>>
    {
        public Guid UserId { get; set; }
        public UpdateNotificationPreferenceDto Preference { get; set; } = new();
    }

    public class CreateDocumentNotificationCommand : IRequest<ApiResponse<bool>>
    {
        public DocumentNotificationContext Context { get; set; } = new();
        public List<Guid> UserIds { get; set; } = new();
        public bool SendRealTime { get; set; } = true;
    }

    // Queries
    public class GetUserNotificationsQuery : IRequest<ApiResponse<PaginatedNotificationsDto>>
    {
        public Guid UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool UnreadOnly { get; set; } = false;
        public string? DocumentType { get; set; }
        public int? Priority { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetNotificationByIdQuery : IRequest<ApiResponse<NotificationDto>>
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetUnreadNotificationCountQuery : IRequest<ApiResponse<int>>
    {
        public Guid UserId { get; set; }
    }

    public class GetNotificationSummaryQuery : IRequest<ApiResponse<NotificationSummaryDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetRecentNotificationsQuery : IRequest<ApiResponse<List<NotificationDto>>>
    {
        public Guid UserId { get; set; }
        public int Count { get; set; } = 5;
    }

    public class GetNotificationTypesQuery : IRequest<ApiResponse<List<NotificationTypeDto>>>
    {
    }

    public class GetUserNotificationPreferencesQuery : IRequest<ApiResponse<List<NotificationPreferenceDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetDocumentNotificationsQuery : IRequest<ApiResponse<List<NotificationDto>>>
    {
        public Guid DocumentId { get; set; }
        public Guid? UserId { get; set; }
    }

    public class GetNotificationCountsByTypeQuery : IRequest<ApiResponse<Dictionary<string, int>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetNotificationStatisticsQuery : IRequest<ApiResponse<Dictionary<string, object>>>
    {
        public Guid? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
