using MediatR;
using Microsoft.AspNetCore.SignalR;
using WorkflowMgmt.Application.Hubs;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Notifications;

namespace WorkflowMgmt.Application.Features.Notifications
{
    // Command Handlers
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CreateNotificationCommandHandler(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var createDto = new CreateNotificationDto
                {
                    UserId = request.UserId,
                    NotificationTypeName = request.NotificationTypeName,
                    Title = request.Title,
                    Message = request.Message,
                    DocumentId = request.DocumentId,
                    DocumentType = request.DocumentType,
                    RelatedUserId = request.RelatedUserId,
                    Metadata = request.Metadata,
                    Priority = request.Priority,
                    ExpiresAt = request.ExpiresAt
                };

                var notificationId = await _unitOfWork.NotificationRepository.CreateNotificationAsync(createDto);
                await _unitOfWork.CommitAsync();

                // Send real-time notification
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

                    await _hubContext.SendNotificationToUser(request.UserId, realTimeNotification);
                    
                    // Update unread count
                    var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountAsync(request.UserId);
                    await _hubContext.SendUnreadCountUpdate(request.UserId, unreadCount);
                }

                return ApiResponse<Guid>.SuccessResponse(notificationId, "Notification created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<Guid>.ErrorResponse($"Error creating notification: {ex.Message}");
            }
        }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.NotificationRepository.MarkNotificationAsReadAsync(request.NotificationId, request.UserId);
                await _unitOfWork.CommitAsync();

                if (result)
                {
                    // Update unread count
                    var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountAsync(request.UserId);
                    await _hubContext.SendUnreadCountUpdate(request.UserId, unreadCount);
                }

                return ApiResponse<bool>.SuccessResponse(result, result ? "Notification marked as read" : "Notification not found or already read");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<bool>.ErrorResponse($"Error marking notification as read: {ex.Message}");
            }
        }
    }

    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MarkAllNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<bool>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.NotificationRepository.MarkAllNotificationsAsReadAsync(request.UserId);
                await _unitOfWork.CommitAsync();

                if (result)
                {
                    // Send updated unread count (should be 0)
                    await _hubContext.SendUnreadCountUpdate(request.UserId, 0);
                }

                return ApiResponse<bool>.SuccessResponse(result, "All notifications marked as read");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<bool>.ErrorResponse($"Error marking all notifications as read: {ex.Message}");
            }
        }
    }

    // Query Handlers
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, ApiResponse<PaginatedNotificationsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PaginatedNotificationsDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var requestDto = new GetUserNotificationsRequest
                {
                    UserId = request.UserId,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    UnreadOnly = request.UnreadOnly,
                    DocumentType = request.DocumentType,
                    Priority = request.Priority,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate
                };

                var result = await _unitOfWork.NotificationRepository.GetUserNotificationsAsync(requestDto);
                return ApiResponse<PaginatedNotificationsDto>.SuccessResponse(result, "Notifications retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<PaginatedNotificationsDto>.ErrorResponse($"Error retrieving notifications: {ex.Message}");
            }
        }
    }

    public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, ApiResponse<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUnreadNotificationCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var count = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountAsync(request.UserId);
                return ApiResponse<int>.SuccessResponse(count, "Unread notification count retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error retrieving unread notification count: {ex.Message}");
            }
        }
    }

    public class GetNotificationSummaryQueryHandler : IRequestHandler<GetNotificationSummaryQuery, ApiResponse<NotificationSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetNotificationSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<NotificationSummaryDto>> Handle(GetNotificationSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var summary = await _unitOfWork.NotificationRepository.GetNotificationSummaryAsync(request.UserId);
                return ApiResponse<NotificationSummaryDto>.SuccessResponse(summary, "Notification summary retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<NotificationSummaryDto>.ErrorResponse($"Error retrieving notification summary: {ex.Message}");
            }
        }
    }

    public class GetRecentNotificationsQueryHandler : IRequestHandler<GetRecentNotificationsQuery, ApiResponse<List<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentNotificationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<NotificationDto>>> Handle(GetRecentNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var notifications = await _unitOfWork.NotificationRepository.GetRecentNotificationsAsync(request.UserId, request.Count);
                return ApiResponse<List<NotificationDto>>.SuccessResponse(notifications, "Recent notifications retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NotificationDto>>.ErrorResponse($"Error retrieving recent notifications: {ex.Message}");
            }
        }
    }

    public class GetNotificationTypesQueryHandler : IRequestHandler<GetNotificationTypesQuery, ApiResponse<List<NotificationTypeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetNotificationTypesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<NotificationTypeDto>>> Handle(GetNotificationTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var types = await _unitOfWork.NotificationRepository.GetNotificationTypesAsync();
                return ApiResponse<List<NotificationTypeDto>>.SuccessResponse(types, "Notification types retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NotificationTypeDto>>.ErrorResponse($"Error retrieving notification types: {ex.Message}");
            }
        }
    }

    public class GetUserNotificationPreferencesQueryHandler : IRequestHandler<GetUserNotificationPreferencesQuery, ApiResponse<List<NotificationPreferenceDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserNotificationPreferencesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<NotificationPreferenceDto>>> Handle(GetUserNotificationPreferencesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var preferences = await _unitOfWork.NotificationRepository.GetUserNotificationPreferencesAsync(request.UserId);
                return ApiResponse<List<NotificationPreferenceDto>>.SuccessResponse(preferences, "User notification preferences retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NotificationPreferenceDto>>.ErrorResponse($"Error retrieving user notification preferences: {ex.Message}");
            }
        }
    }
}
