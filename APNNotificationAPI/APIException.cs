using System;

namespace DotNetCore.APNNotificationAPI
{
    public class APIException : Exception
    {
        public APIException(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; private set; }
    }
}