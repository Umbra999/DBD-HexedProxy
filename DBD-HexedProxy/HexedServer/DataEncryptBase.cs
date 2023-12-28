using System.Security.Cryptography;
using System.Text;

namespace HexedProxy.HexedServer
{
    internal class DataEncryptBase
    {
        public static string DecryptData(string combinedString, string privateKey)
        {
            string[] parts = combinedString.Split('|');
            string encryptedDataString = parts[0];
            string encryptedAesKeyString = parts[1];
            string encryptedAesIVString = parts[2];

            byte[] encryptedData = Convert.FromBase64String(encryptedDataString);
            byte[] encryptedAesKey = Convert.FromBase64String(encryptedAesKeyString);
            byte[] encryptedAesIV = Convert.FromBase64String(encryptedAesIVString);

            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(privateKey);

            byte[] aesKey = rsa.Decrypt(encryptedAesKey, false);
            byte[] aesIV = rsa.Decrypt(encryptedAesIV, false);

            byte[] decryptedData = DecryptWithAes(encryptedData, aesKey, aesIV);

            return Encoding.UTF8.GetString(decryptedData);
        }

        private static byte[] DecryptWithAes(byte[] data, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using MemoryStream msDecrypt = new();
            using (CryptoStream csDecrypt = new(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
            {
                csDecrypt.Write(data, 0, data.Length);
                csDecrypt.FlushFinalBlock();
            }
            return msDecrypt.ToArray();
        }

        public static string EncryptData(string dataToEncrypt, string publicKey)
        {
            using Aes aes = Aes.Create();
            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncrypt);
            byte[] encryptedData = EncryptWithAes(dataBytes, aes.Key, aes.IV);

            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(publicKey);

            byte[] encryptedAesKey = rsa.Encrypt(aes.Key, false);
            byte[] encryptedAesIV = rsa.Encrypt(aes.IV, false);

            return Convert.ToBase64String(encryptedData) + "|" + Convert.ToBase64String(encryptedAesKey) + "|" + Convert.ToBase64String(encryptedAesIV);
        }

        private static byte[] EncryptWithAes(byte[] data, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                csEncrypt.Write(data, 0, data.Length);
                csEncrypt.FlushFinalBlock();
            }
            return msEncrypt.ToArray();
        }
    }
}
