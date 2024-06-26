using Microsoft.Win32;
using System.Security.Cryptography.X509Certificates;

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
        public static ServerObjects.HWID GetHWID()
        {
            List<string> unique = new();
            string DriveID = GetDriveID();
            if (DriveID != null && DriveID.Length > 1) unique.Add(DriveID);
            string BiosID = GetBiosID();
            if (BiosID != null && BiosID.Length > 1) unique.Add(BiosID);

            List<string> common = new();
            string DriveName = GetDriveName();
            if (DriveName != null && DriveName.Length > 1) common.Add(DriveName);
            string ProcessorID = GetProcessorID();
            if (ProcessorID != null && ProcessorID.Length > 1) common.Add(ProcessorID);
            string ProcessorName = GetProcessorName();
            if (ProcessorName != null && ProcessorName.Length > 1) common.Add(ProcessorName);
            string ProcessorVendor = GetProcessorVendor();
            if (ProcessorVendor != null && ProcessorVendor.Length > 1) common.Add(ProcessorVendor);
            string BiosManufacturer = GetBiosManufacturer();
            if (BiosManufacturer != null && BiosManufacturer.Length > 1) common.Add(BiosManufacturer);
            string BiosVendor = GetBiosVendor();
            if (BiosVendor != null && BiosVendor.Length > 1) common.Add(BiosVendor);
            string BiosProduct = GetBiosProduct();
            if (BiosProduct != null && BiosProduct.Length > 1) common.Add(BiosProduct);
            string BiosSystemManufacturer = GetBiosSystemManufacturer();
            if (BiosSystemManufacturer != null && BiosSystemManufacturer.Length > 1) common.Add(BiosSystemManufacturer);
            string BiosSystemName = GetBiosSystemName();
            if (BiosSystemName != null && BiosSystemName.Length > 1) common.Add(BiosSystemName);

            ServerObjects.HWID HWID = new()
            {
                Unique = unique.ToArray(),
                Common = common.ToArray()
            };

            return HWID;
        }

        private static string GetProcessorID()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            if (key != null) return key.GetValue("Identifier")?.ToString();

            return null;
        }

        private static string GetProcessorName()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            if (key != null) return key.GetValue("ProcessorNameString")?.ToString();

            return null;
        }

        private static string GetProcessorVendor()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            if (key != null) return key.GetValue("VendorIdentifier")?.ToString();

            return null;
        }

        private static string GetBiosManufacturer()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS");
            if (key != null) return key.GetValue("BaseBoardManufacturer")?.ToString();

            return null;
        }

        private static string GetBiosProduct()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS");
            if (key != null) return key.GetValue("BaseBoardProduct")?.ToString();

            return null;
        }

        private static string GetBiosVendor()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS");
            if (key != null) return key.GetValue("BIOSVendor")?.ToString();

            return null;
        }

        private static string GetBiosSystemManufacturer()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS");
            if (key != null) return key.GetValue("SystemManufacturer")?.ToString();

            return null;
        }

        private static string GetBiosSystemName()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS");
            if (key != null) return key.GetValue("SystemProductName")?.ToString();

            return null;
        }
        private static string GetBiosID()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("SYSTEM\\HardwareConfig");
            if (key != null) return key.GetValue("LastConfig")?.ToString();

            return null;
        }

        private static string GetDriveName()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0");
            if (key != null) return key.GetValue("Identifier")?.ToString();

            return null;
        }

        private static string GetDriveID()
        {
            RegistryKey key = Registry.LocalMachine?.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0");
            if (key != null) return key.GetValue("SerialNumber")?.ToString();

            return null;
        }
    }
}