using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace HexedProxy.HexedServer
{
    internal class ServerHandler
    {
        private static string Token;
        private static string ServerThumbprint;
        private static string EncryptionKey;
        private static string DecryptionKey;

        // CLIENT SIDE VALIDATION
        private static bool ValidateServerCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return certificate.Thumbprint == ServerThumbprint;
        }

        public static bool Init(string Key)
        {
            Token = Key;

            ServerThumbprint = EncryptUtils.FromBase64(FetchCert().Result);
            EncryptionKey = EncryptUtils.FromBase64(FetchEncryptionKey().Result);
            DecryptionKey = EncryptUtils.FromBase64(FetchDecryptionKey().Result);

            if (!IsValidToken().Result) return false;

            string EncodedImguiLib = DownloadAsset("cimgui.dll").Result;
            File.WriteAllBytes("cimgui.dll", Convert.FromBase64String(EncodedImguiLib));

            string EncodedUEMap = DownloadAsset("DBDMap.usmap").Result;
            File.WriteAllBytes("DBDMap.usmap", Convert.FromBase64String(EncodedUEMap));

            return true;
        }

        private static async Task<string> FetchCert()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/Certificate");
            HttpResponseMessage Response = await Client.SendAsync(Payload);
            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }

        private static async Task<string> FetchEncryptionKey()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/EncryptKey");
            HttpResponseMessage Response = await Client.SendAsync(Payload);
            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }

        private static async Task<string> FetchDecryptionKey()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/DecryptKey");
            HttpResponseMessage Response = await Client.SendAsync(Payload);
            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }

        public static async Task<bool> IsValidToken()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Post, "https://api.logout.rip/Server/IsValidToken")
            {
                Content = new StringContent(DataEncryptBase.EncryptData(JsonSerializer.Serialize(new { Key = Token, HWID = SerialHandler.GetHWID(), ServerTime = EncryptUtils.GetUnixTime() }), EncryptionKey), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            return Response.IsSuccessStatusCode;
        }

        public static async Task<string> DownloadAsset(string Asset)
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, $"https://api.logout.rip/Server/GetAsset/{Asset}");
            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }
    }
}