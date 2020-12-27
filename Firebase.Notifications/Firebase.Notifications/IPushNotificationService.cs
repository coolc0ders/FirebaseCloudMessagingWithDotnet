using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoolCodersDemos.Firebase.Notifications.Firebase.Models;

namespace CoolCodersDemos.Firebase.Notifications
{
    public interface IPushNotificationService
    {
        Task<IEnumerable<(int index, string reason)>> UnsubscribeFromTopic(string topic, List<string> tokens);
        /// <summary>
        /// Send message to a user, using his firebase token only
        /// </summary>
        /// <param name="token"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="imageUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SendNotificationToUserWithToken(string token, string title, string body, string imageUrl, Dictionary<string, string> data = null);
        /// <summary>
        /// Send message to a topic with several users
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="imageUrl"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SendNotificationToTopic(string topic, string title, string body, string imageUrl, Dictionary<string, string> data = null);
        /// <summary>
        /// Registers the user's token to his new device group
        /// </summary>
        /// <param name="userUniqueId">A unique identifier for the user's device group</param>
        /// <param name="firebaseToken">The firebase token for the device</param>
        /// <returns>A unic device token for this user</returns>
        Task<string> RegisterUserToDeviceGroup(string userUniqueId, string firebaseToken);
        /// <summary>
        /// When a new firebase token is passed gotten from the app, 
        /// Use this method to register it to the user's device group.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceGroupId"></param>
        /// <param name="firebaseToken"></param>
        /// <returns>Devicegroup Id</returns>
        Task<string> AddFirebaseTokenToDeviceGroup(string userId, string deviceGroupId, string firebaseToken);
        Task RemoveFirebaseTokensFromDeviceGroup(string userId, string deviceGroupId, List<string> firebaseTokens);
        Task<IEnumerable<(int index, string reason)>> SubscribeToTopic(string topic, List<string> tokens);
        Task<string> GetDeviceGroupIdForUser(string userId);
        Task<SendDeviceMessageResponse> SendNotificationToUserDevices(string deviceGroupId, string title, string body, string imageUrl, Dictionary<string, string> data = null);
    }
}
