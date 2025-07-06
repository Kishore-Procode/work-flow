using System.Text.Json;

namespace WorkflowMgmt.Domain.Models.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? DocumentId { get; set; }
        public string? DocumentType { get; set; }
        public Guid? RelatedUserId { get; set; }
        public string? RelatedUserName { get; set; }
        public string? RelatedUserEmail { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public bool IsRead { get; set; }
        public int Priority { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
        
        // Notification type info
        public string TypeName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        
        // Computed properties
        public string TimeAgo => GetTimeAgo();
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.Now;
        public string PriorityText => GetPriorityText();

        private string GetTimeAgo()
        {
            var timeSpan = DateTime.Now - CreatedDate;
            
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)}w ago";
            
            return CreatedDate.ToString("MMM dd, yyyy");
        }

        private string GetPriorityText()
        {
            return Priority switch
            {
                1 => "Low",
                2 => "Medium",
                3 => "High",
                4 => "Urgent",
                _ => "Normal"
            };
        }
    }

    public class CreateNotificationDto
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

    public class NotificationSummaryDto
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int HighPriorityCount { get; set; }
        public int UrgentCount { get; set; }
        public List<NotificationDto> RecentNotifications { get; set; } = new();
    }

    public class NotificationPreferenceDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsEnabled { get; set; }
        public string DeliveryMethod { get; set; } = "in_app";
    }

    public class UpdateNotificationPreferenceDto
    {
        public int NotificationTypeId { get; set; }
        public bool IsEnabled { get; set; }
        public string DeliveryMethod { get; set; } = "in_app";
    }

    public class NotificationTypeDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; }
    }

    public class PaginatedNotificationsDto
    {
        public List<NotificationDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    // Request DTOs
    public class GetUserNotificationsRequest
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

    public class MarkNotificationAsReadRequest
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }

    public class MarkAllNotificationsAsReadRequest
    {
        public Guid UserId { get; set; }
    }

    public class DeleteNotificationRequest
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }

    // Document notification specific DTOs
    public class DocumentNotificationContext
    {
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentTitle { get; set; } = string.Empty;
        public string? PreviousStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? StageName { get; set; }
        public string? ActionType { get; set; }
        public string? Comments { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? ProcessedByUserId { get; set; }
        public int? DepartmentId { get; set; }
    }

    // Real-time notification DTO for SignalR
    public class RealTimeNotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? DocumentId { get; set; }
        public string? DocumentType { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        
        public string TimeAgo => GetTimeAgo();

        private string GetTimeAgo()
        {
            var timeSpan = DateTime.Now - CreatedDate;
            
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            
            return CreatedDate.ToString("MMM dd, HH:mm");
        }
    }
}
