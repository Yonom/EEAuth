using System;
using System.Linq;
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

        public AuthService()
        {
            this.OriginValidator = this.MyOriginValidator;
        }

        private bool MyOriginValidator(string s)
        {
            return new Uri(s).Host == "eeauth.yonom.org";
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
                this.Send("room " + worldId);
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
                if (this._username != null)
                {
                    this.Chat("Room contaminated. Evacuating...");
                    this.Error("Login cancelled. Another user joined the room.");
                }
                else
                {
                    Thread.Sleep(1000);

                    this._username = m.GetString(1);
                    this.Chat("Please chat \"yes\" to confirm the login.");
                }
            }
            else if (m.Type == "left")
            {
                this.Die("user_cancel", "Login was cancelled. User left the room.");
            }
            else if (m.Type == "say" && m.GetInt(0) != 1)
            {
                if (m.GetString(1).StartsWithIgnoreCase("y"))
                {
                    this.Chat("Login complete. You may now exit this room.");
                    this.FinishLogin();
                }
                else if (m.GetString(1).EqualsIgnoreCase("no"))
                {
                    this.Die("user_cancel", "User said no.");
                }
            }
        }

        private void Chat(string text)
        {
            this._connection.Send("say", text);
        }

        private void FinishLogin()
        {
            this.Send("redirect " + ResponseHelper.GetUrl(this._keyPair, this._redirectUri, this.Context.QueryString["redirect_uri"], this._username, this._state));
            this.Close();
        }

        private void Error(string error)
        {
            this.Send("error " + error);
            this.Close();
        }

        private void Die(string error, string description)
        {
            this.Send("redirect " + ResponseHelper.GetErrorUrl(this._redirectUri, error, description));
            this.Close();
        }

        private void Close()
        {
            this.Context.WebSocket.Close(CloseStatusCode.Normal);
            this._timeoutResetEvent.Set();
        }
    }
}