using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.UI.Navs;
using UnityEngine.EventSystems;

namespace Unity.UI.Navs.Editor
{

    public class UINavWindow : EditorWindow
    {
        Vector2 scrollPos;

        [MenuItem("Window/UI Nav")]
        public static void ShowExample()
        {
            UINavWindow wnd = GetWindow<UINavWindow>();
            wnd.titleContent = new GUIContent("UI Nav");
        }
        private void OnGUI()
        {
            Navigation nav = Nav.Current as Navigation;
            if (nav == null)
            {
                GUILayout.Label("Nav.Current null");
                return;
            }
            EditorGUILayout.LabelField("Count", nav.Count.ToString());

            NavUtility.LogEnabled = EditorGUILayout.Toggle("Log", NavUtility.LogEnabled);

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Back"))
                {
                    Nav.Back();
                }
                if (GUILayout.Button("Back To Home"))
                {
                    Nav.BackToHome();
                }
                if (GUILayout.Button("Clear"))
                {
                    Nav.Clear();
                }
            }

            EditorGUILayout.ObjectField("Selected GameObject", EventSystem.current?.currentSelectedGameObject, typeof(GameObject), true);
            using (var sv = new GUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = sv.scrollPosition;

                for (int k = 0; k < Nav.navScopes.Count; k++)
                {
                    nav = Nav.navScopes[k].Provider as Navigation;

                    GUILayout.Label($"Scope [{k}]");

                    int i = 1;
                    foreach (var item in nav.list)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            string str;
                            str = $"#{item.Id}";
                            if ((item.Flags & NavFlags.Home) != 0)
                            {
                                str += " [Home]";
                            }
                            if ((item.Flags & NavFlags.DisableBack) != 0)
                            {
                                str += " [DisableBack]";
                            }
                            if (item.ViewResult != null)
                            {
                                if (item.ViewResult.IsLoad)
                                {
                                    str += " [Load]";
                                }
                                else
                                {
                                    str += " [Loading]";
                                }

                                if (item.ViewResult.IsNavEnter)
                                {
                                    str += " [Enter]";
                                }
                                else
                                {
                                    str += " [Leave]";
                                }
                            }

                            GUILayout.Label(str);
                            GUILayout.Space(15);
                            using (new GUILayout.VerticalScope())
                            {
                                GUILayout.Label(item.Url);
                                using (new GUILayout.HorizontalScope())
                                {
                                    GUIContent label = new GUIContent("View");
                                    var result = item.ViewResult;
                                    if (result != null)
                                    {
                                        if (result is GameObjectViewResult goResult)
                                        {

                                            EditorGUILayout.ObjectField(label, goResult.GameObject, typeof(GameObject), true);
                                        }
                                        else
                                        {
                                            EditorGUILayout.LabelField(label, new GUIContent(result.ToString()));
                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField(label, new GUIContent("null"));
                                    }
                                }

                            }
                        }
                    }
                }

            }


            Repaint();
        }
    }
}