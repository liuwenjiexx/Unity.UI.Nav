using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Navs;

class MyDialogController : Controller
{

    public override ViewResult View(string viewName)
    {

        var result = new ResourcesViewResult()
        {
            ViewData = ViewData,
            ViewName = viewName,
            Path = "UI/MyDialog/" + viewName
        };

        return result;
    }


    public ViewResult OK(string title, string content)
    {
        Debug.Log("Action: OK");
        var result = View("OK");
        result.Loaded += (r) =>
        {
            result.Transform.Find("Panel/Title").GetComponent<Text>().text = title;
            result.Transform.Find("Panel/Content").GetComponent<Text>().text = content;
        };
        return result;
    }

    public ViewResult YesNo(string title, string content)
    {
        Debug.Log("Action: YesNo");
        var result = View("YesNo");
        result.Loaded += (r) =>
        {
            result.Transform.Find("Panel/Title").GetComponent<Text>().text = title;
            result.Transform.Find("Panel/Content").GetComponent<Text>().text = content;
        };
        return result;
    }

    public ViewResult OK_Model(object model)
    {
        Debug.Log("Action: OK_Model");
        var result = View("OK_Model", model);
        return result;
    }


}
