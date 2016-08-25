using System;
using System.Linq;
using System.Threading;
using EEAuth.Helpers;
using EEAuth.Security;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace EEAuth.Services
{
    internal class AuthService : WebSocketBehavior
    {
        private readonly ManualResetEvent _timeoutResetEvent = new ManualResetEvent(false);
        private KeyPair _keyPair;
        private Uri _redirectUri;
        private string _state;

        public AuthService()
        {
            this.OriginValidator = this.MyOriginValidator;
            this.IgnoreExtensions = true;
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
                this.Send("room " + EEHelper.worldId);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"Connection closed: Clean? {e.WasClean}, Code: {e.Code}, Reason: {e.Reason}");
            this._timeoutResetEvent.Set();
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
            Player selectedPlayer = Global.Bot.Players.Values.FirstOrDefault((Player p) => "verifyCode" + p.Token == e.Data);
            Console.WriteLine(e.Data);

            if (selectedPlayer == null)
            {
                Error("Invalid key! Please reload page.");
                return;
            }

            Global.Bot.PmTo(selectedPlayer.Username, "Thank you for using EEAuth. You may leave this world now.");
            Global.Bot.Players.Remove(Global.Bot.Players.FirstOrDefault(val => val.Value == selectedPlayer).Key);
            FinishLogin(selectedPlayer.Username, selectedPlayer.ConnectUserId);
        }

        private void TimeoutWork()
        {
            this._timeoutResetEvent.WaitOne(new TimeSpan(0, 10, 0));
            if (this.Context.WebSocket.IsAlive)
                this.Error("Authentication timed out. Please reload page.");
        }

        private void FinishLogin(string username, string connectUserId)
        {
            this.Send("redirect " + ResponseHelper.GetUrl(this._keyPair, this._redirectUri, this.Context.QueryString["redirect_uri"], username, connectUserId, this._state));
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