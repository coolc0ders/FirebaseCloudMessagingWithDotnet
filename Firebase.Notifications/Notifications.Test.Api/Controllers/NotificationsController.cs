using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationsBackend.Data;
using CoolCodersDemos.Firebase.Notifications;
using CoolCodersDemos.Firebase.Notifications.Exceptions;

namespace NotificationsBackend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NotificationsController : Controller
    {
        ILogger<NotificationsController> _logger;
        UserDataStore _userIdTokenStore;
        IPushNotificationService _pushNotificationsService;

        public NotificationsController(ILogger<NotificationsController> logger,
            IPushNotificationService pushNotificationService, 
            UserDataStore userDataStore)
        {
            _userIdTokenStore = userDataStore;
            _logger = logger;
            _pushNotificationsService = pushNotificationService;
        }

        [HttpPost()]
        public IActionResult RegisterUserToken([FromBody] RegisterUserTokenRequest tokenRequest)
        {
            if (!_userIdTokenStore.TokenStore.ContainsKey(tokenRequest.UserId))
                _userIdTokenStore.TokenStore.Add(tokenRequest.UserId, (tokenRequest.UserToken, tokenRequest.Platform));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterToTopic([FromBody] RegisterUserTokenRequest requestBody)
        {
            var topicName = "AllUsersTopicTest";
            var res = await _pushNotificationsService.SubscribeToTopic(topicName, new List<string>
            {
                requestBody.UserToken
            });

            if (res != null && res.Any())
                return StatusCode(500, JsonConvert.SerializeObject(res));

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToTopic([FromBody] NotificationRequest notif)
        {
            var topicName = "AllUsersTopicTest";
            var res = await _pushNotificationsService.SendNotificationToTopic(topicName, notif.NotificationTitle, 
                notif.NotificationContent, null, new Dictionary<string, string>
                {
                    {
                        CoolCodersDemos.Firebase.Notifications.Firebase.Constants.EmptyDataPayloadDataName,
                        CoolCodersDemos.Firebase.Notifications.Firebase.Constants.EmptyDataPayloadDataName
                    }
                });

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToDeviceGroup([FromBody] NotificationRequest notif)
        {
            var res = await _pushNotificationsService.SendNotificationToUserDevices(notif.DeviceGroupId,
                notif.NotificationTitle, notif.NotificationContent, "");

            if (res.ResponseType != CoolCodersDemos.Firebase.Notifications.Firebase.Models.ResponseType.TotalSuccess)
                return StatusCode(500, JsonConvert.SerializeObject(res));

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUserTokenToDeviceGroup([FromBody] RegisterUserTokenRequest tokenRequest)
        {
            if (!_userIdTokenStore.TokenStore.ContainsKey(tokenRequest.UserId))
            {
                _userIdTokenStore.TokenStore.Add(tokenRequest.UserId, (tokenRequest.UserToken, tokenRequest.Platform));
            }

            try
            {
                var deviceGroupId = await _pushNotificationsService.RegisterUserToDeviceGroup(tokenRequest.UserId, tokenRequest.UserToken);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NotificationsServiceException))
                {
                    ;
                }

                ;
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetDeviceGroupIdForUser([FromQuery] string userId)
        {
            try
            {
                var deviceGroupId = await _pushNotificationsService.GetDeviceGroupIdForUser(userId);
                return Ok(deviceGroupId);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NotificationsServiceException))
                {
                    ;
                }

                ;
            }

            return Ok();
        }

        /*
        [HttpPost()]
        public IActionResult SendNotification([FromBody] NotificationRequest notification)
        {
            IEnumerable<(string token, string platform, string userId)> tokens = default;

            if (notification.UserIds != null)
                tokens = _userIdTokenStore.TokenStore.Where(kv => notification.UserIds.Contains(kv.Key)).Select(kv => (kv.Value.token, kv.Value.platform, kv.Key));
            else
                tokens = _userIdTokenStore.TokenStore.Select(kv => (kv.Value.token, kv.Value.platform, kv.Key));

            _pushNotificationsService.SendPushNotifications(tokens.ToList(), notification.NotificationTitle, notification.NotificationContent);
            return Ok();
        }
        */
    }

    public class RegisterUserTokenRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserToken { get; set; }
        public string Platform { get; set; }
    }

    public class NotificationRequest
    {
        [Required]
        public string NotificationTitle { get; set; }
        [Required]
        public string NotificationContent { get; set; }
        public List<string> UserIds { get; set; }
        public string DeviceGroupId { get; set; }
    }
}
