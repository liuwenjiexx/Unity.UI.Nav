using System;
using UnityEngine;
using UnityEngine.UI.Navs;
using UnityEngine.UI.Navs.Routing;

public class TestScene : MonoBehaviour
{

    void Start()
    {

        if (!Nav.Initalized)
        {
            //开启日志
            //UINav.LogEnabled = true;

            Nav.Initialize();
            Nav.Root.Routes.Add("scene", new Route("Scene/{scene}", new { controller = "Scene" }, new NavRouteHandler()));

            //UINav.Push( "Scene/Scene1");
        }
    }


}

class SceneController : Controller
{

    class SceneViewResult : ViewResult
    { 
        public SceneViewResult(Controller controller, Context context, string sceneName) 
        {
            this.ViewName = sceneName;
        }


        public override void Load(Context context)
        {
            base.Load(context); 
            NavLoading.LoadScene(ViewName, "Loading", () =>
            {
                OnLoaded();
            });
        }
        
    }

    public override ViewResult View()
    {
        //获取场景名称
        string sceneName = Context.RouteData["scene"] as string;
        return View(sceneName);
    }

    public override ViewResult View(string viewName)
    {
        return new SceneViewResult(this, Context, viewName);
    }

    public override void OnEnter(NavMode mode, Context from)
    {
        base.OnEnter(mode, from); 
    }

}
