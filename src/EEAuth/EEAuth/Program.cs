using System;
using System.Net;
using EEAuth.Helpers;
using EEAuth.Security;
using EEAuth.Services;
using WebNom;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace EEAuth
{
    internal static class Program
    {
        private static void Main()
        {
            var websocket = new WebSocketServer(IPAddress.Any, 5010, true)
            {
                SslConfiguration = { ServerCertificate = Key.Cert },
                Log = { Level = LogLevel.Info }
            };
            websocket.AddWebSocketService<AuthService>("/Auth");
            websocket.Start();

            var webnom = new WebNomClient
            {
                Host = "localhost", 
                Port = 5011
            };
            webnom.Start();

            while(true)
            {
                if (Console.ReadLine() == "exit")
                    Environment.Exit(0);
            }
        }
    }
}