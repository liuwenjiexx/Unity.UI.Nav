using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI.Navs;

/// <summary>
/// 实现对话框控制器
/// </summary>
class DialogController : Controller
{
    public override ViewResult View(string viewName)
    {

        var result = new UIMgrResourcesViewResult(viewName, "UI/Dialog/" + viewName,null, Context) ;

        return result;
    }


    public ViewResult OK(string content, string title,   DialogClickDelegate onClick)
    {
        Debug.Log("Action: OK");
        var model = new DialogModel() { Content = content, Title = title ,OnClick=onClick};

        var result = View("OK", model);
        return result;
    }
    public ViewResult YesNo(string content, string title, DialogClickDelegate onClick)
    {
        Debug.Log("Action: YesNo");
        var model = new DialogModel() { Content = content, Title = title, OnClick = onClick };
        var result = View("YesNo", model);
        return result;
    }
}
