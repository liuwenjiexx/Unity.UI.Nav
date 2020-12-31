using System;

namespace UnityEngine.UI.Navs.Routing
{
    public class RouteException : Exception
    {
        public RouteException(string url)
            : this(url, "Missing route")
        {
        }

        public RouteException(string url, string message)
            : base($"{message}\nUrl: {url}")
        {
        }

        public RouteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
