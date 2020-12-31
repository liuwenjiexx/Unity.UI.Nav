using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Navs
{
    /// <summary>
    /// UI 导航管理器
    /// </summary>
    public class Nav
    {
        //NavScope
        static bool? logEnabled;

        /// <summary>
        /// UINavBehaviour Log开关
        /// </summary>
        public static bool LogEnabled
        {
            get
            {
                if (logEnabled == null)
                {
                    logEnabled = PlayerPrefs.GetInt("UI.Navs.LogEnabled") != 0;
                }
                return logEnabled.Value;
            }
            set
            {
                if (logEnabled != value)
                {
                    logEnabled = value;
                    PlayerPrefs.SetInt("UI.Navs.LogEnabled", logEnabled.Value ? 1 : 0);
                    PlayerPrefs.Save();
                }
            }
        }
        //private static IUINavProvider provider;

        //public static IUINavProvider Provider
        //{
        //    get
        //    {
        //        if (provider == null)
        //            throw new Exception("Uninitialized");
        //        return provider;
        //    }
        //}

        private static INav root;

        public static string Current { get { return Root.Current; } }

        public static int Count { get { return Root.Count; } }

        public static INav Root
        {
            get
            {
                if (root == null)
                    throw new Exception("root null");
                return root;
            }
        }

        public static event Action<NavBackingEventArgs> Backing
        {
            add
            {
                Root.Backing += value;
            }
            remove
            {
                Root.Backing -= value;
            }
        }

        public static event Action<UINavBeforeEventArgs> BeforeNavigation
        {
            add
            {
                Root.BeforeNavigation += value;
            }
            remove
            {
                Root.BeforeNavigation -= value;
            }
        }
        public static bool Initalized { get; private set; }
        public static void Initialize()
        {
            if (Initalized)
                return;
            NavBehaviour root = new GameObject("[UINav]").AddComponent<NavBehaviour>();
            GameObject.DontDestroyOnLoad(root.gameObject);
            Initialize(root);
        }

        public static void Initialize(INav root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            Nav.root = root;
            Initalized = true;
        }

        public static int Navigation(NavMode mode, string url, object parameter = null)
        {
            return Root.Navigation(mode, url, parameter);
        }
        public static int Push(string url, object parameter = null)
        {
            return Root.Push(url, parameter);
        }

        public static int Replace(string url, object parameter = null)
        {
            return Root.Replace(url, parameter);
        }

        public static bool Back()
        {
            string current = Current;
            if (!string.IsNullOrEmpty(current))
            {
                return Root.Back(current);
            }
            return false;
        }

        public static bool Back(string name)
        {
            return Root.Back(name);
        }
        public static bool Back(int id)
        {
            return Root.Back(id);
        }

        public static void BackTo(string name)
        {
            Root.BackTo(name);
        }

        public static void BackTo(int id)
        {
            Root.BackTo(id);
        }

        public static void BackToHome()
        {
            Root.BackToHome();
        }

        public static bool Contains(string name)
        {
            return Root.Contains(name);
        }
        public static bool Contains(int id)
        {
            return Root.Contains(id);
        }

        public static bool Remove(string name)
        {
            return Root.Remove(name);
        }
        public static bool Remove(int id)
        {
            return Root.Remove(id);
        }

        public static void Clear()
        {
            Root.Clear();
        }
        public static bool IsEnter()
        {
            return Root.IsEnter();
        }

        public static void SetHome(int id, bool isHome = true)
        {
            Root.SetHome(id, isHome);
        }

        public static bool IsHome(int id)
        {
            return (Root.GetFlags(id) & UINavFlags.Home) == UINavFlags.Home;
        }

        //[System.Diagnostics.Conditional("UINAV_LOG")]
        public static void Log(string message)
        {
            if (LogEnabled)
            {
                Debug.Log("[Nav]: " + message);
            }
        }

    }



    public class UINavBeforeEventArgs : EventArgs
    {
        internal UINavBeforeEventArgs(INav nav, NavMode mode, string url, object parameter)
        {
            this.Navigation = nav;
            this.Url = url;
            this.Mode = mode;
            this.Parameter = parameter;
        }

        public INav Navigation { get; private set; }

        public NavMode Mode { get; private set; }

        public string Url { get; private set; }

        public object Parameter { get; private set; }

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }


    public enum UINavFlags
    {
        None,
        Home,
    }
}