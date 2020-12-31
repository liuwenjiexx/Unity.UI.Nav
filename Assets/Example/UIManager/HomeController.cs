using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Navs;

/// <summary>
/// 实现主页控制器
/// </summary>
class HomeController : Controller
{
    public override ViewResult View(string viewName)
    {

        var result = new UIMgrResourcesViewResult()
        {
            ViewData = ViewData,
            ViewName = viewName,
            Path = "UI/Home/" + viewName
        };

        return result;
    }


}