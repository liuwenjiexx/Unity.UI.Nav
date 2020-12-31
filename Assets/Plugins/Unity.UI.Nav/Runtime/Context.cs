using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI.Navs.Routing;
using System.Text;

namespace UnityEngine.UI.Navs
{

    public class Context
    {
        internal int id;
        internal string url;
        public int Id { get => id; set => id = value; }
        public string ActionName { get; set; }
        public string Url { get => url; set => url = value; }
        public RouteData RouteData { get; set; }

        public ViewResult ViewResult { get; set; }
    }
}
