using System;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Interface.IServices
{
    public interface IWorkflowNotificationService
    {
        /// <summary>
        /// Send notification when a document is assigned to a user
        /// </summary>
        Task NotifyDocumentAssignedAsync(Guid documentId, string documentType, Guid assignedToUserId, 
            string stageName, string actionRequired);

        /// <summary>
        /// Send notification when a document action is completed
        /// </summary>
        Task NotifyActionCompletedAsync(Guid documentId, string documentType, Guid processedByUserId, 
            string actionName, string? comments);

        /// <summary>
        /// Send notification when a document workflow is completed
        /// </summary>
        Task NotifyWorkflowCompletedAsync(Guid documentId, string documentType, Guid initiatedByUserId, 
            string finalStatus);

        /// <summary>
        /// Send notification when a document is rejected
        /// </summary>
        Task NotifyDocumentRejectedAsync(Guid documentId, string documentType, Guid rejectedByUserId, 
            Guid documentOwnerUserId, string reason);

        /// <summary>
        /// Send notification when feedback is added to a document
        /// </summary>
        Task NotifyFeedbackAddedAsync(Guid documentId, string documentType, Guid feedbackProviderId, 
            Guid documentOwnerUserId, string feedbackText);

        /// <summary>
        /// Send notification when a document is escalated
        /// </summary>
        Task NotifyDocumentEscalatedAsync(Guid documentId, string documentType, Guid escalatedByUserId, 
            Guid escalatedToUserId, string reason);

        /// <summary>
        /// Send reminder notification for pending actions
        /// </summary>
        Task SendPendingActionReminderAsync(Guid documentId, string documentType, Guid assignedToUserId, 
            string stageName, int daysPending);

        /// <summary>
        /// Send bulk notifications for multiple users
        /// </summary>
        Task SendBulkNotificationAsync(Guid[] userIds, string subject, string message, 
            string notificationType = "general");
    }
}
