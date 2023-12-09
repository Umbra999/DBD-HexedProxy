using HexedProxy.HexedServer;
using HexedProxy.Wrappers;
using Newtonsoft.Json;
using System.Text;

namespace HexedServer
{
    internal class ServerHandler
    {
        private static ServerObjects.UserData UserData;

        public static void Init()
        {
            Logger.Log("Authenticating...");

            if (!File.Exists("Key.Hexed"))
            {
                Logger.LogError("No Key provided");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            Encryption.ServerThumbprint = EncryptUtils.FromBase64(FetchCert().Result);
            Encryption.EncryptionKey = EncryptUtils.FromBase64(FetchEncryptionKey().Result);
            Encryption.DecryptionKey = EncryptUtils.FromBase64(FetchDecryptionKey().Result);

            UserData = Login(EncryptUtils.FromBase64(File.ReadAllText("Key.Hexed"))).Result;

            if (UserData == null || !UserData.KeyAccess.Contains(ServerObjects.KeyPermissionType.DeadByDaylightUnlocker))
            {
                Logger.LogError("Key is not Valid");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            Logger.Log($"Authenticated as {UserData.Username}");

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

        private static async Task<ServerObjects.UserData> Login(string Key)
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Post, "https://api.logout.rip/Server/Login")
            {
                Content = new StringContent(DataEncryptBase.EncryptData(JsonConvert.SerializeObject(new { Key = Key, HWID = Encryption.GetHWID(), ServerTime = EncryptUtils.GetUnixTime() }), Encryption.EncryptionKey), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string EncryptedData = await Response.Content.ReadAsStringAsync();
                string RawData = DataEncryptBase.DecryptData(EncryptedData, Encryption.DecryptionKey);
                return JsonConvert.DeserializeObject<ServerObjects.UserData>(RawData);
            }

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