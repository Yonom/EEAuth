using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using EEAuth.Helpers;
using PlayerIOClient;
using System.Linq;
using System.Xml;

namespace EEAuth.Services
{
    public class AuthBot
    {
        private Random random = new Random();
        private Connection _connection;
        public Dictionary<int, Player> Players = new Dictionary<int, Player>();

        public AuthBot()
        {
            this.Connect();
        }

        public void Connect()
        {
            EEHelper.Connect(EEHelper.worldId, this.OnEEConnect, this.OnEEError);
        }

        private void OnEEConnect(Connection conn)
        {
            this._connection = conn;
            this._connection.OnMessage += this.OnEEMessage;
            this._connection.OnDisconnect += this.OnEEDisconnect;
            this._connection.Send("init");
        }

        private void OnEEDisconnect(object sender, string arg)
        {
            Thread.Sleep(3000);
            this.Connect();
        }

        private void OnEEError(PlayerIOError err)
        {
            Console.WriteLine(err.Message);
            Environment.Exit(1);
        }

        private void OnEEMessage(object sender, Message m)
        {
            try
            {
                if (m.Type == "init")
                {
                    this._connection.Send("init2");
                    this._connection.Send("name", "EEAuth v2.0.1");
                }
                else if (m.Type == "add" && m.GetBoolean(8))
                {
                    var userId = m.GetInt(0);
                    var username = m.GetString(1);
                    var connectUserId = m.GetString(2);

                    Players.Add(userId, new Player
                    {
                        ConnectUserId = connectUserId,
                        Username = username,
                        Token = GetToken()
                    });

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        Thread.Sleep(1000);
                        this.PmTo(username, "Hello, you are logging in with EEAuth.");
                        Thread.Sleep(1000);
                        this.PmTo(username, $"Your authentication code is: {Players[userId].Token}");
                    });
                }
                else if (m.Type == "left")
                {
                    var userId = m.GetInt(0);

                    if (Global.Bot.Players.ContainsKey(userId))
                        Global.Bot.Players.Remove(userId);
                }
                else if (m.Type == "say")
                {
                    var userId = m.GetInt(0);
                    var text = m.GetString(1);

                    if (text.ToLower().Contains(Players[userId].Token.ToLower()))
                    {
                        if (Players[userId].TokenSpoils < 3)
                        {
                            var newToken = GetToken();
                            Players[userId].Token = newToken;
                            Players[userId].TokenSpoils++;
                            PmTo(Players[userId].Username,
                                $"Please do NOT share your authentication code in the chat! Here's a new code: {newToken}");
                        }
                        else
                        {
                            KickUser(Players[userId].Username, "Do not share your authentication code in the chat.");
                            Global.Bot.Players.Remove(userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void PmTo(string name, string text)
        {
            this._connection.Send("say", $"/pm {name} {text}");
        }

        public void KickUser(string name, string reason)
        {
            this._connection.Send("say", $"/kick {name} {reason}");
        }

        private string GetToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz-_";
            return new string(Enumerable.Repeat(chars, 16)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class Player
    {
        public string Username { get; set; }
        public string ConnectUserId { get; set; }
        public string Token { get; set; } = "";
        public int TokenSpoils { get; set; } = 0;
    }
}
