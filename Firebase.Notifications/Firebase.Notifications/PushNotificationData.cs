using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications
{
    public class PushNotificationData
    {
        /// <summary>
        /// The push notification type (message, notification...)
        /// </summary>
        public PushNotificationType PushNotificationType { get; set; }
        public object Data { get; set; }
    }
}
