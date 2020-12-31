using System;
using System.Collections.Generic;
using UnityEngine.UI.Navs.Routing;

namespace UnityEngine.UI.Navs
{
    /// <summary>
    /// default route pattern: {controller=Home}/{action?}/{id?}
    /// </summary>
    public class NavRouteHandler : IRouteHandler
    {

        public const string DefaultPattern = "{controller=Home}/{action?}/{id?}";
        static Dictionary<string, Type> cachedControllerTypes;

        public virtual void ProcessRoute(Context context)
        {
            RouteData routeData = context.RouteData;
              
            if (routeData.Values.ContainsKey("action"))
                context.ActionName = routeData.Values["action"] as string;
        }

        public virtual Controller GetController(Context context)
        {
            Controller controller = null;

            string controllerName = null;
            if (context.RouteData.Values.ContainsKey("controller"))
                controllerName = context.RouteData.Values["controller"] as string;

            if (!string.IsNullOrEmpty(controllerName))
            {
                string typeName = controllerName + "Controller";

                if (cachedControllerTypes == null)
                    cachedControllerTypes = new Dictionary<string, Type>();

                Type type;
                if (!cachedControllerTypes.TryGetValue(typeName, out type))
                {
                    type = Type.GetType(typeName);

                    if (type != null)
                    {
                        if (!typeof(Controller).IsAssignableFrom(type))
                            type = null;
                    }

                    if (type == null)
                    {
                        foreach (var ass in AppDomain.CurrentDomain.GetAssemblies().Referenced(typeof(Controller).Assembly))
                        {
                            type = ass.GetType(typeName);
                            if (type != null)
                            {
                                if (!typeof(Controller).IsAssignableFrom(type))
                                {
                                    type = null;
                                    continue;
                                }
                                break;
                            }
                        }
                    }
                    cachedControllerTypes[typeName] = type;
                }

                if (type != null)
                {
                    controller = Activator.CreateInstance(type) as Controller;
                }
                else
                {
                    if (controller == null)
                        throw new Exception("not found controller type: " + typeName);
                }
            }

            if (controller == null)
                throw new Exception("Controller null, controller name: " + controllerName);
            return controller;
        }
    }

}
