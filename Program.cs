using DotNetCore.APNNotificationAPI;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        private static string BASE_URL = "https://api.push.apple.com";
        private static string BASE_URL_DEVELOPMENT = "https://api.development.push.apple.com";

        // Link this to your certificate
        // How to get the certificate? Check https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CommunicatingwithAPNs.html#//apple_ref/doc/uid/TP40008194-CH11-SW1
        private static string CERTIFICATE_PATH = @"<path>\certificate.p12";

        static async Task Main(string[] args)
        {
            try
            {
                var notification = new
                {
                    Aps = new
                    {
                        Alert = "Hello!"
                    }
                };

                using var api = new APNNotificationAPI(BASE_URL, CERTIFICATE_PATH);

                Console.WriteLine($"Sending notification: {Environment.NewLine}{NotificationSerializer.Serialize(notification)}");

                // change this to your device token
                var deviceToken = "420e6134164a7d3192d97ca5186f79a479725f8045e17c034930076dc8a4e8e1";

                await api.SendAsync(notification, deviceToken);

                Console.WriteLine("Send succeeded");
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Sending notification failed with reason: '{ex.Reason}'");
            }
        }
    }
}