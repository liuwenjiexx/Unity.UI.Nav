using UnityEngine;

namespace Kuxue.UI.Navs
{
    public enum NavFlags
    {
        None,
        /// <summary>
        /// 主界面，不能回退来关闭
        /// </summary>
        Home = 1 << 0,
        /// <summary>
        /// 禁用回退关闭界面
        /// </summary>
        DisableBack = 1 << 1,
        /// <summary>
        /// 浮动层界面，保持上一个界面不被隐藏，如：对话框
        /// </summary>
        Float = 1 << 2,
    }
}
