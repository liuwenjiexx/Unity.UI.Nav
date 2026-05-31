using Unity.UI.Navs;
using UnityEngine;

//namespace UnityEngine.UI.Navs.Example
//{

public class TestUIManager : MonoBehaviour
{
    private void Awake()
    {
        UIManager.OpenHome("Home");
    }
}

class UIMgrResourcesViewResult : ResourcesViewResult
{

    public UIMgrResourcesViewResult(string viewName, string resourcePath, Transform parent, NavContext context)
        : base(viewName, resourcePath, parent, context)
    {

    }

    protected override void OnLoaded()
    {
        GameObject go = GameObject;
        int layer = UIManager.LAYER_DEFAULT;
        if (Context.RouteData.Values.ContainsKey("__layer"))
            layer = (int)Context.RouteData["__layer"];
        Transform parent = UIManager.Instance.uiLayers[layer];
        go.transform.SetParent(parent, false);
        base.OnLoaded();
    }
    /*
    public override void Unload()
    {
        base.Unload();
        if (Status == ViewResultStatus.Unloading)
        {
            GameObject go = this.GameObject;
            if (go)
                GameObject.Destroy(go);
            GameObject = null;
            OnUnloaded();
        }
    }*/
}



//}
