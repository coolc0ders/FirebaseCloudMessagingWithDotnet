using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Firebase
{
    public class Constants
    {
        public const string DeviceGroupAlreadyExistsErrorCode = "Device_Group_Exists";
        public const string UnknownErrorCode = "Unknown";

        //This is to ensure that, notification data always contains someting
        public const string EmptyDataPayloadDataName = "Empty";
        public const string NotificationTypeDataName = "NotificationType";
        public const string URLDataName = "URL";
    }
}
