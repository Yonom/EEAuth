using EEAuth.Security;
using PlayerIOClient;

namespace EEAuth.Helpers
{
    internal static class EEHelper
    {
        public static readonly string WorldId; //Removed

        private static readonly Client Client = PlayerIO.QuickConnect.SimpleConnect(
            "everybody-edits-su9rn58o40itdbnw69plyw",
            Key.Email,
            Key.Password,
            null);

        public static void Connect(string worldId, Callback<Connection> successCallback,
            Callback<PlayerIOError> errorCallback)
        {
            int ver = Client.BigDB.Load("Config", "config").GetInt("version");
            Client.Multiplayer.CreateJoinRoom(
                worldId, 
                "Everybodyedits" + ver, 
                false,
                null,
                null, 
                successCallback,
                errorCallback);
        }
    }
}