using System.Text.Json;

namespace DotNetCore.APNNotificationAPI
{
    public static class NotificationSerializer
    {
        public static string Serialize(object notification)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var json = JsonSerializer.Serialize(notification, options);

            return json;
        }
    }
}
