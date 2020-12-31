using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Navs;

/// <summary>
/// 对话框数据模型
/// </summary>
class DialogModel
{
    /// <summary>
    /// 对话框内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 对话框标题
    /// </summary>
    public string Title { get; set; }

    public DialogClickDelegate OnClick { get; set; }
}
