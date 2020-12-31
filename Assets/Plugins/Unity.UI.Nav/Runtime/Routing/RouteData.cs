using System.Collections.Generic;

namespace UnityEngine.UI.Navs.Routing
{

    public class RouteData
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();
        private Dictionary<string, object> metadata = new Dictionary<string, object>();

        public Dictionary<string, object> Values { get => values; }

        public Dictionary<string, object> Metadata { get => metadata;  }


        public IRouteHandler RouteHandler { get; set; }

        public object this[string paramName]
        {
            get => Values[paramName];
        }

    }
}
