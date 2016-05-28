using System;
using System.Collections.Generic;
using EEAuth.Security;
using PlayerIOClient;

namespace EEAuth.Helpers
{
    internal static class EEHelper
    {
        private static readonly Client _client = PlayerIO.QuickConnect.SimpleConnect(
            "everybody-edits-su9rn58o40itdbnw69plyw",
            Key.Email,
            Key.Password,
            null);

        public static string GenerateWorldId()
        {
            string roomId = UnixCrypt.Crypt("OW", Guid.NewGuid().ToString("D"))
                .Replace("/", null)
                .Replace(".", null);
            Console.WriteLine(roomId);
            return roomId;
        }

        public static void Connect(string worldId, Callback<Connection> successCallback,
            Callback<PlayerIOError> errorCallback)
        {
            int ver = _client.BigDB.Load("Config", "config").GetInt("version");
            _client.Multiplayer.CreateJoinRoom(
                worldId, 
                "Everybodyedits" + ver, 
                false,
                new Dictionary<string, string> {{"name", "**** [EEAuth v1.0.1] fuck"}}, 
                null, 
                successCallback,
                errorCallback);
        }
    }
}