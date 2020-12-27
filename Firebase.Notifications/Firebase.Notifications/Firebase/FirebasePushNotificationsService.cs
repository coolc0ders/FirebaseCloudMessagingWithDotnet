using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolCodersDemos.Firebase.Notifications.Exceptions;
using CoolCodersDemos.Firebase.Notifications.Firebase.Models;

namespace CoolCodersDemos.Firebase.Notifications.Firebase
{
    public class FirebasePushNotificationsService : IPushNotificationService
    {
        FirebaseConfig _config;
        FirebaseDeviceGroupService _deviceGroupService;

        public FirebasePushNotificationsService(FirebaseConfig config)
        {
            _config = config;
            _deviceGroupService = new FirebaseDeviceGroupService(_config);

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(_config.CredentialsJson)
            });
        }

        public async Task<IEnumerable<(int index, string reason)>> SubscribeToTopic(string topic, List<string> tokens)
        {
            var errors = new List<(int index, string reason)>();
            var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
                tokens, topic);

            if (response.FailureCount > 0)
            {
                errors.AddRange(response.Errors.Select(err => (err.Index, err.Reason)));
                var errorsString = response.Errors.Select(resp => $"### Subscription Error: {resp.Reason}");
                //don't block execution flow with an exception. Cause other users might need to receive notifications messages too.
            }

            return errors;
        }

        public async Task SendNotificationToUserWithToken(string token, string title, string body, string imageUrl, Dictionary<string, string> data = null)
        {
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(new Message  
            {
                Token = token,
                Notification = new Notification
                {
                    Body = body,
                    Title = title,
                    ImageUrl = imageUrl
                },
                Data = data,
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        ContentAvailable = true,
                        MutableContent = true,
                    }
                },
            });
        }

        public Task<string> SendNotificationToTopic(string topic, string title, string body, string imageUrl, Dictionary<string, string> data = null)
        {
            return FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Body = body,
                    Title = title,
                    ImageUrl = imageUrl
                },
                Data = data,
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        ContentAvailable = true,
                        MutableContent = true,
                    }
                },
            });
        }

        public async Task<string> RegisterUserToDeviceGroup(string userUniqueId, string firebaseToken)
        {
            //TODO: Handle this: 
            //{"error":"notification_key already exists"} (When you create a device group which already exists)
            var deviceGroup = await _deviceGroupService.CreateDeviceGroup(userUniqueId,
                firebaseToken);

            HandleErrorInDeviceGroup(deviceGroup, "Registering user device token");

            return deviceGroup.NotificationKey;
        }

        public async Task<string> AddFirebaseTokenToDeviceGroup(string userId, string deviceGroupId, string firebaseToken)
        {
            var newDeviceGroup = await _deviceGroupService.AddTokenToDeviceGroup(userId,
                deviceGroupId, firebaseToken);

            HandleErrorInDeviceGroup(newDeviceGroup, "Adding firebase token to devicegroup", deviceGroupId);

            return newDeviceGroup.NotificationKey;
        }

        private void HandleErrorInDeviceGroup(DeviceGroupResponse deviceGroupResponse, string operation, string deviceGroupId = "")
        {
            if (!string.IsNullOrWhiteSpace(deviceGroupId))
                deviceGroupResponse.NotificationKey = deviceGroupId;
            if (deviceGroupResponse.IsSuccess)
                return;

            var errorMessage = $"An error occured while {operation} the device group with ID: {deviceGroupResponse.NotificationKey}." +
                $" ###Message: {deviceGroupResponse.Error}";

            var errorCode = deviceGroupResponse.ErrorType == ErrorType.DeviceGroupAlreadyExists ? Constants.DeviceGroupAlreadyExistsErrorCode : Constants.UnknownErrorCode;
            throw new NotificationsServiceException(errorMessage, errorCode);
        }

        public async Task<string> GetDeviceGroupIdForUser(string userId)
        {
            var deviceGroup = await _deviceGroupService.GetDeviceGroupNotificationKey(userId);

            HandleErrorInDeviceGroup(deviceGroup, "Getting device group for user");

            return deviceGroup.NotificationKey;
        }

        public Task<SendDeviceMessageResponse> SendNotificationToUserDevices(string deviceGroupId, string title, string body, string imageUrl, Dictionary<string, string> data = null)
        {
            return _deviceGroupService.SendNotificationToDevices(new DeviceMessage
            {
                Data = data,
                To = deviceGroupId,
                Notification = new Notification
                {
                    Body = body,
                    Title = title,
                    ImageUrl = imageUrl
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        ContentAvailable = true,
                        MutableContent = true,
                    }
                },
            });
        }

        public async Task<IEnumerable<(int index, string reason)>> UnsubscribeFromTopic(string topic, List<string> tokens)
        {
            var errors = new List<(int index, string reason)>();
            var response = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(
                tokens, topic);

            if (response.FailureCount > 0)
            {
                errors.AddRange(response.Errors.Select(err => (err.Index, err.Reason)));
                var errorsString = response.Errors.Select(resp => $"### Subscription Error: {resp.Reason}");
                //don't block execution flow with an exception. Cause other users might need to receive notifications messages too.
            }

            return errors;
        }

        public async Task RemoveFirebaseTokensFromDeviceGroup(string userId, string deviceGroupId, List<string> firebaseTokens)
        {
            var response = await _deviceGroupService.RemoveTokenFromDeviceGroup(userId, firebaseTokens, deviceGroupId);

            HandleErrorInDeviceGroup(response, "Removing firebase token from device group");
        }
    }
}
