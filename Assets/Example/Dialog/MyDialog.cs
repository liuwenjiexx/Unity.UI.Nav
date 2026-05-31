using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LWJ.UI.Navs;

/// <summary>
/// 对话框视图
/// </summary>
public class MyDialog : Dialog
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

    void SetButton(Button btn, int button)
    {
        if (!btn)
            return;
        btn.onClick.AddListener(() =>
        {
            Click(button);
        });
    }

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
