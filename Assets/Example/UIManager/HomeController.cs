using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI.Navs;

/// <summary>
/// 实现主页控制器
/// </summary>
class HomeController : Controller
{
    public override ViewResult View(string viewName)
    {

        var result = new UIMgrResourcesViewResult(viewName, "UI/Home/" + viewName,null, Context);

        return result;
    }


}