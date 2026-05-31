using System;
using System.Collections.Generic;

namespace Unity.UI.Routing
{

    public class RouteData
    {
        private Dictionary<string, object> values = new (StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, object> metadata = new (StringComparer.InvariantCultureIgnoreCase);

        public Dictionary<string, object> Values { get => values; }

        public Dictionary<string, object> Metadata { get => metadata;  }


        public IRouteHandler RouteHandler { get; set; }

        public object this[string paramName]
        {
            get => Values[paramName];
        }

    }
}
