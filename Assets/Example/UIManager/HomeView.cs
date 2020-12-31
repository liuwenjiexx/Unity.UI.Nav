using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI.Navs.Example
{
    public class HomeView : View
    {
        public void OpenDialogOK()
        {
            UIManager.DialogOK("test ok", (button) =>
            {
                Debug.Log("button: " + button);
            });
        }

        public void OpenDialogYesNo()
        {
            UIManager.DialogYesNo("test yesNo", (button) =>
            {
                Debug.Log("button: " + button);
            });
        }
    }
}
