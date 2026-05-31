using Unity.UI.Navs;
using Unity.UI.Routing;
using UnityEngine;

public class TestDialog : MonoBehaviour
{
    void Start()
    {
        Nav.Initialize();
        Nav.Routes.Add("dialog", new Route("Dialog/{action}/{content=}/{title=}", new { controller = "MyDialog" }, new NavRouteHandler()));
        Nav.Push("Dialog/Home");

    }


}

