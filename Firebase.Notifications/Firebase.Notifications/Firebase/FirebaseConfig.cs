using System;
using System.Collections.Generic;
using System.Text;

namespace CoolCodersDemos.Firebase.Notifications.Firebase
{
    public class FirebaseConfig
    {
        public string CredentialsJson { get; set; }
        /// <summary>
        /// The project's sender ID found at : Firbase Console > Settings > Cloud Messaging Tab
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// The server's API key, found at the same location as the the sender Id above
        /// </summary>
        public string APIKey { get; set; }
        /// <summary>
        /// Use an http handler which permits the API to make requests
        /// Prohibited by old dotnet apis, like a get request with body
        /// </summary>
        public bool UseHttpHandlerWithLegacySupport { get; set; }
    }
}
