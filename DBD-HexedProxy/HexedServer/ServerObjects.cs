namespace HexedServer
{
    internal class ServerObjects
    {
        public enum KeyPermissionType
        {
            DeadByDaylightUnlocker = 4,
        }

        public class UserData
        {
            public string Username { get; set; }
            public string Token { get; set; }
            public KeyPermissionType[] KeyAccess { get; set; }
        }
    }
}
