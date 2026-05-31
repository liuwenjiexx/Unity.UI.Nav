using System;
using System.Collections.Generic;

namespace Kuxue.UI.Navs
{

    /// <summary>
    /// 对话框数据模型
    /// </summary>
    public class DialogModel
    {
        /// <summary>
        /// 对话框内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 对话框标题
        /// </summary>
        public string Title { get; set; }
        public string Style { get; set; }

        public int Buttons { get; set; }

        public bool Cancelable { get; set; }

        public int Flags;

        public Dictionary<string, object> Values { get; set; }

        public bool IsClosed { get; private set; }

        public void Close(int result)
        {
            if (!IsClosed)
            {
                IsClosed = true;
                OnClosing?.Invoke(result);
                Result = result;
                OnClose?.Invoke(Result);
            }
        }

        public bool IsDiried { get; private set; }

        public string InputPlaceholder { get; set; }
        public object ResultValue { get; set; }
        public int Result { get; set; }

        //public Func<object, bool> ValidateInput;

        public DialogClickDelegate OnClick { get; set; }

        public Action<int> OnClosing { get; set; }
        public Action<int> OnClose { get; set; }


        /// <summary>
        /// 通知界面更新
        /// </summary>
        public void SetDiry()
        {
            IsDiried = true;
        }

        public void ResetDiry()
        {
            IsDiried = false;
        }
    }

    public class InputDialogModel : DialogModel
    {

    }


    public class ButtonId
    {

        /// <summary>
        /// 确定按钮：Yes, Confirm
        /// </summary>
        public const int POSITIVE = 1 << 0;
        /// <summary>
        /// 否定按钮，No
        /// </summary>
        public const int NEGATIVE = 1 << 1;
        /// <summary>
        /// 中立按钮，Cancel
        /// </summary>
        public const int NEUTRAL = 1 << 2;

        public const int CLOSE = 1 << 3;

    }
}