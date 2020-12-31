using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnityEngine.UI.Navs.Routing
{

    public class RouteCollection : Collection<Route>
    {
        Dictionary<string, Route> map = new Dictionary<string, Route>();
        private List<string> ignores = new List<string>();

        public Route this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;
                Route result;
                if (map.TryGetValue(name, out result))
                    return result;
                return null;
            }
        }

        public void Add(string name, Route item)
        {
            this.Add(item);
            if (name != null)
                map[name] = item;
        }

        protected override void ClearItems()
        {
            map.Clear();
            base.ClearItems();
        }

        public void Ignore(string url)
        {
            ignores.Add(url);
        }

        public RouteData GetRouteData(string url, object parameter = null)
        {
            RouteData routeData = null;
            foreach (var item in this)
            {
                routeData = item.GetRouteData(url, parameter);
                if (routeData != null)
                    return routeData;
            }

            throw new RouteException(url); 
        }

    }
}
