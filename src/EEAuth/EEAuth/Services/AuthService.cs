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

        private bool MyOriginValidator(string s) => new Uri(s).Host == "eeauth.spambler.com";

        protected override void OnOpen()
        {
            Log.Info($"Connection: ip {Context.UserEndPoint} url {Context.Origin}");
            new Thread(TimeoutWork) {IsBackground = true}.Start();

            if (
                Sessions.Sessions.Where(
                    session => Equals(session.Context.UserEndPoint.Address, Context.UserEndPoint.Address))
                    .Count(session => session.ID != ID) >= 3)
            {
                Error("IP limit reached.");
                return;
            }

            if (LoadArgs())
            {
                Send("room " + EEHelper.WorldId);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"Connection closed: Clean? {e.WasClean}, Code: {e.Code}, Reason: {e.Reason}");
            this._timeoutResetEvent.Set();
        }

        private bool LoadArgs()
        {
            _state = Context.QueryString["state"];
            try
            {
                _redirectUri = new UriBuilder(Context.QueryString["redirect_uri"]).Uri;
            }
            catch (Exception)
            {
                Error("Invalid redirect_uri parameter.");
                return false;
            }

            try
            {
                _keyPair = KeyPair.FromPublicPart(Context.QueryString["client_id"]);
            }
            catch (Exception)
            {
                Error("Invalid client_id parameter.");
                return false;
            }

            return true;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var selectedPlayer = Global.Bot.Players.Values.FirstOrDefault(p => "verifyCode" + p.Token == e.Data);
            Console.WriteLine(e.Data);

            if (selectedPlayer == null)
            {
                Error("Invalid authentication code. Please reload page to retry.");
                return;
            }

            Global.Bot.PmTo(selectedPlayer.Username, "Thank you for using EEAuth. You may leave this world now.");

            var userId = Global.Bot.Players.FirstOrDefault(val => val.Value == selectedPlayer).Key;
            if (Global.Bot.Players.ContainsKey(userId))
                Global.Bot.Players.Remove(userId);

            FinishLogin(selectedPlayer.Username, selectedPlayer.ConnectUserId);
        }

        private void TimeoutWork()
        {
            _timeoutResetEvent.WaitOne(new TimeSpan(0, 10, 0));
            if (Context.WebSocket.IsAlive)
                Error("Authentication timed out. Please reload page.");
        }

        private void FinishLogin(string username, string connectUserId)
        {
            Send("redirect " +
                 ResponseHelper.GetUrl(_keyPair, _redirectUri, Context.QueryString["redirect_uri"], username,
                     connectUserId, _state));
            Close();
        }

        private void Error(string error)
        {
            Send("error " + error);
            Close();
        }

        private void Close()
        {
            Context.WebSocket.Close(CloseStatusCode.Normal);
            _timeoutResetEvent.Set();
        }
    }
}