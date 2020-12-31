using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Navs;

namespace UnityEditor.UI.Navs
{
    [CustomEditor(typeof(UINavButton))]
    public class UIButtonInspector : Editor
    {
        UINavButton Asset
        {
            get => target as UINavButton;
        }

        public override void OnInspectorGUI()
        {
            UINavButton btn = Asset;

            using (var checker = new EditorGUI.ChangeCheckScope())
            {

                btn.mode = (NavMode)EditorGUILayout.EnumPopup("Mode", btn.mode);

                //if (btn.mode == NavMode.Back )
                //{
                //    int[] values = new int[] { 0, UIManager.BUTTON_POSITIVE, UIManager.BUTTON_NEUTRAL, UIManager.BUTTON_NEGATIVE };
                //    string[] options = new string[] { "None", "Positive", "Neutral", "Negative" };
                //    btn.button = EditorGUILayout.IntPopup("Button", btn.button, options, values);
                //}

                if (btn.mode == NavMode.Back)
                {

                }
                else
                {
                    btn.url = EditorGUILayout.TextField("Url", btn.url);
                }
                if (checker.changed)
                {
                    EditorUtility.SetDirty(target);
                    serializedObject.UpdateIfRequiredOrScript();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

    }
}