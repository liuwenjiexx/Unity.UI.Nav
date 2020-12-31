using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Navs;

/// <summary>
/// 对话框视图
/// </summary>
public class DialogView : View
{

    public Text content;
    public Text title;
    public Button btnPositive;
    public Button btnNeutral;
    public Button btnNegative;

    private void Awake()
    {
        SetButton(btnPositive, UIManager.BUTTON_POSITIVE);
        SetButton(btnNeutral, UIManager.BUTTON_NEUTRAL);
        SetButton(btnNegative, UIManager.BUTTON_NEGATIVE);
    }

    void SetButton(Button btn,int button)
    {

        if (btn)
        {
            btn.onClick.AddListener(() =>
            {
                var data = Model as DialogModel;
                if (data != null)
                {
                    if (data.OnClick != null)
                    {
                        data.OnClick(button);
                    }
                }
            });
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
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
