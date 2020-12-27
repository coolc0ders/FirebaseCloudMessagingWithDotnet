using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Firebase
{
    public class DeviceGroupRequestModel
    {
        [JsonProperty(PropertyName = "operation", NullValueHandling = NullValueHandling.Ignore)]
        public string Operation { get; set; }
        [JsonProperty(PropertyName = "notification_key", NullValueHandling = NullValueHandling.Ignore)]
        public string NotificationKey { get; set; }
        [JsonProperty(PropertyName = "notification_key_name", NullValueHandling = NullValueHandling.Ignore)]
        public string NotificationKeyName { get; set; }
        [JsonProperty(PropertyName = "registration_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RegistrationIds { get; set; }

        public DeviceGroupRequestModel()
        {
            RegistrationIds = new List<string>();
        }
    }
}

