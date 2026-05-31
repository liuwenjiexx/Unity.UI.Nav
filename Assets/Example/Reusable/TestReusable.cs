using System;
using System.Linq;
using LWJ.UI.Navs;
using LWJ.UI.Routing;
using UnityEngine;

public class TestReusable : MonoBehaviour
{

    void Start()
    {
        //开启日志
        NavUtility.LogEnabled = true;

        Nav.Initialize();
        Nav.Routes.Add("reusable", new Route("Reusable/{action?}/{id?}", new { controller = "Reusable" }, new NavRouteHandler()));

        foreach(var go in Reusable.cached.Values)
        {
            go.SetActive(false);
        }

        Nav.PushHome("Reusable");

    }

}

public class ReusableController : Controller
{
    public override ViewResult View(string viewName)
    {
        ViewResult result = null;
        if (Reusable.TryGet(viewName, out var go))
        {
            result = GameObjectViewResult.FromGameObject(viewName, go, Context);
        }
        if (result == null)
        {
            string path = $"UI/Reusable/" + viewName;
            result = new ResourcesViewResult(viewName, path, null, Context);
        }

        return result;
    }
}