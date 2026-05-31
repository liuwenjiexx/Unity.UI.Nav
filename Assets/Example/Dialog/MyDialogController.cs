using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.UI.Navs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

class MyDialogController : Controller
{

    //public static int DialogSortOrder = 10;
    public static Transform canvasRoot;

    public override void Initialize(NavContext context)
    {
        base.Initialize(context);

        //不会隐藏之前的界面
        context.Flags |= NavFlags.Float;

        if (!canvasRoot)
        {
            canvasRoot = (GameObject.FindAnyObjectByType(typeof(Canvas)) as Canvas).transform;
        }

    }


    public ViewResult Home()
    {
        string path = "UI/MyDialog/Home";
        var result = new ResourcesViewResult("Home", path, canvasRoot, Context);
        result.SortOrder = canvasRoot.childCount;

        result.Loaded += Result_Loaded;
        return result;
    }

    public override ViewResult View(string viewName)
    {
        ViewResult result = null;
        viewName = "Dialog_" + viewName;
        if (Reusable.TryGet(viewName, out var go))
        {
            result = GameObjectViewResult.FromGameObject(viewName, go, Context);
        }
        if (result == null)
        {
            string path;
            path = "UI/MyDialog/" + viewName;
            result = new ResourcesViewResult(viewName, path, canvasRoot, Context);
        }

        result.SortOrder = canvasRoot.childCount;
        result.Loaded += Result_Loaded;
        return result;
    }

    private void Result_Loaded(ViewResult result)
    {
        var goResult = result as GameObjectViewResult;
        GameObject go = goResult.GameObject;
        go.transform.SetSiblingIndex(result.SortOrder);
    }

    public ViewResult OK(string content, string title)
    {
        Debug.Log("Action: OK");
        DialogModel model = new DialogModel
        {
            Title = title,
            Content = content
        };

        return View("OK", model);
    }


    public ViewResult YesNo(DialogModel model)
    {
        Debug.Log("Action: OK_Model");
        var result = View("YesNo", model);
        return result;
    }

    public ViewResult Loading(DialogModel model)
    {
        Debug.Log("Action: Loading");
        var result = View("Loading", model);
        return result;
    }

}
