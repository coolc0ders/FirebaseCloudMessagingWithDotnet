using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoolCodersDemos.Firebase.Notifications.Firebase.Models;

namespace CoolCodersDemos.Firebase.Notifications.Firebase
{
    public class FirebaseDeviceGroupService
    {
        private const string BaseFCMNotificationsURL = "https://fcm.googleapis.com/fcm/notification";
        private const string FirebaseSendMessageURL = "https://fcm.googleapis.com/fcm/send";
        private FirebaseConfig _firebaseConfig;

        public FirebaseDeviceGroupService(FirebaseConfig firebaseConfig)
        {
            _firebaseConfig = firebaseConfig;
        }

        private async Task<string> PostToFirebaseDeviceGroup(DeviceGroupRequestModel requestModel)
        {
            string body = JsonConvert.SerializeObject(requestModel);
            var respStr = await Post(body, BaseFCMNotificationsURL);
            return respStr;
        }

        private async Task<string> Post(string serializedBody, string url)
        {
            HttpClient httpClient;

            if (_firebaseConfig.UseHttpHandlerWithLegacySupport)
            {
                httpClient = new HttpClient(new WinHttpHandler());
            }
            else
            {
                httpClient = new HttpClient();
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "project_id", _firebaseConfig.SenderId }
                },
                Content = new StringContent(serializedBody, Encoding.UTF8, "application/json")
            };

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_firebaseConfig.APIKey}");
            var resp = await httpClient.SendAsync(requestMessage);
            string respStr = await resp.Content.ReadAsStringAsync();
            httpClient.Dispose();

            //Error handeling is done later, using the error property in the devicegroup model.

            return respStr;
        }

        private async Task<string> Get(Dictionary<string, string> requestParams, string url)
        {
            HttpClient httpClient;

            if (_firebaseConfig.UseHttpHandlerWithLegacySupport)
            {
                httpClient = new HttpClient(new WinHttpHandler());
            }
            else
            {
                httpClient = new HttpClient();
            }

            var urlEncoded = requestParams.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}");
            url = $"{url}?{string.Join("&", urlEncoded)}";

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "project_id", _firebaseConfig.SenderId },
                },
                ///NB: This get request must have this empty body, else an error is fired, stating it is not a json request
                ///Doc: https://firebase.google.com/docs/cloud-messaging/android/device-group#retrieving-a-notification-key
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            //Token might not be recognized above, so we add without any validation
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_firebaseConfig.APIKey}");

            var resp = await httpClient.SendAsync(requestMessage);
            string respStr = await resp.Content.ReadAsStringAsync();
            httpClient.Dispose();

            //Error handeling is done later, using the error property in the devicegroup model.

            return respStr;
        }

        public async Task<DeviceGroupResponse> GetDeviceGroupNotificationKey(string userId)
        {
            var result = await Get(new Dictionary<string, string> { { "notification_key_name", userId } }, BaseFCMNotificationsURL);

            var notificationKey = JsonConvert.DeserializeObject<DeviceGroupResponse>(result);
            return notificationKey;
        }

        public async Task<DeviceGroupResponse> AddTokenToDeviceGroup(string userId, string token,
            string notificationKey)
        {
            var result = await PerformOperation("add", userId, new List<string> { token }, notificationKey);

            var notificationKeyModel = JsonConvert.DeserializeObject<DeviceGroupResponse>(result);
            return notificationKeyModel;
        }

        public async Task<DeviceGroupResponse> CreateDeviceGroup(string userId, string token)
        {
            var result = await PerformOperation("create", userId, new List<string> { token }, string.Empty);

            var notificationKey = JsonConvert.DeserializeObject<DeviceGroupResponse>(result);
            return notificationKey;
        }

        private Task<string> PerformOperation(string operation, string userId, List<string> tokens,
            string notificationKey)
        {
            DeviceGroupRequestModel deviceGroup = new DeviceGroupRequestModel();
            deviceGroup.Operation = operation.ToLower();
            deviceGroup.NotificationKeyName = userId;
            deviceGroup.RegistrationIds.AddRange(tokens);
            deviceGroup.NotificationKey = notificationKey;

            return PostToFirebaseDeviceGroup(deviceGroup);
        }

        public async Task<DeviceGroupResponse> RemoveTokenFromDeviceGroup(string userId, List<string> tokens,
            string notificationKey)
        {
            var result = await PerformOperation("remove", userId, tokens, notificationKey);

            var deviceGroupResp = JsonConvert.DeserializeObject<DeviceGroupResponse>(result);
            return deviceGroupResp;
        }

        public async Task<SendDeviceMessageResponse> SendNotificationToDevices(DeviceMessage message)
        {
            var resultStr = await Post(JsonConvert.SerializeObject(message), FirebaseSendMessageURL);
            var result = JsonConvert.DeserializeObject<SendDeviceMessageResponse>(resultStr);

            if (result.FailureCount == 0 && result.SuccessCount == 0)
            {
                result.ResponseType = ResponseType.EmptyDeviceGroup;
            }
            else if (result.FailureCount > 0 && result.SuccessCount > 0)
            {
                result.ResponseType = ResponseType.PartialSuccess;
            }
            else if (result.FailureCount == 0 && result.SuccessCount > 0)
            {
                result.ResponseType = ResponseType.TotalSuccess;
            }
            else
            {
                result.ResponseType = ResponseType.TotalFailure;
            }

            //NOTE: As mentioned in the docs, when sending a message fails for a a token, 
            //retry sending the message to the token

            return result;
        }
    }
}
