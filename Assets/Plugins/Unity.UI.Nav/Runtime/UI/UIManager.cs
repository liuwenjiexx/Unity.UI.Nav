using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Navs.Routing;

namespace UnityEngine.UI.Navs
{
    public class UIManager : MonoBehaviour
    {
        [NonSerialized]
        public List<Transform> uiLayers;

        private static UIManager instance;

        /// <summary>
        /// 基础层，Home层
        /// </summary>
        public const int LAYER_Home = 0;
        /// <summary>
        /// 默认层
        /// </summary>
        public const int LAYER_DEFAULT = 1;
        /// <summary>
        /// 对话框层
        /// </summary>
        public const int LAYER_DIALOG = 2;
        /// <summary>
        /// tips层
        /// </summary>
        public const int LAYER_TIPS = 3;

        /// <summary>
        /// 肯定按钮：Yes
        /// </summary>
        public const int BUTTON_POSITIVE = 0x1;
        /// <summary>
        /// 中立按钮，Cancel
        /// </summary>
        public const int BUTTON_NEUTRAL = 0x2;
        /// <summary>
        /// 否定按钮，No
        /// </summary>
        public const int BUTTON_NEGATIVE = 0x3;




        public static UIManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject("[UI]").AddComponent<UIManager>();
                    GameObject.DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (!instance)
            {
                instance = this;

                uiLayers = new List<Transform>();
                uiLayers.Add(CreateLayer(LAYER_Home));
                uiLayers.Add(CreateLayer(LAYER_DEFAULT));
                uiLayers.Add(CreateLayer(LAYER_DIALOG));
                uiLayers.Add(CreateLayer(LAYER_TIPS));

                //初始化UI导航
                if (!Nav.Initalized)
                {
                    var nav = instance.gameObject.GetComponent<NavBehaviour>();
                    if (!nav)
                        nav = instance.gameObject.AddComponent<NavBehaviour>();
                    Nav.Initialize(nav);
                    Nav.Root.Routes.Add("default", new Route(NavRouteHandler.DefaultPattern, new NavRouteHandler()));
                }
            }
        }

        protected virtual Transform CreateLayer(int layer)
        {
            GameObject go = new GameObject($"{layer}");
            go.transform.parent = transform;
            go.AddComponent<Canvas>();
            go.AddComponent<GraphicRaycaster>();
            var canvas = go.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100 * layer;
            RectTransform trans = go.GetComponent<RectTransform>();
            trans.localScale = Vector3.one;
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.offsetMin = Vector3.zero;
            trans.offsetMax = Vector3.zero;
            return go.transform;
        }


        public static int Open(string url, object parameter = null)
        {
            return Open(LAYER_DEFAULT, url, parameter);
        }
        public static int Open(int layer, string url, object parameter = null)
        {
            var dic = Route.ObjectToDictionary(parameter);
            dic["__layer"] = layer;
            return Nav.Push(url, dic);
        }

        public static int OpenHome(string url, object parameter = null)
        {
            int id = Open(LAYER_Home, url, parameter);
            Nav.SetHome(id);
            return id;
        }

        /// <summary>
        /// 打开<see cref="UILayer.Dialog"/>对话框层
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        public static void Dialog(string url, object parameter)
        {
            Open(LAYER_DIALOG, url, parameter);
        }


        public static void DialogOK(string content, DialogClickDelegate onClick = null)
        {
            DialogOK(content, null, onClick);
        }

        public static void DialogOK(string content, string title, DialogClickDelegate onClick = null)
        {
            Dialog("Dialog/OK", new { content, title, onClick });
        }

        public static void DialogYesNo(string content, DialogClickDelegate onClick)
        {
            DialogYesNo(content, null, onClick);
        }
        public static void DialogYesNo(string content, string title, DialogClickDelegate onClick)
        {
            Dialog("Dialog/YesNo", new { content, title, onClick });
        }

        public static void Tips(string content)
        {
            Dialog("Tips", new { content });
        }


        public static void ShowSceneLoading()
        {
        }

        public static void CloseSceneLoading()
        {
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="url"></param> 
        public static void Close(int id)
        {
            Nav.Remove(id);
        }

        /// <summary>
        /// 回到主界面
        /// </summary>
        public static void BackToHome()
        {
            Nav.BackToHome();
        }

        public static void Back()
        {
            Nav.Back();
        }

    }

    public delegate void DialogClickDelegate(int button);

}