using Microsoft.Win32;

namespace HexedProxy.HexedServer
{
    internal class SerialHandler
    {
        public static ServerObjects.HWID GetHWID()
        {
            ServerObjects.HWID HWID = new()
            {
                Unique = GatherUniqueSerials(),
                Common = GatherCommonSerials()
            };

            return HWID;
        }

        private static string[] GatherUniqueSerials()
        {
            List<string> unique = new();

            string BiosID = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\SYSTEM\\HardwareConfig", "LastConfig");
            ValidateAndAddSerial(BiosID, unique);

            string[] DiskSerials = SearchRegistryKeys("HKEY_LOCAL_MACHINE\\HARDWARE\\DEVICEMAP\\Scsi", "SerialNumber");
            foreach (string Path in DiskSerials)
            {
                string DeviceType = (string)GetKeyValue(Path, "DeviceType");
                if (DeviceType == null || DeviceType != "DiskPeripheral") continue;

                string OriginalSerial = (string)GetKeyValue(Path, "SerialNumber");
                ValidateAndAddSerial(OriginalSerial, unique);
            }

            return unique.ToArray();
        }

        private static string[] GatherCommonSerials()
        {
            List<string> common = new();

            string[] DiskNames = SearchRegistryKeys("HKEY_LOCAL_MACHINE\\HARDWARE\\DEVICEMAP\\Scsi", "Identifier");
            foreach (string Path in DiskNames)
            {
                string DeviceType = (string)GetKeyValue(Path, "DeviceType");
                if (DeviceType == null || DeviceType != "DiskPeripheral") continue;

                string OriginalSerial = (string)GetKeyValue(Path, "Identifier");
                ValidateAndAddSerial(OriginalSerial, common);
            }

            string ProcessorID = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "Identifier");
            ValidateAndAddSerial(ProcessorID, common);

            string ProcessorName = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "ProcessorNameString");
            ValidateAndAddSerial(ProcessorName, common);

            string ProcessorVendor = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "VendorIdentifier");
            ValidateAndAddSerial(ProcessorVendor, common);

            string BiosManufacturer = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "BaseBoardManufacturer");
            ValidateAndAddSerial(BiosManufacturer, common);

            string BiosVendor = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "BIOSVendor");
            ValidateAndAddSerial(BiosVendor, common);

            string BiosProduct = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "BaseBoardProduct");
            ValidateAndAddSerial(BiosProduct, common);

            string BiosSystemManufacturer = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "SystemManufacturer");
            ValidateAndAddSerial(BiosSystemManufacturer, common);

            string BiosSystemName = (string)GetKeyValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "SystemProductName");
            ValidateAndAddSerial(BiosSystemName, common);

            return common.ToArray();
        }

        public static void ValidateAndAddSerial(string input, List<string> Holder)
        {
            if (string.IsNullOrEmpty(input)) return;

            if (Holder.Contains(input)) return;

            if (input.All(c => c == input[0])) return;

            Holder.Add(input);
        }

        public static string[] SearchRegistryKeys(string path, string searchKey)
        {
            List<string> foundPaths = new List<string>();
            SearchRegistryKeysInternal(path, searchKey, foundPaths);
            return foundPaths.ToArray();
        }

        private static void SearchRegistryKeysInternal(string path, string searchKey, List<string> foundPaths)
        {
            string type = path.Contains("\\") ? path.Split('\\')[0] : path;
            string subPath = path.Contains("\\") ? path.Substring(type.Length + 1) : null;

            RegistryKey registry = type switch
            {
                "HKEY_CURRENT_USER" => string.IsNullOrEmpty(subPath) ? Registry.CurrentUser : Registry.CurrentUser.OpenSubKey(subPath),
                "HKEY_LOCAL_MACHINE" => string.IsNullOrEmpty(subPath) ? Registry.LocalMachine : Registry.LocalMachine.OpenSubKey(subPath),
                "HKEY_USERS" => string.IsNullOrEmpty(subPath) ? Registry.Users : Registry.Users.OpenSubKey(subPath),
                _ => null,
            };

            if (registry == null) return;

            try
            {
                foreach (string subKeyName in registry.GetSubKeyNames())
                {
                    using RegistryKey subKey = registry.OpenSubKey(subKeyName);
                    if (subKey != null && subKey.GetValueNames().Contains(searchKey))
                    {
                        foundPaths.Add($"{path}\\{subKeyName}");
                    }
                    SearchRegistryKeysInternal($"{path}\\{subKeyName}", searchKey, foundPaths);
                }
            }
            catch { }
        }

        public static object GetKeyValue(string path, string key)
        {
            string Type = path.Split('\\')[0];
            string SubPath = path.Replace(Type + "\\", "");

            RegistryKey registry;

            switch (Type)
            {
                case "HKEY_CURRENT_USER":
                    registry = string.IsNullOrEmpty(SubPath) ? Registry.CurrentUser : Registry.CurrentUser.OpenSubKey(SubPath, false);
                    break;

                case "HKEY_LOCAL_MACHINE":
                    registry = string.IsNullOrEmpty(SubPath) ? Registry.LocalMachine : Registry.LocalMachine.OpenSubKey(SubPath, false);
                    break;

                case "HKEY_USERS":
                    registry = string.IsNullOrEmpty(SubPath) ? Registry.Users : Registry.Users.OpenSubKey(SubPath, false);
                    break;

                default:
                    return null;
            }

            if (registry == null) return null;

            return registry.GetValue(key);
        }
    }
}
