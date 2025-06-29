using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkflowMgmt.Application.Features.Notifications;
using WorkflowMgmt.Domain.Models.Notifications;

namespace WorkflowMgmt.WebAPI.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    public class NotificationController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool unreadOnly = false,
            [FromQuery] string? documentType = null,
            [FromQuery] int? priority = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetUserNotificationsQuery
            {
                UserId = userId.Value,
                Page = page,
                PageSize = pageSize,
                UnreadOnly = unreadOnly,
                DocumentType = documentType,
                Priority = priority,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetNotificationByIdQuery
            {
                NotificationId = id,
                UserId = userId.Value
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetUnreadNotificationCountQuery { UserId = userId.Value };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetNotificationSummaryQuery { UserId = userId.Value };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentNotifications([FromQuery] int count = 5)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetRecentNotificationsQuery 
            { 
                UserId = userId.Value,
                Count = count
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetNotificationTypes()
        {
            var query = new GetNotificationTypesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("preferences")]
        public async Task<IActionResult> GetNotificationPreferences()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetUserNotificationPreferencesQuery { UserId = userId.Value };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("counts-by-type")]
        public async Task<IActionResult> GetNotificationCountsByType()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetNotificationCountsByTypeQuery { UserId = userId.Value };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("document/{documentId}")]
        public async Task<IActionResult> GetDocumentNotifications(Guid documentId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var query = new GetDocumentNotificationsQuery 
            { 
                DocumentId = documentId,
                UserId = userId.Value
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulkNotifications([FromBody] CreateBulkNotificationsCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = id,
                UserId = userId.Value
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var command = new MarkAllNotificationsAsReadCommand { UserId = userId.Value };
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var command = new DeleteNotificationCommand
            {
                NotificationId = id,
                UserId = userId.Value
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("read")]
        public async Task<IActionResult> DeleteAllReadNotifications()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var command = new DeleteAllReadNotificationsCommand { UserId = userId.Value };
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("preferences")]
        public async Task<IActionResult> UpdateNotificationPreference([FromBody] UpdateNotificationPreferenceDto preference)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var command = new UpdateNotificationPreferenceCommand
            {
                UserId = userId.Value,
                Preference = preference
            };

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetNotificationStatistics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var userId = GetCurrentUserId();
            
            var query = new GetNotificationStatisticsQuery
            {
                UserId = userId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
}
