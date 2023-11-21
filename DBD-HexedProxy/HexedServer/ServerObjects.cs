namespace HexedServer
{
    internal class ServerObjects
    {
        public enum KeyPermissionType
        {
            VRChat = 0,
            SeaOfThieves = 1,
            CSGO = 2,
            Minecraft = 3,
            VRChatBot = 4,
            VRChatReuploader = 5
        }

        public class UserData
        {
            public string Username { get; set; }
            public string Token { get; set; }
            public KeyPermissionType[] KeyAccess { get; set; }
        }
    }
}
