using System;
using System.Collections;
using Unity.Loading;
using LWJ.UI.Navs;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestDialogHome : MonoBehaviour
{
 
    public void OK_Url()
    {
        string title = Uri.EscapeDataString("title");
        string content = Uri.EscapeDataString("content $ % /");
        Nav.Push($"Dialog/OK/{content}/{title}");
    }

    /// <summary>
    /// 传递匿名对象参数
    /// </summary>
    public void OK()
    {
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
    public void YesNo()
    {
        StartCoroutine(_YesNo());
    }
    public IEnumerator _YesNo()
    {

        DialogModel dialog = new DialogModel()
        {
            Title = "my title",
            Content = "my content " + Random.Range(1, 100),
        };
        dialog.OnClick = (button) =>
        {
            Debug.Log("Click button " + button);

            dialog.Close(button);
        };

        Nav.Push("Dialog/YesNo", dialog);

        yield return new WaitUntil(() => dialog.IsClosed);

        Debug.Log("Dialog Result: " + dialog.Result);
    }
    public void Loaidng()
    {
        StartCoroutine(_Loaidng());
    }

    IEnumerator _Loaidng()
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
}
