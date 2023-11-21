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

            Encryption.ServerThumbprint = await FetchCert();
            UserData = await Login(Encryption.FromBase64(File.ReadAllText("Key.Hexed")));

            if (UserData == null || !UserData.KeyAccess.Contains(ServerObjects.KeyPermissionType.VRChatBot))
            {
                Logger.LogError("Key is not Valid");
                await Task.Delay(3000);
                Environment.Exit(0);
            }
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

        private static async Task<string> FetchTime()
        {
            HttpClient Client = new(new HttpClientHandler { UseCookies = false, ServerCertificateCustomValidationCallback = Encryption.ValidateServerCertificate });
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Hexed)");

            HttpRequestMessage Payload = new(HttpMethod.Get, "https://api.logout.rip/Server/Time");
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
                Content = new StringContent(JsonConvert.SerializeObject(new { Auth = Encryption.EncryptAuthKey(Key, await FetchTime(), Encryption.GetHWID()) }), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string RawData = await Response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ServerObjects.UserData>(RawData);
            }

            return null;
        }
    }
}