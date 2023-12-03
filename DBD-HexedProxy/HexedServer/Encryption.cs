using Microsoft.Win32;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HexedServer
{
    internal class Encryption
    {
        public static string ServerThumbprint;
        public static string PublicEncryptionKey;

        // GENERAL UTILS

        public static string GetMD5HashFromFile(string fileName)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fileName);
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
        }

        public static Random Random = new(Environment.TickCount);
        public static string RandomString(int length)
        {
            char[] array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToArray();
            string text = string.Empty;
            for (int i = 0; i < length; i++)
            {
                text += array[Random.Next(array.Length)].ToString();
            }
            return text;
        }

        public static bool ValidateServerCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return certificate.Thumbprint == ServerThumbprint;
        }

        public static string FromBase64(string Data)
        {
            var base64EncodedBytes = Convert.FromBase64String(Data);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToBase64(string Data)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(Data);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static long GetUnixTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        // CLIENT SIDE ENCRYPTION

        public static string EncryptData(string dataToEncrypt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncrypt);
                byte[] encryptedData = EncryptWithAes(dataBytes, aes.Key, aes.IV);

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(PublicEncryptionKey);

                    byte[] encryptedAesKey = rsa.Encrypt(aes.Key, false);
                    byte[] encryptedAesIV = rsa.Encrypt(aes.IV, false);

                    return Convert.ToBase64String(encryptedData) + "|" + Convert.ToBase64String(encryptedAesKey) + "|" + Convert.ToBase64String(encryptedAesIV);
                }
            }
        }

        private static byte[] EncryptWithAes(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        // HWID FOR CLIENTS

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
            HWID += GetDriveID();
            HWID += GetHWIDProfile();
            HWID = GenerateHash(HWID);
            return "H" + HWID + "EX";
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