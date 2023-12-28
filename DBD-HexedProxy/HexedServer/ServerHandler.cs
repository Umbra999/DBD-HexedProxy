using HexedProxy.HexedServer;
using HexedProxy.Wrappers;

namespace HexedServer
{
    internal class ServerHandler
    {
        public static void Init()
        {
            Encryption.ServerThumbprint = Utils.FromBase64(FetchCert().Result);
            string EncodedAsset = DownloadAsset("cimgui.dll").Result;
            File.WriteAllBytes("cimgui.dll", Convert.FromBase64String(EncodedAsset));
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