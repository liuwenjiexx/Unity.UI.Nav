# UI Navigation

UI界面导航，使用MVC（Model，View，Controller）设计模式分离UI代码



## 使用


#### 打开主界面

```c#
Nav.SetHome(Nav.Push("Home"));
```

`SetHome` 设置主页防止被关闭

#### 打开界面

```c#
Nav.Push("Home/Window1");
```

#### 打开包含OK按钮的对话框

```
Nav.Push("Dialog/OK", new { content = "my content" });
```

#### 打开包含Yes和No按钮的对话框

```
Nav.Push("Dialog/YesNo", new { content = "my content" });
```



### 使用封装的 `UIManager`

```
UIManager.OpenHome("Home");
UIManager.Open("Home/Window1");
UIManager.DialogOK("my content");
UIManager.DialogYesNo("my content", (button) =>
	{
		Debug.Log("button: " + button);
	});
```






## Url 格式


```
{controller=Home}/{action?}/{id?}
```

- controller

  控制器名称，默认值为 `Home`，对应 `HomeController`控制器, `controller` 相当于一个模块

- action

  可选，执行动作

- id

  可选，参数`id`值




### 参数名称

```
{Name}
```

如

```
{controller}
{action}
```

### 默认值

使用 `=` 符号

```
{controller=Home}
```

默认值为`Home`

```
{title=}
```

默认值为`空`

### 可选参数

使用 `?` 符号，可选参数必须在末尾

```
{action?}
{id?}
```

获取参数值

```
Context.RouteData["controller"]
```



### 添加默认Url

Url

```c#
Nav.Root.Routes.Add("default", new Route("{controller=Home}/{action?}/{id?}", new NavRouteHandler()));
```






## 传递参数

**Url**

```c#
Nav.Root.Routes.Add("dialog", new Route("Dialog/{action}/{content=}/{title=}", new { controller = "Dialog" }, new NavRouteHandler()));
```

- content

  消息内容，默认为空

- title

  对话框标题，默认为空

**控制器**

```c#
public ViewResult OK(string title, string content)
{
	var result = View("OK");
    result.Loaded += (r) =>
    {
        result.Transform.Find("Title").GetComponent<Text>().text = title;
        result.Transform.Find("Content").GetComponent<Text>().text = content;
    };
    return result;
}
```
方法名 `OK` 为 `{action}`

方法参数 `title`, `content` 对应路由参数 `{title}`, `{content}`

**调用**

```c#
Nav.Push("Dialog/OK",	new	{ title = "my title", content = "my content"  });
```

**YesNo按钮对话框**

**控制器**

```c#
public ViewResult YesNo(string title, string content)
{
	var result = View("YesNo");
	...
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
    public string Content { get; set; }
    public string Title { get; set; }
}
```

**视图**

```c#
public class DialogView : View
{
    public Text content;
    public Text title;
    protected override void OnShow()
    {
        var data = Model as DialogModel;
        if (data != null)
        {
            if (content)
                content.text = data.Content;
            if (title)
                title.text = data.Title;
        }
    }
}
```

**控制器**

```c#
public ViewResult OK(object model)
{
    var result = View("OK", model);
    return result;
}
```

**调用**

```c#
UINav.Push("Dialog/OK", new
{
    model = new DialogModel()
    {
        Title = "my title",
        Content = "my content"
    }
});
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

