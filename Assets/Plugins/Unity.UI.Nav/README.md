# UI Navigation

UI界面导航，使用MVC（Model，View，Controller）设计模式分离UI代码



## 使用

**打开主界面**

```c#
Nav.PushHome("Home");
```

`PushHome` `SetHome` 设置主页防止被关闭, 忽略 `Back` 

**打开界面**

```c#
Nav.Push("Home/Window1");
```

**打开包含OK按钮的对话框**

```c#
Nav.Push("Dialog/OK", new { content = "my content" });
```

**打开包含Yes和No按钮的对话框**

```c#
Nav.Push("Dialog/YesNo", new { content = "my content" });
```

**关闭界面**

```
Nav.Back()
Nav.Back(int viewId)
Nav.Back(IView view)
```




## Url 路由


```c#
{controller=Home}/{action?}/{id?}
```

- controller

  控制器名称，默认值为 `Home`，对应 `HomeController`类型, `controller` 相当于一个模块

- action

  可选，执行动作

- id

  可选，参数`id`值



**参数名称**

```c#
{Name}
```

如

```c#
{controller}
{action}
```

**默认值**

使用 `=` 符号

```c#
{controller=Home}
```

默认值为`Home`

```c#
{title=}
```

默认值为`空`

**可选参数**

使用 `?` 符号，可选参数必须在末尾

```c#
{action?}
{id?}
```

获取参数值

```c#
Context.RouteData["controller"]
```



**注册路由**

```c#
Nav.Routes.Add("default", new Route("UI/{action}/{id?}", new { controller = "UI" }, new NavRouteHandler()));
```

**对话框**

```
Nav.Routes.Add("dialog", new Route("Dialog/{action}/{content=}/{title=}", new { controller = "Dialog" }, new NavRouteHandler()));
```




## 传递参数

**Url**

```c#
Nav.Routes.Add("dialog", new Route("Dialog/{action}/{content=}/{title=}", new { controller = "Dialog" }, new NavRouteHandler()));
```

- content

  消息内容，默认为空

- title

  对话框标题，默认为空

**控制器**

路由参数按名称映射到方法参数

```c#
ViewResult OK(string content, string title)
{
    DialogModel model = new DialogModel
    {
        Title = title,
        Content = content
    };
    return View("OK", model);
}
```
方法名 `OK` 为 `{action}`

方法参数 `title`, `content` 对应路由参数 `{title}`, `{content}`

**调用**

```
Nav.Push("Dialog/OK",	new	{ title = "my title", content = "my content"  });
```

**Url 方式调用**

```c#
string title = Uri.EscapeDataString("title");
string content = Uri.EscapeDataString("content");
Nav.Push($"Dialog/OK/{content}/{title}");
```

**YesNo按钮对话框**

**控制器**

```c#
ViewResult YesNo(string content, string title)
{
    DialogModel dialog = new DialogModel
    {
        Title = title,
        Content = content
    };
    return View("YesNo", dialog);
}
```

**调用**

```c#
Nav.Push("Dialog/YesNo", new { title = "title", content = "content" });
```





## 数据模型

**Url**

```c#
Nav.Root.Routes.Add("dialog", new Route("Dialog/{action}/{content=}/{title=}", new { controller = "Dialog" }, new NavRouteHandler()));
```

**数据模型**

```c#
class DialogModel
{
    //对话框内容
    public string Content { get; set; }
    //对话框标题
    public string Title { get; set; }
    //返回点击的按钮 ButtonId
    public int Result { get; set; }
    //返回结果值
    public object ResultValue { get; set; }
    //是否关闭的
    public bool IsClosed { get; private set; }
    //按钮点击事件
    public DialogClickDelegate OnClick { get; set; }
    //关闭事件
    public Action<int> OnClose { get; set; }
    //通知界面更新
    public void SetDiry()
    ...
}
```

**视图**

继承 `Navigable`

```c#
public class Navigable : MonoBehaviour, INavigable
{
    //界面ID
	int ViewId { get; }
    //视图数据
	Dictionary<string, object> ViewData { get; set; }
	//数据模型
	object Model { get; set; }
    //加载完成
    void OnLoad();
	//卸载
    void OnUnload();
	//导航进入
    void OnNavigationFrom(NavContext from);
	//导航离开
    void OnNavigationTo(NavContext to);
}
```

对话框继承 `Dialog`

```c#
class MyDialog : Dialog
{
	public Text content;
    public Text title;
    
    protected override void Refresh()
    {
        base.Refresh();
        var model = Model;
        if (model != null)
        {
            if (content)
                content.text = model.Content;
            if (title)
                title.text = model.Title;
        }
    }
}
```

**控制器**

继承 `Controller`，实现加载 `View`



```c#
class MyDialogController : Controller
{
	public override void Initialize(NavContext context)
	{
        base.Initialize(context);

        //不会隐藏之前的界面
        context.Flags |= NavFlags.Float;
    }
    
    public override ViewResult View(string viewName)
    {
        ViewResult result = null;
        viewName = "Dialog_" + viewName;
        //优先使用可复用的实例
        if (Reusable.TryGet(viewName, out var go))
        {
            result = GameObjectViewResult.FromGameObject(viewName, go, Context);
        }
        if (result == null)
        {
            string path;
            path = "UI/MyDialog/" + viewName;
            result = new ResourcesViewResult(viewName, path, Context);
        }
        return result;
    }
    
    public ViewResult YesNo(DialogModel model)
    {
        var result = View("YesNo", model);
        return result;
    }
}
```

**调用**

```c#
var dialog = new DialogModel()
{
    Title = "my title",
    Content = "my content"
};
UINav.Push("Dialog/YesNo", dialog);
```



**按钮事件**

```c#
dialog.OnClick = (button) =>
{
	switch (button)
    {
        case ButtonId.POSITIVE:
            break;
        case ButtonId.NEUTRAL:
            break;
        case ButtonId.NEGATIVE:
            break;
    }
    //关闭对话框
    dialog.Close(button);
};
```

**关闭事件**

```c#
dialog.OnClose = (button) =>
{
};
```

**异步等待关闭**

```c#
yield return new WaitUntil(() => dialog.IsClosed);

Debug.Log("Dialog Result: " + dialog.Result);
```



**更新界面**

`model.SetDiry` 通知 `Refresh` 更新界面



```c#
IEnumerator Loaidng()
{
    DialogModel dialog = new DialogModel()
    {
        Content = "Loading ",
        Cancelable = true,
    };
    Nav.Push("Dialog/Loading", dialog);
    float t = 2f;
    while (!dialog.IsClosed && t > 0)
    {
        t -= Time.deltaTime;
        dialog.Content = $"Loading {t:0.#}s";
        dialog.SetDiry();
        yield return new WaitForEndOfFrame();
    }
    dialog.Close(0);
    Debug.Log("Dialog Result: " + dialog.Result);
}
```



## 跳转场景

**Url**

```c#
Nav.Root.Routes.Add("scene", new Route("Scene/{scene}", new { controller = "Scene" }, new NavRouteHandler()));
```

**调用**

```c#
Nav.Push("Scene/Scene1");
```





## 导航模式

**NavMode**

- Push

  保留当前界面，打开新界面

- Replace

  替换当前界面，如果当前为`Home`界面则`Push`模式

- Back

  关闭当前界面，忽略`Home`界面

- BackUntil

  回退到指定的界面，忽略`Home`界面

