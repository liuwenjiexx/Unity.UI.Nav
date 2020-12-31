using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI.Navs;

namespace UnityEditor.UI.Navs
{
    [CustomEditor(typeof(NavBehaviour))]
    public class UINavBehaviourInspector : Editor
    {
        Vector2 scrollPos;


        public override void OnInspectorGUI()
        {
            NavBehaviour nav = target as NavBehaviour;
            EditorGUILayout.LabelField("Count", nav.Count.ToString());

            Nav.LogEnabled = EditorGUILayout.Toggle("Log", Nav.LogEnabled);

            if (GUILayout.Button("Back"))
            {
                Nav.Back();
            }
            if (GUILayout.Button("Back To Home"))
            {
                Nav.BackToHome();
            }

            using (var sv = new GUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = sv.scrollPosition;

                int i = 1;
                foreach (var item in nav.list)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("" + i++);
                        GUILayout.Space(15);
                        using (new GUILayout.VerticalScope())
                        {
                            GUILayout.Label(item.Url + string.Format(" (#{0}, {1}, {2})", item.Id.ToString(), item.status.ToString(), item.IsHome ? "Home" : ""));
                            using (new GUILayout.HorizontalScope())
                            {
                                GUIContent label = new GUIContent("View");
                                var result = item.ViewResult;
                                if (result != null)
                                {
                                    if (result.GameObject)
                                    {
                                        EditorGUILayout.ObjectField(label, result.GameObject, typeof(Object), true);
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


            Repaint();
        }
    }
}