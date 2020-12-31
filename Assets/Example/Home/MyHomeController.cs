using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Navs;

public class MyHomeController : Controller
{

    public override ViewResult View(string viewName)
    {

        var result = new ResourcesViewResult()
        {
            ViewData = ViewData,
            ViewName = viewName,
            Path = "UI/MyHome/" + viewName
        };

        result.Loaded += (r) =>
        {
            if (r.View != null)
            {
                //新界面打开时隐藏
                r.View.HideOnExit = true;
            }
        };
        return result;
    }

}
