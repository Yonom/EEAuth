using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using EEAuth.Helpers;
using EEAuth.Security;
using PlayerIOClient;
using ServiceStack;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace EEAuth.Services
{
    internal class AuthService : WebSocketBehavior
    {
        private readonly ManualResetEvent _timeoutResetEvent = new ManualResetEvent(false);
        private Connection _connection;
        private KeyPair _keyPair;
        private Uri _redirectUri;
        private string _state;
        private string _username;
        private string _connectUserId;
        private int _userId;
        private int _token;

        public AuthService()
        {
            this.OriginValidator = this.MyOriginValidator;
        }

        private bool MyOriginValidator(string s)
        {
            return new Uri(s).Host == "eeauth.spambler.com";
        }

        protected override void OnOpen()
        {
            this.Log.Info(String.Format("Connection: ip {0} url {1}", this.Context.UserEndPoint, this.Context.Origin));
            new Thread(this.TimeoutWork) {IsBackground = true}.Start();

            if (
                this.Sessions.Sessions.Where(
                    session => Equals(session.Context.UserEndPoint.Address, this.Context.UserEndPoint.Address))
                    .Count(session => session.ID != this.ID) >= 3)
            {
                this.Error("IP limit reached.");
                return;
            }

            if (this.LoadArgs())
            {
                string worldId = EEHelper.GenerateWorldId();
                this._token = this.GetToken();
                this.Send("room " + worldId + " " + this._token);
                EEHelper.Connect(worldId, this.OnEEConnect, this.OnEEError);
            }
        }

        private bool LoadArgs()
        {
            this._state = this.Context.QueryString["state"];
            try
            {
                this._redirectUri = new UriBuilder(this.Context.QueryString["redirect_uri"]).Uri;
            }
            catch (Exception)
            {
                this.Error("Invalid redirect_uri parameter.");
                return false;
            }

            try
            {
                this._keyPair = KeyPair.FromPublicPart(this.Context.QueryString["client_id"]);
            }
            catch (Exception)
            {
                this.Error("Invalid client_id parameter.");
                return false;
            }

            return true;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (this._token.ToString("D") == e.Data)
            {
                this.FinishLogin();
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (this._connection != null)
                this._connection.Disconnect();
        }

        private void TimeoutWork()
        {
            this._timeoutResetEvent.WaitOne(new TimeSpan(0, 10, 0));
            if (this.Context.WebSocket.IsAlive)
                this.Error("Authentication timed out.");
        }

        private void OnEEConnect(Connection conn)
        {
            this._connection = conn;
            this._connection.OnMessage += this.OnEEMessage;
            this._connection.Send("init");
        }

        private void OnEEError(PlayerIOError err)
        {
            this.Error("Problem communicating with EE. Please try again later.");
            this.Log.Error(err.Message);
        }

        private void OnEEMessage(object sender, Message m)
        {
            if (m.Type == "init")
            {
                this._connection.Send("init2");
            }
            else if (m.Type == "k")
            {
                this.Chat("You are about to login with EEAuth.");
            }
            else if (m.Type == "add")
            {
                this._userId = m.GetInt(0);
                this._username = m.GetString(1);
                this._connectUserId = m.GetString(2);

                new Thread(() =>
                {
                    Thread.Sleep(1000);
                    this.Chat("Please chat your code to confirm the login.");
                }) {IsBackground = true}.Start();
            }
            else if (m.Type == "say" && m.GetInt(0) == this._userId)
            {
                if (m.GetString(1).StartsWithIgnoreCase(this._token.ToString()))
                {
                    this.Chat("Login complete. You may now exit this room.");
                    this.FinishLogin();
                }
            }
        }

        private int GetToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                const long diff = 99999999 - 10000000;
                var uint32Buffer = new byte[4];
                while (true)
                {
                    rng.GetBytes(uint32Buffer);
                    UInt32 rand = BitConverter.ToUInt32(uint32Buffer, 0);
                    const long max = (1 + (Int64)UInt32.MaxValue);
                    const long remainder = max % diff;
                    if (rand < max - remainder)
                    {
                        return (Int32)(10000000 + (rand % diff));
                    }
                }
            }
        }

        private void Chat(string text)
        {
            this._connection.Send("say", text);
        }

        private void FinishLogin()
        {
            this.Send("redirect " + ResponseHelper.GetUrl(this._keyPair, this._redirectUri, this.Context.QueryString["redirect_uri"], this._username, this._connectUserId, this._state));
            this.Close();
        }

        private void Error(string error)
        {
            this.Send("error " + error);
            this.Close();
        }

        private void Close()
        {
            this.Context.WebSocket.Close(CloseStatusCode.Normal);
            this._timeoutResetEvent.Set();
        }
    }
}