using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WorkflowMgmt.Application.Features.Notifications;

namespace WorkflowMgmt.Application.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(IMediator mediator, ILogger<NotificationHub> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                // Get user's department and add to department group
                var userDepartment = GetUserDepartment();
                if (userDepartment != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Department_{userDepartment}");
                }

                // Get user's role and add to role group
                var userRole = GetUserRole();
                if (userRole != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{userRole}");
                }

                _logger.LogInformation($"User {userId} connected to NotificationHub with connection {Context.ConnectionId}");

                // Send unread notification count
                await SendUnreadNotificationCount(userId.Value);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                _logger.LogInformation($"User {userId} disconnected from NotificationHub");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Client can call this to join specific groups
        public async Task JoinDocumentGroup(string documentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Document_{documentId}");
        }

        public async Task LeaveDocumentGroup(string documentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Document_{documentId}");
        }

        // Mark notification as read
        public async Task MarkNotificationAsRead(string notificationId)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var command = new MarkNotificationAsReadCommand
                {
                    NotificationId = Guid.Parse(notificationId),
                    UserId = userId.Value
                };

                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    // Send updated unread count
                    await SendUnreadNotificationCount(userId.Value);
                }
            }
        }

        // Mark all notifications as read
        public async Task MarkAllNotificationsAsRead()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var command = new MarkAllNotificationsAsReadCommand
                {
                    UserId = userId.Value
                };

                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    // Send updated unread count (should be 0)
                    await SendUnreadNotificationCount(userId.Value);
                }
            }
        }

        // Get notifications for current user
        public async Task GetNotifications(int page = 1, int pageSize = 20, bool unreadOnly = false)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var query = new GetUserNotificationsQuery
                {
                    UserId = userId.Value,
                    Page = page,
                    PageSize = pageSize,
                    UnreadOnly = unreadOnly
                };

                var result = await _mediator.Send(query);
                await Clients.Caller.SendAsync("NotificationsReceived", result);
            }
        }

        // Send notification to specific user
        public async Task SendNotificationToUser(Guid userId, object notification)
        {
            await Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification);
        }

        // Send notification to department
        public async Task SendNotificationToDepartment(int departmentId, object notification)
        {
            await Clients.Group($"Department_{departmentId}").SendAsync("ReceiveNotification", notification);
        }

        // Send notification to role
        public async Task SendNotificationToRole(string role, object notification)
        {
            await Clients.Group($"Role_{role}").SendAsync("ReceiveNotification", notification);
        }

        // Send notification to document watchers
        public async Task SendNotificationToDocumentWatchers(string documentId, object notification)
        {
            await Clients.Group($"Document_{documentId}").SendAsync("ReceiveNotification", notification);
        }

        private async Task SendUnreadNotificationCount(Guid userId)
        {
            var query = new GetUnreadNotificationCountQuery { UserId = userId };
            var result = await _mediator.Send(query);

            await Clients.Group($"User_{userId}").SendAsync("UnreadNotificationCount", result.Data);
        }

        private Guid? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }

        private int? GetUserDepartment()
        {
            var departmentClaim = Context.User?.FindFirst("department_id")?.Value;
            if (int.TryParse(departmentClaim, out var departmentId))
            {
                return departmentId;
            }
            return null;
        }

        private string? GetUserRole()
        {
            return Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }

    // Extension methods for sending notifications from other parts of the application
    public static class NotificationHubExtensions
    {
        public static async Task SendNotificationToUser(this IHubContext<NotificationHub> hubContext,
            Guid userId, object notification)
        {
            await hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification);
        }

        public static async Task SendNotificationToDepartment(this IHubContext<NotificationHub> hubContext,
            int departmentId, object notification)
        {
            await hubContext.Clients.Group($"Department_{departmentId}").SendAsync("ReceiveNotification", notification);
        }

        public static async Task SendNotificationToRole(this IHubContext<NotificationHub> hubContext,
            string role, object notification)
        {
            await hubContext.Clients.Group($"Role_{role}").SendAsync("ReceiveNotification", notification);
        }

        public static async Task SendUnreadCountUpdate(this IHubContext<NotificationHub> hubContext,
            Guid userId, int unreadCount)
        {
            await hubContext.Clients.Group($"User_{userId}").SendAsync("UnreadNotificationCount", unreadCount);
        }

        public static async Task SendDocumentNotification(this IHubContext<NotificationHub> hubContext,
            string documentId, object notification)
        {
            await hubContext.Clients.Group($"Document_{documentId}").SendAsync("ReceiveNotification", notification);
        }
    }
}
