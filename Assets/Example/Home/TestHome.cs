using System;
using System.Collections.Generic;
using UnityEngine.UI.Navs.Routing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Navs;

public class TestHome : MonoBehaviour
{

    void Start()
    {
        //开启日志
        //UINav.LogEnabled = true;

        Nav.Initialize();
        Nav.Root.Routes.Add("default", new Route("Home/{action?}/{id?}",new { controller = "MyHome" }, new NavRouteHandler()));

        //设置Home 界面防止被关闭
        Nav.SetHome(Nav.Push("Home"));

    }

}
