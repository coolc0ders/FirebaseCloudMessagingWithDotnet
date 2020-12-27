using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Exceptions
{
    public class NotificationsServiceException : Exception
    {
        public string ErrorCode { get; private set; }

        public NotificationsServiceException(string message, string code): base(message)
        {
            ErrorCode = code;
        }
    }
}
