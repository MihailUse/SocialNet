using API.Extentions;
using API.Models.Notification;
using API.Services;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = SwaggerDefinitionNames.Api)]
    public class NotificationController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public NotificationController(UserService userService, NotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IEnumerable<NotificationModel> GetNotifications(int skip = 0, int take = 20, DateTimeOffset? fromTime = null)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            return _notificationService.GetNotifications(userId, skip, take, fromTime ?? DateTimeOffset.UtcNow);
        }

        [HttpPost]
        public async Task<List<string>> SendNotification(SendNotificationModel sendNotificationModel)
        {
            Guid userId = sendNotificationModel.UserId ?? User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            List<string> messages = new List<string>();

            string? token = await _userService.GetNotificationToken(userId);
            if (token != null)
                messages = _notificationService.SendNotification(token, sendNotificationModel.Notification);

            return messages;
        }

        [HttpPost]
        public async Task Subscribe(NotificationTokenModel pushTokenModel)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.SetNotificationToken(userId, pushTokenModel.Token);
        }

        [HttpDelete]
        public async Task Unsubscribe()
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _userService.SetNotificationToken(userId, null);
        }

        [HttpDelete]
        public async Task DeleteNotification(Guid notificationId)
        {
            Guid userId = User.GetClaimValue<Guid>(TokenClaimTypes.UserId);
            await _notificationService.DeleteNotification(userId, notificationId);
        }
    }
}
