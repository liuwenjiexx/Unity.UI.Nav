using System;
using Kuxue.UI.Navs;
using Kuxue.UI.Routing;
using UnityEngine;

public class TestScene : MonoBehaviour
{

    void Start()
    {

        if (!Nav.Initalized)
        {
            Nav.Initialize();
            Nav.Routes.Add("scene", new Route("Scene/{scene}", new { controller = "Scene" }, new NavRouteHandler()));

            //UINav.Push( "Scene/Scene1");
        }
    }


}

class SceneController : Controller
{

    class SceneViewResult : ViewResult
    {
        public SceneViewResult(string viewName, string sceneName, Controller controller, NavContext context)
            : base(viewName, context)
        {
        }


        public override void Load(NavContext context)
        {
            base.Load(context);
            NavLoading.LoadScene(ViewName, "Loading", () =>
            {
                OnLoaded();

                //场景加载完成后清除
                Nav.Back(context.Url);
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
        return new SceneViewResult(viewName, viewName, this, Context);
    }

    public override void OnEnter(NavMode mode, NavContext from)
    {
        base.OnEnter(mode, from);
    }

}
