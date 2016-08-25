﻿using System.Net;
using EEAuth.Security;
using WebNom.Pages;

namespace EEAuth.Pages
{
    internal class GetKey : JsonPage<object>
    {
        protected override string Path => "/getkey";

        protected override object GetContent(HttpListenerContext context, InputReader input, OutputWriter output) => KeyPair.New();
    }
}