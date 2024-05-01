using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace HexedProxy.HexedServer
{
    internal class ServerHandler
    {
        private static string Token;

        public static bool Init(string Key)
        {
            Token = Key;

            Encryption.ServerThumbprint = EncryptUtils.FromBase64(FetchCert().Result);
            Encryption.EncryptionKey = EncryptUtils.FromBase64(FetchEncryptionKey().Result);
            Encryption.DecryptionKey = EncryptUtils.FromBase64(FetchDecryptionKey().Result);

            if (!IsValidToken().Result) return false;

            string EncodedAsset = DownloadAsset("cimgui.dll").Result;
            File.WriteAllBytes("cimgui.dll", Convert.FromBase64String(EncodedAsset));

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
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/EncryptKey");
            HttpResponseMessage Response = await Client.SendAsync(Payload);
            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }

        private static async Task<string> FetchDecryptionKey()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/DecryptKey");
            HttpResponseMessage Response = await Client.SendAsync(Payload);
            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }

        public static async Task<bool> IsValidToken()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Post, "https://api.logout.rip/Server/IsValidToken")
            {
                Content = new StringContent(DataEncryptBase.EncryptData(JsonSerializer.Serialize(new { Key = Token, HWID = Encryption.GetHWID(), ServerTime = EncryptUtils.GetUnixTime() }), Encryption.EncryptionKey), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            return Response.IsSuccessStatusCode;
        }

        public static async Task<string> DownloadAsset(string Asset)
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, $"https://api.logout.rip/Server/GetAsset/{Asset}");
            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            return null;
        }
    }
}