using System;
using System.Collections.Generic;
using System.Linq;
using Unity.UI.Routing;
using System.Text;

namespace Unity.UI.Navs
{

    public class NavContext
    {
        private int id;
        private string url;
        public NavContext(INavigation provider)
        {
            Navigation = provider;
        }

        public int Id { get => id; set => id = value; }
        public string ActionName { get; set; }
        public string Url { get => url; set => url = value; }
        public RouteData RouteData { get; set; }

        public Controller Controller { get; internal set; }


        public Dictionary<string, object> ViewData => Controller.ViewData;

        public ViewResult ViewResult { get; set; }


        public NavFlags Flags { get; set; }


        public INavigation Navigation { get; private set; }
         
        //public bool IsClosed { get; set; }
    }
}
