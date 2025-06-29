using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models.Notifications;

namespace WorkflowMgmt.Domain.IRepository
{
    public interface INotificationRepository
    {
        // Notification CRUD operations
        Task<Guid> CreateNotificationAsync(CreateNotificationDto notification);
        Task<NotificationDto?> GetNotificationByIdAsync(Guid id);
        Task<PaginatedNotificationsDto> GetUserNotificationsAsync(GetUserNotificationsRequest request);
        Task<List<NotificationDto>> GetRecentNotificationsAsync(Guid userId, int count = 5);
        Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
        Task<bool> MarkAllNotificationsAsReadAsync(Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);
        Task<bool> DeleteAllReadNotificationsAsync(Guid userId);

        // Notification counts and summaries
        Task<int> GetUnreadNotificationCountAsync(Guid userId);
        Task<NotificationSummaryDto> GetNotificationSummaryAsync(Guid userId);
        Task<Dictionary<string, int>> GetNotificationCountsByTypeAsync(Guid userId);

        // Notification types
        Task<List<NotificationTypeDto>> GetNotificationTypesAsync();
        Task<NotificationTypeDto?> GetNotificationTypeByNameAsync(string typeName);

        // User preferences
        Task<List<NotificationPreferenceDto>> GetUserNotificationPreferencesAsync(Guid userId);
        Task<bool> UpdateUserNotificationPreferenceAsync(Guid userId, UpdateNotificationPreferenceDto preference);
        Task<bool> CreateDefaultPreferencesForUserAsync(Guid userId);

        // Notification templates
        Task<NotificationTemplate?> GetNotificationTemplateAsync(string typeName);
        Task<string> ProcessNotificationTemplateAsync(string template, Dictionary<string, object> variables);

        // Bulk operations
        Task<List<Guid>> CreateBulkNotificationsAsync(List<CreateNotificationDto> notifications);
        Task<bool> MarkNotificationsAsReadByDocumentAsync(Guid documentId, Guid userId);

        // Document-specific notifications
        Task<List<NotificationDto>> GetDocumentNotificationsAsync(Guid documentId, Guid? userId = null);
        Task<bool> CreateDocumentNotificationAsync(DocumentNotificationContext context, List<Guid> userIds);

        // Cleanup operations
        Task<int> CleanupOldNotificationsAsync(int daysToKeep = 90);
        Task<int> CleanupExpiredNotificationsAsync();

        // Statistics
        Task<Dictionary<string, object>> GetNotificationStatisticsAsync(Guid? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
