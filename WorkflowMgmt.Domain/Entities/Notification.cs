using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? DocumentId { get; set; }
        public string? DocumentType { get; set; }
        public Guid? RelatedUserId { get; set; }
        public string? Metadata { get; set; } // JSON string
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public int Priority { get; set; } = 1;
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReadDate { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public NotificationType? NotificationType { get; set; }
        public User? RelatedUser { get; set; }
    }

    public class NotificationType
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<UserNotificationPreference> UserPreferences { get; set; } = new List<UserNotificationPreference>();
        public ICollection<NotificationTemplate> Templates { get; set; } = new List<NotificationTemplate>();
    }

    public class UserNotificationPreference
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string DeliveryMethod { get; set; } = "in_app"; // 'in_app', 'email', 'both'
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public NotificationType? NotificationType { get; set; }
    }

    public class NotificationTemplate
    {
        public Guid Id { get; set; }
        public int NotificationTypeId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string TitleTemplate { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string? Variables { get; set; } // JSON string
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public NotificationType? NotificationType { get; set; }
    }

    public class NotificationDeliveryLog
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public string DeliveryMethod { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = "pending"; // 'pending', 'sent', 'delivered', 'failed'
        public int DeliveryAttempt { get; set; } = 1;
        public string? ErrorMessage { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Notification? Notification { get; set; }
    }
}
