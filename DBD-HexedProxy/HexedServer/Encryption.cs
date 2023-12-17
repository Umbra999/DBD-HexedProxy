using System.Security.Cryptography.X509Certificates;

namespace HexedProxy.HexedServer
{
    internal class Encryption
    {
        public static string ServerThumbprint;

        // CLIENT SIDE VALIDATION
        public static bool ValidateServerCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return certificate.Thumbprint == ServerThumbprint;
        }
    }
}