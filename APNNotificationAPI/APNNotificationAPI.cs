using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotNetCore.APNNotificationAPI
{
    public class APNNotificationAPI : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly HttpClientHandler clientHandler;

        public APNNotificationAPI(string baseUrl, string certificatePath)
        {
            var clientCertificate = new X509Certificate2(File.ReadAllBytes(certificatePath), "VMar2016Mbk");

            clientHandler = new HttpClientHandler();
            clientHandler.ClientCertificates.Add(clientCertificate);
            clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            clientHandler.SslProtocols = SslProtocols.Tls12;

            httpClient = new HttpClient(clientHandler)
            {
                // APN only works with HTTP/2
                DefaultRequestVersion = new Version(2, 0),
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task SendAsync(object notification, string deviceToken, string topic = null)
        {
            HttpResponseMessage response = null;
            try
            {
                var requestUri = $"/3/device/{deviceToken}";

                var message = NotificationSerializer.Serialize(notification);
                
                var content = new StringContent(message);
                content.Headers.Add("apns-id", Guid.NewGuid().ToString());
                content.Headers.Add("apns-expiration", "0"); // notification will only deliver once, and be dropped if not could be delivered
                content.Headers.Add("apns-priority", "10"); // priorities are between 10-0, 10 = highest

                if (topic != null)
                    content.Headers.Add("apns-topic", topic);

                response = await httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();
            }
            catch(Exception)
            {
                throw await DeserializeErrorMessage(response.Content);
            }
        }

        public void Dispose()
        {
            clientHandler.Dispose();
            httpClient.Dispose();
        }

        private async Task<Exception> DeserializeErrorMessage(HttpContent httpContent)
        {
            var jsonResponse = await httpContent.ReadAsStringAsync();

            using var errorMessage = JsonDocument.Parse(jsonResponse);

            var root = errorMessage.RootElement;
            var reason = root.GetProperty("reason").GetString();

            return new APIException(reason);
        }
    }
}