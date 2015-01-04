using System.Web;

namespace EEAuth.Helpers
{
    public static class Extensions
    {
        public static string ToToken(this byte[] input)
        {
            return HttpServerUtility.UrlTokenEncode(input).TrimEnd('1');
        }

        public static byte[] FromToken(this string input)
        {
            return HttpServerUtility.UrlTokenDecode(input + '1');
        }
    }
}