using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using EEAuth.Security;
using ServiceStack.Text;

namespace EEAuth.Helpers
{
    internal static class ResponseHelper
    {
        public static string GetUrl(KeyPair keyPair, Uri redirectUri, string reqRedirectUri, string username, string state)
        {
            var response = new Dictionary<string, string>
            {
                {"redirect_uri", reqRedirectUri},
                {"expires", Convert.ToString(UnixTimeNow() + 60)},
                {"name", username}
            };
            if (state != null)
                response.Add("state", state);
            response.Add("sig", Sign.CalcSig(keyPair, response));
            response.Remove("redirect_uri");

            string jsonOutput = JsonSerializer.SerializeToString(response);
            string jsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonOutput));

            var uriBuilder = new UriBuilder(redirectUri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["data"] = jsonBase64;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri.ToString();
        }

        private static long UnixTimeNow()
        {
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        internal static string GetErrorUrl(Uri redirectUri, string error, string description)
        {
            var uriBuilder = new UriBuilder(redirectUri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["error"] = error;
            query["error_description"] = description;
            uriBuilder.Query = query.ToString();

            return Uri.EscapeUriString(uriBuilder.Uri.ToString());
        }
    }
}