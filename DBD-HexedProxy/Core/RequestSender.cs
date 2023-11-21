using Fiddler;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace HexedProxy.Core
{
    internal class RequestSender
    {
        public static HTTPRequestHeaders headers;

        public static async Task<bool> SendFriendRequest(string uid)
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
    }
}
