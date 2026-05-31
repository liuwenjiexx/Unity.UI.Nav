using System;
using Kuxue.UI.Navs;
using Kuxue.UI.Routing;
using UnityEngine;

public class TestHome : MonoBehaviour
{

    void Start()
    {
        Nav.Initialize();
        Nav.Routes.Add("default", new Route("Home/{action?}/{id?}", new { controller = "MyHome" }, new NavRouteHandler()));
        Nav.PushHome("Home"); 

    }

}
public class MyHomeController : Controller
{

    public override ViewResult View(string viewName)
    {
        string path = "UI/MyHome/" + viewName;
        var result = new ResourcesViewResult(viewName, path,null, Context) ;

        return result;
    }

}