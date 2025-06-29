using Dapper;
using System.Data;
using System.Text.Json;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Domain.Models.Notifications;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class NotificationRepository : RepositoryTranBase, INotificationRepository
    {
        public NotificationRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<Guid> CreateNotificationAsync(CreateNotificationDto notification)
        {
            var sql = @"
                INSERT INTO workflowmgmt.notifications
                (user_id, notification_type_id, title, message, document_id, document_type,
                 related_user_id, metadata, priority, expires_at)
                SELECT @UserId, nt.id, @Title, @Message, @DocumentId, @DocumentType,
                       @RelatedUserId, @Metadata::jsonb, @Priority, @ExpiresAt
                FROM workflowmgmt.notification_types nt
                WHERE nt.type_name = @NotificationTypeName
                RETURNING id";

            var metadataJson = notification.Metadata != null ? JsonSerializer.Serialize(notification.Metadata) : null;

            var id = await Connection.QuerySingleAsync<Guid>(sql, new
            {
                notification.UserId,
                notification.NotificationTypeName,
                notification.Title,
                notification.Message,
                notification.DocumentId,
                notification.DocumentType,
                notification.RelatedUserId,
                Metadata = metadataJson,
                notification.Priority,
                notification.ExpiresAt
            }, Transaction);

            return id;
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id)
        {
            var sql = @"
                SELECT n.id, n.user_id, n.title, n.message, n.document_id, n.document_type,
                       n.related_user_id, n.metadata, n.is_read, n.priority, n.expires_at,
                       n.created_date, n.read_date, nt.type_name, nt.icon, nt.color,
                       ru.first_name || ' ' || ru.last_name as related_user_name,
                       ru.email as related_user_email
                FROM workflowmgmt.notifications n
                JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
                LEFT JOIN workflowmgmt.users ru ON n.related_user_id = ru.id
                WHERE n.id = @Id AND n.is_deleted = false";

            var result = await Connection.QueryFirstOrDefaultAsync(sql, new { Id = id }, Transaction);
            
            if (result == null) return null;

            return MapToNotificationDto(result);
        }

        public async Task<PaginatedNotificationsDto> GetUserNotificationsAsync(GetUserNotificationsRequest request)
        {
            var whereConditions = new List<string> { "n.user_id = @UserId", "n.is_deleted = false" };
            var parameters = new DynamicParameters();
            parameters.Add("UserId", request.UserId);

            if (request.UnreadOnly)
            {
                whereConditions.Add("n.is_read = false");
            }

            if (!string.IsNullOrEmpty(request.DocumentType))
            {
                whereConditions.Add("n.document_type = @DocumentType");
                parameters.Add("DocumentType", request.DocumentType);
            }

            if (request.Priority.HasValue)
            {
                whereConditions.Add("n.priority = @Priority");
                parameters.Add("Priority", request.Priority.Value);
            }

            if (request.FromDate.HasValue)
            {
                whereConditions.Add("n.created_date >= @FromDate");
                parameters.Add("FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                whereConditions.Add("n.created_date <= @ToDate");
                parameters.Add("ToDate", request.ToDate.Value);
            }

            var whereClause = string.Join(" AND ", whereConditions);
            var offset = (request.Page - 1) * request.PageSize;

            // Get total count
            var countSql = $@"
                SELECT COUNT(*)
                FROM workflowmgmt.notifications n
                JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
                WHERE {whereClause}";

            var totalCount = await Connection.QuerySingleAsync<int>(countSql, parameters, Transaction);

            // Get notifications
            var sql = $@"
                SELECT n.id, n.user_id, n.title, n.message, n.document_id, n.document_type,
                       n.related_user_id, n.metadata, n.is_read, n.priority, n.expires_at,
                       n.created_date, n.read_date, nt.type_name, nt.icon, nt.color,
                       ru.first_name || ' ' || ru.last_name as related_user_name,
                       ru.email as related_user_email
                FROM workflowmgmt.notifications n
                JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
                LEFT JOIN workflowmgmt.users ru ON n.related_user_id = ru.id
                WHERE {whereClause}
                ORDER BY n.created_date DESC
                LIMIT @PageSize OFFSET @Offset";

            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", offset);

            var results = await Connection.QueryAsync(sql, parameters, Transaction);
            var notifications = results.Select(MapToNotificationDto).ToList();

            return new PaginatedNotificationsDto
            {
                Notifications = notifications,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<List<NotificationDto>> GetRecentNotificationsAsync(Guid userId, int count = 5)
        {
            var sql = @"
                SELECT n.id, n.user_id, n.title, n.message, n.document_id, n.document_type,
                       n.related_user_id, n.metadata, n.is_read, n.priority, n.expires_at,
                       n.created_date, n.read_date, nt.type_name, nt.icon, nt.color,
                       ru.first_name || ' ' || ru.last_name as related_user_name,
                       ru.email as related_user_email
                FROM workflowmgmt.notifications n
                JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
                LEFT JOIN workflowmgmt.users ru ON n.related_user_id = ru.id
                WHERE n.user_id = @UserId AND n.is_deleted = false
                ORDER BY n.created_date DESC
                LIMIT @Count";

            var results = await Connection.QueryAsync(sql, new { UserId = userId, Count = count }, Transaction);
            return results.Select(MapToNotificationDto).ToList();
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
        {
            var sql = @"
                UPDATE workflowmgmt.notifications 
                SET is_read = true, read_date = NOW()
                WHERE id = @NotificationId AND user_id = @UserId AND is_read = false";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { NotificationId = notificationId, UserId = userId }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var sql = @"
                UPDATE workflowmgmt.notifications 
                SET is_read = true, read_date = NOW()
                WHERE user_id = @UserId AND is_read = false AND is_deleted = false";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { UserId = userId }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            var sql = @"
                UPDATE workflowmgmt.notifications 
                SET is_deleted = true
                WHERE id = @NotificationId AND user_id = @UserId";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { NotificationId = notificationId, UserId = userId }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAllReadNotificationsAsync(Guid userId)
        {
            var sql = @"
                UPDATE workflowmgmt.notifications 
                SET is_deleted = true
                WHERE user_id = @UserId AND is_read = true";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { UserId = userId }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM workflowmgmt.notifications
                WHERE user_id = @UserId AND is_read = false AND is_deleted = false";

            return await Connection.QuerySingleAsync<int>(sql, new { UserId = userId }, Transaction);
        }

        public async Task<NotificationSummaryDto> GetNotificationSummaryAsync(Guid userId)
        {
            var sql = @"
                SELECT 
                    COUNT(*) as total_count,
                    COUNT(*) FILTER (WHERE is_read = false) as unread_count,
                    COUNT(*) FILTER (WHERE priority = 3 AND is_read = false) as high_priority_count,
                    COUNT(*) FILTER (WHERE priority = 4 AND is_read = false) as urgent_count
                FROM workflowmgmt.notifications
                WHERE user_id = @UserId AND is_deleted = false";

            var result = await Connection.QuerySingleAsync(sql, new { UserId = userId }, Transaction);
            var recentNotifications = await GetRecentNotificationsAsync(userId, 5);

            return new NotificationSummaryDto
            {
                TotalCount = result.total_count,
                UnreadCount = result.unread_count,
                HighPriorityCount = result.high_priority_count,
                UrgentCount = result.urgent_count,
                RecentNotifications = recentNotifications
            };
        }

        public async Task<Dictionary<string, int>> GetNotificationCountsByTypeAsync(Guid userId)
        {
            var sql = @"
                SELECT nt.type_name, COUNT(*) as count
                FROM workflowmgmt.notifications n
                JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
                WHERE n.user_id = @UserId AND n.is_read = false AND n.is_deleted = false
                GROUP BY nt.type_name";

            var results = await Connection.QueryAsync(sql, new { UserId = userId }, Transaction);
            return results.ToDictionary(r => (string)r.type_name, r => (int)r.count);
        }

        private static NotificationDto MapToNotificationDto(dynamic result)
        {
            var metadata = !string.IsNullOrEmpty(result.metadata) 
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(result.metadata)
                : null;

            return new NotificationDto
            {
                Id = result.id,
                UserId = result.user_id,
                Title = result.title,
                Message = result.message,
                DocumentId = result.document_id,
                DocumentType = result.document_type,
                RelatedUserId = result.related_user_id,
                RelatedUserName = result.related_user_name,
                RelatedUserEmail = result.related_user_email,
                Metadata = metadata,
                IsRead = result.is_read,
                Priority = result.priority,
                ExpiresAt = result.expires_at,
                CreatedDate = result.created_date,
                ReadDate = result.read_date,
                TypeName = result.type_name,
                Icon = result.icon,
                Color = result.color
            };
        }

        // Additional methods will be implemented in the next part...
        public async Task<List<NotificationTypeDto>> GetNotificationTypesAsync()
        {
            var sql = @"
                SELECT id, type_name, description, icon, color, is_active
                FROM workflowmgmt.notification_types
                WHERE is_active = true
                ORDER BY type_name";

            var results = await Connection.QueryAsync<NotificationTypeDto>(sql, transaction: Transaction);
            return results.ToList();
        }

        public async Task<NotificationTypeDto?> GetNotificationTypeByNameAsync(string typeName)
        {
            var sql = @"
                SELECT id, type_name, description, icon, color, is_active
                FROM workflowmgmt.notification_types
                WHERE type_name = @TypeName AND is_active = true";

            return await Connection.QueryFirstOrDefaultAsync<NotificationTypeDto>(sql, new { TypeName = typeName }, Transaction);
        }

        public async Task<List<NotificationPreferenceDto>> GetUserNotificationPreferencesAsync(Guid userId)
        {
            var sql = @"
                SELECT unp.id, unp.user_id, unp.notification_type_id, unp.is_enabled, unp.delivery_method,
                       nt.type_name, nt.description
                FROM workflowmgmt.user_notification_preferences unp
                JOIN workflowmgmt.notification_types nt ON unp.notification_type_id = nt.id
                WHERE unp.user_id = @UserId AND nt.is_active = true
                ORDER BY nt.type_name";

            var results = await Connection.QueryAsync<NotificationPreferenceDto>(sql, new { UserId = userId }, Transaction);
            return results.ToList();
        }

        public async Task<bool> UpdateUserNotificationPreferenceAsync(Guid userId, UpdateNotificationPreferenceDto preference)
        {
            var sql = @"
                UPDATE workflowmgmt.user_notification_preferences 
                SET is_enabled = @IsEnabled, delivery_method = @DeliveryMethod, modified_date = NOW()
                WHERE user_id = @UserId AND notification_type_id = @NotificationTypeId";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                preference.NotificationTypeId,
                preference.IsEnabled,
                preference.DeliveryMethod
            }, Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> CreateDefaultPreferencesForUserAsync(Guid userId)
        {
            var sql = @"
                INSERT INTO workflowmgmt.user_notification_preferences (user_id, notification_type_id, is_enabled, delivery_method)
                SELECT @UserId, nt.id, true, 'in_app'
                FROM workflowmgmt.notification_types nt
                WHERE nt.is_active = true
                ON CONFLICT (user_id, notification_type_id) DO NOTHING";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { UserId = userId }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<NotificationTemplate?> GetNotificationTemplateAsync(string typeName)
        {
            var sql = @"
                SELECT nt.id, nt.notification_type_id, nt.template_name, nt.title_template, 
                       nt.message_template, nt.variables, nt.is_active
                FROM workflowmgmt.notification_templates nt
                JOIN workflowmgmt.notification_types ntype ON nt.notification_type_id = ntype.id
                WHERE ntype.type_name = @TypeName AND nt.is_active = true
                LIMIT 1";

            return await Connection.QueryFirstOrDefaultAsync<NotificationTemplate>(sql, new { TypeName = typeName }, Transaction);
        }

        public async Task<string> ProcessNotificationTemplateAsync(string template, Dictionary<string, object> variables)
        {
            var processedTemplate = template;
            
            foreach (var variable in variables)
            {
                var placeholder = $"{{{variable.Key}}}";
                processedTemplate = processedTemplate.Replace(placeholder, variable.Value?.ToString() ?? "");
            }

            return processedTemplate;
        }

        public async Task<List<Guid>> CreateBulkNotificationsAsync(List<CreateNotificationDto> notifications)
        {
            var ids = new List<Guid>();
            
            foreach (var notification in notifications)
            {
                var id = await CreateNotificationAsync(notification);
                ids.Add(id);
            }

            return ids;
        }

        public async Task<bool> MarkNotificationsAsReadByDocumentAsync(Guid documentId, Guid userId)
        {
            var sql = @"
                UPDATE workflowmgmt.notifications 
                SET is_read = true, read_date = NOW()
                WHERE document_id = @DocumentId AND user_id = @UserId AND is_read = false";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { DocumentId = documentId, UserId = userId }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<List<NotificationDto>> GetDocumentNotificationsAsync(Guid documentId, Guid? userId = null)
        {
            var whereClause = "n.document_id = @DocumentId AND n.is_deleted = false";
            var parameters = new DynamicParameters();
            parameters.Add("DocumentId", documentId);

            if (userId.HasValue)
            {
                whereClause += " AND n.user_id = @UserId";
                parameters.Add("UserId", userId.Value);
            }

            var sql = $@"
                SELECT n.id, n.user_id, n.title, n.message, n.document_id, n.document_type,
                       n.related_user_id, n.metadata, n.is_read, n.priority, n.expires_at,
                       n.created_date, n.read_date, nt.type_name, nt.icon, nt.color,
                       ru.first_name || ' ' || ru.last_name as related_user_name,
                       ru.email as related_user_email
                FROM workflowmgmt.notifications n
                JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
                LEFT JOIN workflowmgmt.users ru ON n.related_user_id = ru.id
                WHERE {whereClause}
                ORDER BY n.created_date DESC";

            var results = await Connection.QueryAsync(sql, parameters, Transaction);
            return results.Select(MapToNotificationDto).ToList();
        }

        public async Task<bool> CreateDocumentNotificationAsync(DocumentNotificationContext context, List<Guid> userIds)
        {
            // This method will be implemented to create notifications for document events
            // It will use the notification templates and create notifications for multiple users
            return true; // Placeholder
        }

        public async Task<int> CleanupOldNotificationsAsync(int daysToKeep = 90)
        {
            var sql = @"
                DELETE FROM workflowmgmt.notifications 
                WHERE created_date < NOW() - INTERVAL '@DaysToKeep days'
                AND is_read = true";

            return await Connection.ExecuteAsync(sql, new { DaysToKeep = daysToKeep }, Transaction);
        }

        public async Task<int> CleanupExpiredNotificationsAsync()
        {
            var sql = @"
                UPDATE workflowmgmt.notifications 
                SET is_deleted = true
                WHERE expires_at IS NOT NULL AND expires_at < NOW() AND is_deleted = false";

            return await Connection.ExecuteAsync(sql, transaction: Transaction);
        }

        public async Task<Dictionary<string, object>> GetNotificationStatisticsAsync(Guid? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Implementation for notification statistics
            return new Dictionary<string, object>(); // Placeholder
        }
    }
}
