using System;
using System.Collections.Generic;
using Unity.UI.Routing;
using UnityEngine;



namespace Unity.UI.Navs
{


    public class Navigation : INavigation
    {
        private int nextId;
        public LinkedList<_Context> list = new LinkedList<_Context>();
        private RouteCollection routes = new RouteCollection();


        public class _Context : NavContext
        {
            public _Context(INavigation nav)
                : base(nav)
            {

            }
            public UINavStatus status;
            //public bool enter;
            //public bool instanceEnter;
            //public bool instanceExit;
            public object parameter;
            public NavMode mode;

            public _Context to;
            public _Context from;
            public bool hasState;

            public object State { get; set; }
            public bool IsHome
            {
                get => (Flags & NavFlags.Home) == NavFlags.Home;
            }

            public override string ToString()
            {
                return $"{Url}:{status}";
            }
        }


        public event Action<NavBackingEventArgs> Backing;

        public event Action<NavBeforeEventArgs> BeforeNavigation;

        public event Action<NavAfterEventArgs> AfterNavigation;
        //private static NavProvider instance;

        //public IUINavProvider Provider { get; set; }

        /*
        public static NavBehaviour Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject("[" + typeof(Nav).Name + "]").AddComponent<NavBehaviour>();
                    GameObject.DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
   


        public static NavBehaviour Get(GameObject go)
        {
            var nav = go.GetComponentInParent<NavBehaviour>();
            if (!nav)
                nav = Instance;
            return nav;
        }
             */
        //private IUINavProvider GetProvider()
        //{
        //    if (Provider != null)
        //        return Provider;
        //    return UINav.Provider;
        //}



        public int Navigate(NavOptions options)
        {
            NavMode mode = options.Mode;
            NavUtility.Log($"[Navigate] [{mode}] url: <{options.Url}>");


            if (mode == NavMode.Back)
            {
                if (options.ViewId > 0)
                {
                    Back(options.ViewId);
                }
                else if (!string.IsNullOrEmpty(options.Url))
                {
                    Back(options.Url);
                }
                else
                {
                    Back();
                }
                return 0;
            }
            else if (mode == NavMode.BackUntil)
            {
                if (options.ViewId > 0)
                {
                    BackTo(options.ViewId);
                }
                else
                {
                    BackTo(options.Url);
                }
                return 0;
            }

            if (BeforeNavigation != null)
            {
                var e = new NavBeforeEventArgs(this, mode, options.Url, options.Parameter);
                BeforeNavigation.Invoke(e);
                if (e.IsCanceled)
                    return 0;
            }
            _Context state = new _Context(this);
            try
            {
                var context = state;
                state.Url = options.Url;
                state.status = UINavStatus.WaitEnter;
                state.parameter = options.Parameter;
                state.Id = ++nextId;
                state.mode = mode;
                state.Flags = options.Flags;
                var routeData = Routes.GetRouteData(state.Url, state.parameter);

                if (routeData == null)
                    routeData = Nav.Routes.GetRouteData(state.Url, state.parameter);
                if (routeData == null)
                    throw new RouteException(state.Url);

                state.RouteData = routeData;

                _Context from = null;

                if (mode == NavMode.Replace)
                {
                    if (list.Count > 0 && !list.Last.Value.IsHome)
                    {
                        from = list.Last?.Value;
                        if (CanBack(from))
                        {
                            Unload(list.Last, mode, state);
                        }
                        else
                        {
                            from = null;
                        }
                    }

                }

                var node = list.AddLast(state);

                Load(node, from);

                if (from == null)
                {
                    from = node.Previous?.Value;
                    if (from != null)
                    {
                        OnNavExit(from, mode, state, false);
                    }
                }

                OnNavEnter(context, mode, from);
                AfterNavigation?.Invoke(new NavAfterEventArgs(this, mode, context.Url, context.parameter));
            }catch(Exception ex)
            {
                Debug.LogException(ex); 
            }
            return state.Id;
        }
        private _Context currentHandle;
        private void Load(LinkedListNode<_Context> node, NavContext from)
        {
            if (currentHandle != null)
                return;
            NavMode mode = node.Value.mode;
            var prev = node.Previous;
            //if (prev != null)
            //{
            //    if (prev.Value.status == UINavStatus.WaitEnter || prev.Value.status == UINavStatus.WaitExit)
            //    {
            //        Load(prev);
            //        return;
            //    }
            //}

            var context = node.Value;

            currentHandle = context;

            context.status = UINavStatus.Enter;

            var routeData = context.RouteData;
            NavRouteHandler routeHandler = (NavRouteHandler)routeData.RouteHandler;
            Controller controller;
            try
            {
                routeHandler.ProcessRoute(context);
                controller = routeHandler.GetController(context); 
                controller.Initialize(context);

                context.ViewResult = controller.GetActionResult(context.ActionName, context);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                currentHandle = null;
            }


        }

        void OnNavEnter(_Context current, NavMode mode, NavContext from)
        {
            //if (Nav.LogEnabled)
            //    Nav.Log($"OnNavEnter <{mode}> <{current.Url}>   \n<= <{from?.Url}>");

            current.Controller.OnEnter(mode, from);

            //if (current.hasState)
            //{
            //    if (UINav.LogEnabled)
            //        UINav.Log("LoadState: " + current.Url);
            //    current.Controller.OnLoadState(current.State);
            //    current.hasState = false;
            //}
        }
        void OnNavExit(_Context current, NavMode mode, NavContext to, bool unload)
        {
            //if (Nav.LogEnabled)
            //    Nav.Log($"OnNavExit <{mode}> <{current.Url}> unload: {unload} \n=> <{to?.Url}>");

            //if (mode == NavMode.Push)
            //{
            //    if (UINav.LogEnabled)
            //        UINav.Log("SaveState: " + current.Url);
            //    current.State = current.Controller.OnSaveState();
            //    current.hasState = true;
            //}
            current.Controller.OnExit(mode, to, unload);
        }
        private _Context CurrentState
        {
            get
            {
                var current = list.Last;
                if (current != null)
                {
                    return current.Value;
                }
                return null;
            }
        }
        public NavContext Current
        {
            get
            {
                return CurrentState;
            }
        }
        public string CurrentUrl
        {
            get
            {
                var state = CurrentState;
                if (state == null)
                    return null;
                return state.Url;
            }
        }

        UINavStatus PeekStatus()
        {
            var state = CurrentState;
            if (state == null)
                return (UINavStatus)0;
            return state.status;
        }

        //public bool IsEntering
        //{
        //    get
        //    {
        //        var state = CurrentState;
        //        if (state == null)
        //            return false;
        //        return state.status == NavStatus.Entering;
        //    }
        //}
        public bool IsEnteringOrExiting
        {
            get
            {
                var state = CurrentState;
                if (state == null)
                    return false;
                return state.status == UINavStatus.Entering || state.status == UINavStatus.Exiting;
            }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public RouteCollection Routes { get => routes; }

        public IEnumerable<NavContext> Contexts => list;



        /*
private void Awake()
{
if (!instance)
instance = this;

}
*/


        public bool Contains(string url)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.Url == url)
                    return true;
                current = current.Previous;
            }
            return false;
        }

        public bool Contains(int id)
        {
            var current = list.First;
            while (current != null)
            {
                if (current.Value.Id == id)
                    return true;
                current = current.Next;
            }
            return false;
        }


        private bool CanBack(_Context state)
        {
            if ((state.Flags & NavFlags.Home) != 0)
                return false;
            if ((state.Flags & NavFlags.DisableBack) != 0)
                return false;
            NavBackingEventArgs args = new NavBackingEventArgs(this, state, state.Url);
            if (Backing != null)
            {
                Backing(args);
                if (args.IsCanceled)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 卸载当前的UI
        /// </summary>
        /// <param name="name"></param>
        public bool Back()
        {
            var node = list.Last;
            if (node == null)
                return false;
            var context = node.Value;
            if (CanBack(context))
            {
                var to = node.Previous?.Value;
                Unload(node, NavMode.Back, to);
                if (to != null)
                {
                    OnNavEnter(to, NavMode.Back, context);
                }

                return true;
            }
            return false;
        }


        public bool Back(string url)
        {
            var current = list.Last;
            if (current == null)
                return false;

            if (current.Value.Url == url)
            {
                if (CanBack(current.Value))
                {
                    var to = current.Previous?.Value;
                    Unload(current, NavMode.Back, to);
                    if (to != null)
                    {
                        OnNavEnter(to, NavMode.Back, current.Value);
                    }
                    return true;
                }
            }
            return false;
        }


        public bool Back(int id)
        {
            var current = list.Last;
            if (current == null)
                return false;

            if (current.Value.Id == id)
            {
                if (CanBack(current.Value))
                {
                    var to = current.Previous?.Value;
                    Unload(current, NavMode.Back, to);
                    if (to != null)
                    {
                        OnNavEnter(to, NavMode.Back, current.Value);
                    }
                    return true;
                }
            }
            return false;
        }


        public bool BackTo(string url)
        {
            var target = FindByUrl(url);
            if (target == null) return false;
            BackTo((_Context)target);

            if (Current == target)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public bool BackTo(int id)
        {
            var target = FindById(id);
            if (target == null) return false;
            BackTo((_Context)target);
            if (Current == target)
                return true;
            return false;
        }

        void BackTo(_Context target)
        {
            var current = list.Last;
            _Context to = null;
            while (current != null)
            {
                var prev = current.Previous;
                if (current.Value == target)
                    break;
                if (!CanBack(current.Value))
                    break;
                to = current.Previous?.Value;
                Unload(current, NavMode.Back, to);
                if (to != null)
                {
                    OnNavEnter(to, NavMode.Back, current.Value);
                }
                current = prev;
            }

        }

        public void BackToHome()
        {
            var current = list.Last;
            while (current != null)
            {
                var prev = current.Previous;

                if (!CanBack(current.Value))
                    break;
                var to = current.Previous?.Value;
                Unload(current, NavMode.Back, to);
                if (to != null)
                {
                    OnNavEnter(to, NavMode.Back, current.Value);
                }
                current = prev;
            }
        }


        public bool Remove(string url)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.Url == url)
                {
                    _Context to = null;
                    if (current.Next == null)
                        to = current.Previous?.Value;
                    Unload(current, NavMode.Back, to);
                    if (to != null)
                    {
                        OnNavEnter(to, NavMode.Back, current.Value);
                    }
                    return true;
                }
                current = current.Previous;
            }
            return false;
        }
        public bool Remove(int id)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.Id == id)
                {
                    _Context to = null;
                    if (current.Next == null)
                        to = current.Previous?.Value;
                    Unload(current, NavMode.Back, null);
                    if (to != null)
                    {
                        OnNavEnter(to, NavMode.Back, current.Value);
                    }
                    return true;
                }
                current = current.Previous;
            }
            return false;
        }

        /// <summary>
        /// 卸载所有UI
        /// </summary>
        public void Clear()
        {
            var current = list.Last;
            while (current != null)
            {
                var prev = current.Previous;
                //if (!CanBack(current.Value))
                //    break;
                //var to = current.Previous?.Value;
                Unload(current, NavMode.Back, null);
                current = prev;
            }
        }

        public bool IsEntering()
        {
            return PeekStatus() == UINavStatus.Entering;
        }

        public bool IsEnter()
        {
            return PeekStatus() == UINavStatus.Enter;
        }
        public bool IsExiting()
        {
            return PeekStatus() == UINavStatus.Exiting;
        }

        //public void Exit(string name)
        //{
        //    var current = queue.Last;
        //    while (current != null)
        //    {
        //        if (current.Value[i].name == name)
        //        {
        //            break
        //        }
        //        current = current.Previous;
        //    } 
        //    for (int i = queue.Count - 1; i >= 0; i--)
        //    {
        //        if (queue[i].name == name)
        //        {
        //            if (CanBack(queue[i]))
        //                OnExitAt(i);
        //        }
        //    }
        //}


        private void Unload(LinkedListNode<_Context> node, NavMode mode, _Context to)
        {
            var state = node.Value;
            /* if ((state.status == UINavStatus.WaitExit) || (state.status == UINavStatus.Exiting) || (state.status == UINavStatus.Exit))
             {
                 return;
             }*/
            //if (Nav.LogEnabled)
            //    Nav.Log(GetType().Name + " Exit " + state.url);
            state.status = UINavStatus.WaitExit;

            var context = state;


            OnNavExit(context, mode, to, true);
            context.status = UINavStatus.Exit;

            node.List.Remove(node);
            AfterNavigation?.Invoke(new NavAfterEventArgs(this, mode, context.Url, context.parameter));
        }

        private void OnExit(_Context state)
        {
            if (state == null)
                return;
            if (state.status == UINavStatus.Exiting || state.status == UINavStatus.Exit)
                return;

            if (state.ViewResult != null)
            {
                var instance = state.ViewResult;
                state.status = UINavStatus.Exiting;
                //state.controller.OnExit(state, () =>
                {
                    if (state.status == UINavStatus.Exiting)
                    {
                        state.status = UINavStatus.Exit;
                    }
                }//);
            }

        }
        /*
        public void SetFlags(int id, NavFlags flags)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.id == id)
                {
                    current.Value.Flags = flags;
                    return;
                }
                current = current.Previous;
            }
            throw new Exception("missing: " + id);
        }
        */
        public NavContext FindById(int id)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.Id == id)
                {
                    return current.Value;
                }
                current = current.Previous;
            }
            return null;
        }
        public NavContext FindByUrl(string url)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.Url == url)
                {
                    return current.Value;
                }
                current = current.Previous;
            }
            return null;
        }
        public NavFlags GetFlags(int id)
        {
            //var current = list.Last;
            //while (current != null)
            //{
            //    if (current.Value.id == id)
            //    {
            //        return current.Value.flags;
            //    }
            //    current = current.Previous;
            //}
            //throw new Exception("missing: " + id);
            var ctx = FindById(id);
            if (ctx != null) return ctx.Flags;
            return 0;
        }

        public void SetHome(int id, bool isHome)
        {
            /*   var current = list.Last;
               while (current != null)
               {
                   if (current.Value.id == id)
                   {
                       if (isHome)
                           current.Value.flags |= UINavFlags.Home;
                       else
                           current.Value.flags = (~UINavFlags.Home) & current.Value.flags;
                       return;
                   }
                   current = current.Previous;
               }
               throw new Exception("missing: " + id);*/
        }

        //public UINavFlags AddFlags(int id, UINavFlags flags)
        //{

        //}

        //public UINavFlags RemoveFlags(int id, UINavFlags flags)
        //{

        //}

        public enum UINavStatus
        {
            None = 0,
            WaitEnter,
            Entering,
            Enter,
            WaitExit,
            Exiting,
            Exit,
        }

    }



}