using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Firebase.Models
{
    public class DeviceGroupResponse
    {
        [JsonProperty("notification_key")]
        public string NotificationKey { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        public bool IsSuccess => string.IsNullOrWhiteSpace(Error);
        public ErrorType ErrorType
        {
            get => Error switch
            {
                "notification_key already exists" => ErrorType.DeviceGroupAlreadyExists,
                "notification_key not found" => ErrorType.NotificationKeyNotFound,
                _ => ErrorType.None
            };
        }
    }

    public enum ErrorType
    {
        None,
        DeviceGroupAlreadyExists,
        NotificationKeyNotFound
    }
}
