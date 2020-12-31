using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Navs;
using UnityEngine.UI.Navs.Routing;

public class TestDialog : MonoBehaviour
{
    void Start()
    {
        Nav.Initialize();
        Nav.Root.Routes.Add("dialog", new Route("Dialog/{action}/{content=}/{title=}", new { controller = "MyDialog" }, new NavRouteHandler()));

    }

    /// <summary>
    /// 传递匿名对象参数
    /// </summary>
    public void ShowOKDialog()
    {
        Debug.Log("click ShowOKDialog");
        Nav.Push("Dialog/OK",
            new
            {
                title = "my title",
                content = "my content " + Random.Range(1, 100)
            });
    }

    /// <summary>
    /// 传递数据模型参数
    /// </summary>
    public void ShowOKDialog_Model()
    {
        Debug.Log("click ShowOKDialog");
        Nav.Push("Dialog/OK_Model", new
        {
            model = new DialogModel()
            {
                Title = "my title",
                Content = "my content " + Random.Range(1, 100)
            }
        });
    }

}

