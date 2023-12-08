using Fiddler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace HexedProxy.Modules
{
    internal class RequestSender
    {
        private static HTTPRequestHeaders headers;

        public static void OnDefaultHeadersReceived(HTTPRequestHeaders Header)
        {
            headers = Header;
        }

        public static async Task<bool> AddFriend(string uid)
        {
            if (headers == null) return false; 

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            string Body = JsonConvert.SerializeObject(new { ids = new string[] { uid }, platform = "kraken" } );

            HttpRequestMessage Payload = new(HttpMethod.Post, $"https://{headers["Host"]}/api/v1/players/friends/add")
            {
                Content = new StringContent(Body, Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            return Response.IsSuccessStatusCode;
        }

        public static async Task<bool> RemoveFriend(string uid)
        {
            if (headers == null) return false;

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            string Body = JsonConvert.SerializeObject(new { ids = new string[] { uid }, platform = "kraken" });

            HttpRequestMessage Payload = new(HttpMethod.Post, $"https://{headers["Host"]}/api/v1/players/friends/remove")
            {
                Content = new StringContent(Body, Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            return Response.IsSuccessStatusCode;
        }

        public static async Task<DBDObjects.PlayerNameProvider.ResponseRoot> GetPlayerProvider(string uid)
        {
            if (headers == null) return null;

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            HttpRequestMessage Payload = new(HttpMethod.Get, $"https://{headers["Host"]}/api/v1/players/{uid}/provider/provider-id")
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string respBody = await Response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DBDObjects.PlayerNameProvider.ResponseRoot>(respBody);
            }

            return null;
        }

        public static async Task<DBDObjects.PlayerNameById.ResponseRoot> GetPlayerByCloudId(string uid)
        {
            if (headers == null) return null;

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            HttpRequestMessage Payload = new(HttpMethod.Get, $"https://{headers["Host"]}/api/v1/playername/byId/{uid}")
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string respBody = await Response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DBDObjects.PlayerNameById.ResponseRoot>(respBody);
            }

            return null;
        }

        public static async Task<bool> FinishTutorial(string step, string Id)
        {
            if (headers == null) return false;

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            string Body = JsonConvert.SerializeObject(new { clientTutorialId = "FA46EF074B8F42DFA9955B932526871C", stepId = step, tutorialId = Id });

            HttpRequestMessage Payload = new(HttpMethod.Post, $"https://{headers["Host"]}/api/v1/onboarding/update-player-progress")
            {
                Content = new StringContent(Body, Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            return Response.IsSuccessStatusCode;
        }

        public static async Task<DBDObjects.OnboardingChallanges.ResponseRoot> GetOnboardingChallenges()
        {
            if (headers == null) return null;

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            HttpRequestMessage Payload = new(HttpMethod.Get, $"https://{headers["Host"]}/api/v1/onboarding")
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string respBody = await Response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DBDObjects.OnboardingChallanges.ResponseRoot>(respBody);
            }

            return null;
        }

        public static async Task<JObject> FinishBloodweb(string Character, string[] BlockedNodes, string[] SelectedNodes)
        {
            if (headers == null) return null;

            HttpClient Client = new(new HttpClientHandler { UseCookies = false, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true);

            foreach (var ogHeader in headers)
            {
                if (!ogHeader.Name.StartsWith("x-") && ogHeader.Name != "Host" && ogHeader.Name != "User-Agent" && ogHeader.Name != "Cookie") continue;

                Client.DefaultRequestHeaders.Add(ogHeader.Name, ogHeader.Value);
            }

            string Body = JsonConvert.SerializeObject(new { characterName = Character, entityBlockedNodeIds = BlockedNodes, selectedNodeIds = SelectedNodes });

            HttpRequestMessage Payload = new(HttpMethod.Post, $"https://{headers["Host"]}/api/v1/dbd-character-data/bloodweb")
            {
                Content = new StringContent(Body, Encoding.UTF8, "application/json")
            };
            Payload.Content.Headers.ContentType.CharSet = "";

            HttpResponseMessage Response = await Client.SendAsync(Payload);

            if (Response.IsSuccessStatusCode)
            {
                string respBody = await Response.Content.ReadAsStringAsync();
                return JObject.Parse(respBody);
            }

            return null;
        }
    }
}
