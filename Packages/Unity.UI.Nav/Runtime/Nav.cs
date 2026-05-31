using Kuxue.Async;
using Kuxue.UI.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;



namespace Kuxue.UI.Navs
{
    /// <summary>
    /// UI 导航管理器
    /// </summary>
    public class Nav
    {
        private static RouteCollection routes = new RouteCollection();


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

        private static INavigation current;

        public static string CurrentUrl { get { return Current.CurrentUrl; } }

        public static int Count { get { return Current.Count; } }

        public static INavigation Current
        {
            get
            {
                //if (current == null)
                //    throw new Exception("Current null");
                return current;
            }
            internal set
            {
                current = value;
            }
        }

        public static int CurrentId => Current?.Current != null ? Current.Current.Id : 0;


        [Obsolete]
        public static INavigation Root
        {
            get => Current;
        }

        public static RouteCollection Routes { get => routes; }


        public static event Action<NavBackingEventArgs> Backing
        {
            add
            {
                Current.Backing += value;
            }
            remove
            {
                Current.Backing -= value;
            }
        }

        public static event Action<NavBeforeEventArgs> BeforeNavigation
        {
            add
            {
                Current.BeforeNavigation += value;
            }
            remove
            {
                Current.BeforeNavigation -= value;
            }
        }
        public static event Action<NavAfterEventArgs> AfterNavigation
        {
            add
            {
                Current.AfterNavigation += value;
            }
            remove
            {
                Current.AfterNavigation -= value;
            }
        }
        public static bool Initalized { get; private set; }
        internal static List<NavScope> navScopes;
        public static void Initialize()
        {
            if (Initalized)
                return;

            /* NavBehaviour root = new GameObject("[UINav]").AddComponent<NavBehaviour>();
              GameObject.DontDestroyOnLoad(root.gameObject);*/
            Navigation root = new Navigation();
            Initialize(root);
        }

        public static void Initialize(INavigation nav)
        {
            if (nav == null)
                throw new ArgumentNullException(nameof(nav));

            Initalized = true;
            navScopes = new();
            NewScope(nav);
        }

        internal static void Uninitialize()
        {
            Initalized = false;
            for (int i = navScopes.Count - 1; i >= 0; i--)
            {
                navScopes[i].Dispose();
            }
            navScopes.Clear();
        }

        public static IDisposable NewScope(string name = null)
        {
            INavigation provider = new Navigation();
            return NewScope(provider, name);
        }

        public static IDisposable NewScope(INavigation provider, string name = null)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            NavScope scope = new NavScope(name, provider);

            return scope;
        }


        public static int Navigate(NavMode mode, string url, object parameter = null)
        {
            NavOptions options = new NavOptions()
            {
                Mode = mode,
                Url = url,
                Parameter = parameter
            };
            return Navigate(options);
        }
        public static int Navigate(NavOptions options)
        {
            return Current.Navigate(options);
        }
        public static async Task<int> NavigateAsync(NavOptions options)
        {
            int id = Current.Navigate(options);
            if (options.Mode == NavMode.Push || options.Mode == NavMode.Replace)
            {
                if (id > 0)
                {
                    await new WaitUntil(() => IsLoad(id));
                }
            }
            return id;
        }

        public static int Push(string url, object parameter = null, NavFlags flags = 0)
        {
            NavOptions options = new NavOptions()
            {
                Mode = NavMode.Push,
                Url = url,
                Parameter = parameter,
                Flags = flags
            };
            return Navigate(options);
        }

        public static int Push(string url, NavFlags flags)
        {
            NavOptions options = new NavOptions()
            {
                Mode = NavMode.Push,
                Url = url,
                Flags = flags
            };
            return Navigate(options);
        }

        public static int PushHome(string url, object parameter = null, NavFlags flags = 0)
        {
            NavOptions options = new NavOptions()
            {
                Mode = NavMode.Push,
                Url = url,
                Parameter = parameter,
                Flags = flags | NavFlags.Home,
            };
            return Navigate(options);
        }


        public static async Task<int> PushAsync(string url, object parameter = null, NavFlags flags = 0)
        {
            int id = Push(url, parameter, flags);
            await new WaitUntil(() => IsLoad(id));
            return id;
        }

        public static async Task<int> PushAsync(string url, NavFlags flags)
        {
            int id = Push(url, flags);
            await new WaitUntil(() => IsLoad(id));
            return id;
        }

        public static async Task<int> PushHomeAsync(string url, object parameter = null, NavFlags flags = 0)
        {
            int id = PushHome(url, parameter, flags);
            await new WaitUntil(() => IsLoad(id));
            return id;
        }

        public static int Replace(string url, object parameter = null, NavFlags flags = 0)
        {
            NavOptions options = new NavOptions()
            {
                Mode = NavMode.Replace,
                Url = url,
                Parameter = parameter
            };
            return Navigate(options);
        }
        public static int Replace(string url, NavFlags flags)
        {
            return Replace(url, null, flags);
        }

        public static async Task<int> ReplaceAsync(string url, object parameter = null, NavFlags flags = 0)
        {
            int id = Replace(url, parameter, flags);
            await new WaitUntil(() => IsLoad(id));
            return id;
        }
        public static async Task<int> ReplaceAsync(string url, NavFlags flags)
        {
            int id = Replace(url, null, flags);
            await new WaitUntil(() => IsLoad(id));
            return id;
        }

        public static int Back()
        {
            string url = CurrentUrl;
            var ctx = Current.FindByUrl(url);
            if (ctx == null) return 0;
            Back(ctx.Id);
            return ctx.Id;
        }

        public static bool Back(INavigable view)
        {
            if (view == null) return false;
            return Back(view.ViewId);
        }

        public static bool Back(string url)
        {
            var ctx = Current.FindByUrl(url);
            if (ctx == null) return false;
            return Back(ctx.Id);
        }
        public static bool Back(int id)
        {
            return Current.Back(id);
        }
        public static async Task<bool> BackAsync(int id)
        {
            bool suc = Current.Back(id);
            if (suc)
            {
                await new WaitUntil(() => IsUnload(id));
            }
            return suc;
        }
        public static async Task<int> BackAsync()
        {
            int id = Back();
            if (id > 0)
            {
                await new WaitUntil(() => IsUnload(id));
            }
            return id;
        }

        public static void BackTo(string url)
        {
            var ctx = Current.FindByUrl(url);
            if (ctx == null) return;
            BackTo(ctx.Id);
        }

        public static void BackTo(int id)
        {
            Current.BackTo(id);
        }
        public static void BackTo(INavigable view)
        {
            if (view == null) return;
            BackTo(view.ViewId);
        }

        public static void BackToHome()
        {
            Current.BackToHome();
        }

        public static bool Contains(string url)
        {
            return Current.FindByUrl(url) != null;
        }
        public static bool Contains(int id)
        {
            return Current.FindById(id) != null;
        }

        public static bool IsLoad(int id)
        {
            var ctx = Current.FindById(id);
            if (ctx == null) return false;
            if (ctx.ViewResult == null) return false;
            return ctx.ViewResult.IsLoad;
        }
        public static bool IsUnload(int id)
        {
            var ctx = Current.FindById(id);
            if (ctx == null) return true;
            if (ctx.ViewResult == null) return true;
            return ctx.ViewResult.IsUnload;
        }
        public static bool Remove(string url)
        {
            var ctx = Current.FindByUrl(url);
            if (ctx == null) return false;
            return Remove(ctx.Id);
        }
        public static bool Remove(int id)
        {
            return Current.Remove(id);
        }

        public static void Clear()
        {
            Current.Clear();
        }
        //public static bool IsEnter()
        //{
        //    return Current.IsEnter();
        //}

        public static NavFlags GetFlags(int id)
        {
            var context = Current?.FindById(id);
            if (context == null) return 0;
            return context.Flags;
        }

        public static void SetHome(int id)
        {
            var context = Current?.FindById(id);
            if (context == null) return;
            context.Flags |= NavFlags.Home;
        }

        public static void UnsetHome(int id)
        {
            var context = Current?.FindById(id);
            if (context == null) return;
            context.Flags &= ~NavFlags.Home;
        }



        public static bool IsHome(int id)
        {
            return (GetFlags(id) & NavFlags.Home) == NavFlags.Home;
        }

        public static void EnableBack(int id)
        {
            var context = Current?.FindById(id);
            if (context == null) return;
            context.Flags &= ~NavFlags.DisableBack;
        }

        public static void DisableBack(int id)
        {
            var context = Current?.FindById(id);
            if (context == null) return;
            context.Flags |= NavFlags.DisableBack;
        }

    }



    public class NavBeforeEventArgs : EventArgs
    {
        internal NavBeforeEventArgs(INavigation nav, NavMode mode, string url, object parameter)
        {
            this.Navigation = nav;
            this.Url = url;
            this.Mode = mode;
            this.Parameter = parameter;
        }

        public INavigation Navigation { get; private set; }

        public NavMode Mode { get; private set; }

        public string Url { get; private set; }

        public object Parameter { get; private set; }


        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }

    public class NavAfterEventArgs : EventArgs
    {
        internal NavAfterEventArgs(INavigation nav, NavMode mode, string url, object parameter)
        {
            this.Navigation = nav;
            this.Url = url;
            this.Mode = mode;
            this.Parameter = parameter;
        }

        public INavigation Navigation { get; private set; }

        public NavMode Mode { get; private set; }

        public string Url { get; private set; }

        public object Parameter { get; private set; }
    }

    public class NavOptions
    {
        public NavMode Mode { get; set; }
        public string Url { get; set; }
        public object Parameter { get; set; }
        public NavFlags Flags { get; set; }

        public int ViewId { get; set; }

    }

}