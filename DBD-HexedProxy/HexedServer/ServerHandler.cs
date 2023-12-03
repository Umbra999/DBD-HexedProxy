using HexedProxy.Wrappers;
using Newtonsoft.Json;
using System.Text;

namespace HexedServer
{
    internal class ServerHandler
    {
        private static ServerObjects.UserData UserData;

        public static async Task Init()
        {
            Logger.Log("Authenticating...");
            if (!File.Exists("Key.Hexed"))
            {
                Logger.LogWarning("Enter Key:");
                string NewKey = Console.ReadLine();
                File.WriteAllText("Key.Hexed", Encryption.ToBase64(NewKey));
            }

            Encryption.ServerThumbprint = Encryption.FromBase64(await FetchCert());
            Encryption.PublicEncryptionKey = Encryption.FromBase64(await FetchPublicKey());

            UserData = await Login(Encryption.FromBase64(File.ReadAllText("Key.Hexed")));

            if (UserData == null || !UserData.KeyAccess.Contains(ServerObjects.KeyPermissionType.DeadByDaylightUnlocker))
            {
                Logger.LogError("Key is not Valid");
                await Task.Delay(3000);
                Environment.Exit(0);
            }

            string EncodedAsset = await DownloadAsset("cimgui.dll");
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

        private static async Task<string> FetchPublicKey()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/PublicKey");
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
                Content = new StringContent(Encryption.EncryptData(JsonConvert.SerializeObject(new { Key = Key, HWID = Encryption.GetHWID(), ServerTime = Encryption.GetUnixTime() })), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string RawData = await Response.Content.ReadAsStringAsync();
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