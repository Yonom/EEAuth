using System.Net;
using ServiceStack.Text;
using WebNom.Pages;

namespace EEAuth.Pages
{
    internal abstract class JsonPage<T> : Page
    {
        protected sealed override void Run(HttpListenerContext context, InputReader input, OutputWriter output)
        {
            output.AddHeader("Content-Type", "application/json");
            output.Write(JsonSerializer.SerializeToString(GetContent(context, input, output)));
        }

        protected abstract T GetContent(HttpListenerContext context, InputReader input, OutputWriter output);
    }
}