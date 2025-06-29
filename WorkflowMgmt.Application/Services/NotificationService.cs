using Microsoft.AspNetCore.SignalR;
using WorkflowMgmt.Application.Hubs;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Notifications;

namespace WorkflowMgmt.Application.Services
{
    public interface INotificationService
    {
        Task SendDocumentStatusUpdatedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            string previousStatus, string newStatus, Guid processedByUserId, List<Guid> recipientUserIds);
        
        Task SendDocumentCommentedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            string comment, Guid commentedByUserId, List<Guid> recipientUserIds);
        
        Task SendDocumentApprovedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            Guid approvedByUserId, Guid recipientUserId);
        
        Task SendDocumentRejectedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            string reason, Guid rejectedByUserId, Guid recipientUserId);
        
        Task SendDocumentAssignedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            string stageName, Guid assignedByUserId, Guid assignedToUserId);
        
        Task SendWorkflowStageChangedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            string previousStage, string newStage, List<Guid> recipientUserIds);
        
        Task SendFeedbackReceivedNotificationAsync(Guid documentId, string documentType, string documentTitle, 
            string feedbackType, Guid feedbackByUserId, Guid recipientUserId);
    }

    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task SendDocumentStatusUpdatedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, string previousStatus, string newStatus, Guid processedByUserId, List<Guid> recipientUserIds)
        {
            var processedByUser = await _unitOfWork.UserRepository.GetUserById(processedByUserId);
            var processedByName = processedByUser != null ? $"{processedByUser.first_name} {processedByUser.last_name}" : "System";

            var notifications = recipientUserIds.Select(userId => new CreateNotificationDto
            {
                UserId = userId,
                NotificationTypeName = "document_status_updated",
                Title = "Document Status Updated",
                Message = $"The status of \"{documentTitle}\" has been changed from {previousStatus} to {newStatus} by {processedByName}",
                DocumentId = documentId,
                DocumentType = documentType,
                RelatedUserId = processedByUserId,
                Priority = 2,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "previous_status", previousStatus },
                    { "new_status", newStatus },
                    { "processed_by_name", processedByName }
                }
            }).ToList();

            await CreateAndSendNotificationsAsync(notifications);
        }

        public async Task SendDocumentCommentedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, string comment, Guid commentedByUserId, List<Guid> recipientUserIds)
        {
            var commentedByUser = await _unitOfWork.UserRepository.GetUserById(commentedByUserId);
            var commentedByName = commentedByUser != null ? $"{commentedByUser.first_name} {commentedByUser.last_name}" : "Unknown User";

            var notifications = recipientUserIds.Select(userId => new CreateNotificationDto
            {
                UserId = userId,
                NotificationTypeName = "document_commented",
                Title = "New Comment Added",
                Message = $"{commentedByName} added a comment to \"{documentTitle}\"",
                DocumentId = documentId,
                DocumentType = documentType,
                RelatedUserId = commentedByUserId,
                Priority = 2,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "comment", comment },
                    { "commented_by_name", commentedByName }
                }
            }).ToList();

            await CreateAndSendNotificationsAsync(notifications);
        }

        public async Task SendDocumentApprovedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, Guid approvedByUserId, Guid recipientUserId)
        {
            var approvedByUser = await _unitOfWork.UserRepository.GetUserById(approvedByUserId);
            var approvedByName = approvedByUser != null ? $"{approvedByUser.first_name} {approvedByUser.last_name}" : "Unknown User";

            var notification = new CreateNotificationDto
            {
                UserId = recipientUserId,
                NotificationTypeName = "document_approved",
                Title = "Document Approved",
                Message = $"Your document \"{documentTitle}\" has been approved by {approvedByName}",
                DocumentId = documentId,
                DocumentType = documentType,
                RelatedUserId = approvedByUserId,
                Priority = 3,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "approved_by_name", approvedByName }
                }
            };

            await CreateAndSendNotificationsAsync(new List<CreateNotificationDto> { notification });
        }

        public async Task SendDocumentRejectedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, string reason, Guid rejectedByUserId, Guid recipientUserId)
        {
            var rejectedByUser = await _unitOfWork.UserRepository.GetUserById(rejectedByUserId);
            var rejectedByName = rejectedByUser != null ? $"{rejectedByUser.first_name} {rejectedByUser.last_name}" : "Unknown User";

            var notification = new CreateNotificationDto
            {
                UserId = recipientUserId,
                NotificationTypeName = "document_rejected",
                Title = "Document Rejected",
                Message = $"Your document \"{documentTitle}\" has been rejected by {rejectedByName}",
                DocumentId = documentId,
                DocumentType = documentType,
                RelatedUserId = rejectedByUserId,
                Priority = 3,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "rejected_by_name", rejectedByName },
                    { "reason", reason }
                }
            };

            await CreateAndSendNotificationsAsync(new List<CreateNotificationDto> { notification });
        }

        public async Task SendDocumentAssignedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, string stageName, Guid assignedByUserId, Guid assignedToUserId)
        {
            var assignedByUser = await _unitOfWork.UserRepository.GetUserById(assignedByUserId);
            var assignedByName = assignedByUser != null ? $"{assignedByUser.first_name} {assignedByUser.last_name}" : "System";

            var notification = new CreateNotificationDto
            {
                UserId = assignedToUserId,
                NotificationTypeName = "document_assigned",
                Title = "Document Assigned",
                Message = $"Document \"{documentTitle}\" has been assigned to you for {stageName}",
                DocumentId = documentId,
                DocumentType = documentType,
                RelatedUserId = assignedByUserId,
                Priority = 3,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "stage_name", stageName },
                    { "assigned_by_name", assignedByName }
                }
            };

            await CreateAndSendNotificationsAsync(new List<CreateNotificationDto> { notification });
        }

        public async Task SendWorkflowStageChangedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, string previousStage, string newStage, List<Guid> recipientUserIds)
        {
            var notifications = recipientUserIds.Select(userId => new CreateNotificationDto
            {
                UserId = userId,
                NotificationTypeName = "workflow_stage_changed",
                Title = "Workflow Stage Changed",
                Message = $"Document \"{documentTitle}\" has moved from {previousStage} to {newStage} stage",
                DocumentId = documentId,
                DocumentType = documentType,
                Priority = 2,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "previous_stage", previousStage },
                    { "new_stage", newStage }
                }
            }).ToList();

            await CreateAndSendNotificationsAsync(notifications);
        }

        public async Task SendFeedbackReceivedNotificationAsync(Guid documentId, string documentType, 
            string documentTitle, string feedbackType, Guid feedbackByUserId, Guid recipientUserId)
        {
            var feedbackByUser = await _unitOfWork.UserRepository.GetUserById(feedbackByUserId);
            var feedbackByName = feedbackByUser != null ? $"{feedbackByUser.first_name} {feedbackByUser.last_name}" : "Unknown User";

            var notification = new CreateNotificationDto
            {
                UserId = recipientUserId,
                NotificationTypeName = "feedback_received",
                Title = "Feedback Received",
                Message = $"You received feedback on \"{documentTitle}\" from {feedbackByName}",
                DocumentId = documentId,
                DocumentType = documentType,
                RelatedUserId = feedbackByUserId,
                Priority = 2,
                Metadata = new Dictionary<string, object>
                {
                    { "document_title", documentTitle },
                    { "feedback_by_name", feedbackByName },
                    { "feedback_type", feedbackType }
                }
            };

            await CreateAndSendNotificationsAsync(new List<CreateNotificationDto> { notification });
        }

        private async Task CreateAndSendNotificationsAsync(List<CreateNotificationDto> notifications)
        {
            try
            {
                _unitOfWork.Begin();

                foreach (var notificationDto in notifications)
                {
                    var notificationId = await _unitOfWork.NotificationRepository.CreateNotificationAsync(notificationDto);
                    
                    // Get the created notification for real-time sending
                    var notification = await _unitOfWork.NotificationRepository.GetNotificationByIdAsync(notificationId);
                    if (notification != null)
                    {
                        var realTimeNotification = new RealTimeNotificationDto
                        {
                            Id = notification.Id,
                            Title = notification.Title,
                            Message = notification.Message,
                            TypeName = notification.TypeName,
                            Icon = notification.Icon,
                            Color = notification.Color,
                            Priority = notification.Priority,
                            CreatedDate = notification.CreatedDate,
                            DocumentId = notification.DocumentId,
                            DocumentType = notification.DocumentType,
                            Metadata = notification.Metadata
                        };

                        // Send real-time notification
                        await _hubContext.SendNotificationToUser(notificationDto.UserId, realTimeNotification);
                        
                        // Update unread count
                        var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountAsync(notificationDto.UserId);
                        await _hubContext.SendUnreadCountUpdate(notificationDto.UserId, unreadCount);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
