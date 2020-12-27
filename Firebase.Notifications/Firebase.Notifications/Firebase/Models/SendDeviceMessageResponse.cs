using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Firebase.Models
{
    public class SendDeviceMessageResponse
    {
        [JsonProperty("success")]
        public int SuccessCount { get; set; }
        [JsonProperty("failure")]
        public int FailureCount { get; set; }
        /// <summary>
        /// The firebase tokens to which the notification could not be sent
        /// </summary>
        [JsonProperty("failed_registration_ids")]
        public List<string> FailedRegistrationTokenIds { get; set; }
        public ResponseType ResponseType { get; set; }

        public SendDeviceMessageResponse()
        {
            FailedRegistrationTokenIds = new List<string>();
        }
    }

    public enum ResponseType
    {
        TotalSuccess,//The message was sent to every device successfully
        PartialSuccess,//The message failed in some devices
        EmptyDeviceGroup,//The device group we are trying to send the message to, is empty
        TotalFailure
    }
}
