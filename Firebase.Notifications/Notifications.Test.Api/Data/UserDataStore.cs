using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationsBackend.Data
{
    public class UserDataStore
    {
        public Dictionary<string, (string token, string platform)> TokenStore { get; set; }

        public UserDataStore()
        {
            TokenStore = new Dictionary<string, (string token, string platform)>();
        }
    }
}
