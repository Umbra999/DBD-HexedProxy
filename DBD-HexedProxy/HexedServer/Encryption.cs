using Microsoft.Win32;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HexedProxy.HexedServer
{
    internal class Encryption
    {
        public static string ServerThumbprint;
        public static string EncryptionKey;
        public static string DecryptionKey;

        // CLIENT SIDE VALIDATION
        public static bool ValidateServerCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return certificate.Thumbprint == ServerThumbprint;
        }

        // HWID 
        public static string GenerateHash(string Text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Text);
            byte[] hash = SHA256.HashData(bytes);
            string ComputeHash = string.Join("", from it in hash select it.ToString("x2"));
            return ComputeHash;
        }

        public static string GetHWID()
        {
            string HWID = Environment.MachineName;
            HWID += GetProcessorID();
            HWID += GetProcessorName();
            HWID += GetProcessorVendor();
            HWID += GetBIOSManufacturer();
            HWID += GetBIOSProduct();
            HWID += GetDriveName();
            HWID += GetDriveID();
            HWID += GetHWIDProfile();
            return "H" + GenerateHash(HWID) + "EX";
        }

        private static string GetProcessorID()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            if (key != null) return key.GetValue("Identifier")?.ToString();

            return "";
        }

        private static string GetProcessorVendor()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            if (key != null) return key.GetValue("VendorIdentifier")?.ToString();

            return "";
        }

        private static string GetProcessorName()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            if (key != null) return key.GetValue("ProcessorNameString")?.ToString();

            return "";
        }

        private static string GetBIOSManufacturer()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
            if (key != null) return key.GetValue("BaseBoardManufacturer")?.ToString();

            return "";
        }

        private static string GetBIOSProduct()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
            if (key != null) return key.GetValue("BaseBoardProduct")?.ToString();

            return "";
        }

        private static string GetDriveName()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 0\Target Id 0\Logical Unit Id 0");
            if (key != null) return key.GetValue("Identifier")?.ToString();

            return "";
        }

        private static string GetDriveID()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 0\Target Id 0\Logical Unit Id 0");
            if (key != null) return key.GetValue("SerialNumber")?.ToString();

            return "";
        }

        private static string GetHWIDProfile()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001");
            if (key != null) return key.GetValue("HwProfileGuid")?.ToString();

            return "";
        }
    }
}