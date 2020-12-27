using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Firebase.Models
{
    public class DeviceMessage
    {
        [JsonProperty(PropertyName = "to", NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; set; }
        [JsonProperty(PropertyName = "notification", NullValueHandling = NullValueHandling.Ignore)]
        public Notification Notification { get; set; }
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Data { get; set; }
        [JsonProperty(PropertyName = "apns", NullValueHandling = NullValueHandling.Ignore)]
        public ApnsConfig Apns { get; set; }
    }
}
