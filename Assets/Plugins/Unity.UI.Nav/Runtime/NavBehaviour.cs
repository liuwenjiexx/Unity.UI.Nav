using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI.Navs.Routing;
using UnityEngine;



namespace UnityEngine.UI.Navs
{


    public class NavBehaviour : MonoBehaviour, INav
    {
        private int nextId;
        public LinkedList<_Context> list = new LinkedList<_Context>();
        private RouteCollection routes = new RouteCollection();


        public class _Context : Context
        {

            public UINavStatus status;
            public bool enter;
            public bool instanceEnter;
            public bool instanceExit;
            public object parameter;
            public UINavFlags flags;

            public _Context replaceTo;
            public _Context replaceFrom;
            public bool hasState;


            private Controller controller;

            internal Controller Controller { get => controller; set => controller = value; }

            public object State { get; set; }
            public bool IsHome
            {
                get => (flags & UINavFlags.Home) == UINavFlags.Home;
            }

            public override string ToString()
            {
                return $"{Url}:{status}";
            }
        }


        public event Action<NavBackingEventArgs> Backing;

        public event Action<UINavBeforeEventArgs> BeforeNavigation;

        private static NavBehaviour instance;

        //public IUINavProvider Provider { get; set; }


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

        //private IUINavProvider GetProvider()
        //{
        //    if (Provider != null)
        //        return Provider;
        //    return UINav.Provider;
        //}



        public int Navigation(NavMode mode, string url, object parameter = null)
        {

            if (Nav.LogEnabled)
                Nav.Log($"Navigation <{mode}> url: <{url}>");

            if (BeforeNavigation != null)
            {
                var e = new UINavBeforeEventArgs(this, mode, url, parameter);
                BeforeNavigation(e);
                if (e.IsCanceled)
                    return 0;
            }

            if (mode == NavMode.Back)
            {
                if (string.IsNullOrEmpty(url))
                {
                    Back();
                }
                else
                {
                    Back(url);
                }
                return 0;
            }
            else if (mode == NavMode.BackUntil)
            {
                BackTo(url);
                return 0;
            }

            _Context state = new _Context();
            state.Url = url;
            state.status = UINavStatus.WaitEnter;
            state.parameter = parameter;
            state.Id = ++nextId;

            if (mode == NavMode.Replace)
            {
                if (list.Count > 0 && !list.Last.Value.IsHome)
                {
                    //OnExitAt(queue.Count - 1);
                    list.Last.Value.replaceTo = state;
                    state.replaceFrom = list.Last.Value;
                    OnExit(list.Last, mode);
                }
            }


            var node = list.AddLast(state);

            Handle(node, mode);

            return state.id;
        }
        private _Context currentHandle;
        private void Handle(LinkedListNode<_Context> node, NavMode mode)
        {
            if (currentHandle != null)
                return;
            var prev = node.Previous;
            if (prev != null)
            {
                if (prev.Value.status == UINavStatus.WaitEnter || prev.Value.status == UINavStatus.WaitExit)
                {
                    Handle(prev, mode);
                    return;
                }
            }

            var context = node.Value;
            if (context.status == UINavStatus.WaitEnter)
            {
                currentHandle = context;

                context.status = UINavStatus.Entering;
                context.enter = true;


                var routeData = Nav.Root.Routes.GetRouteData(context.Url, context.parameter);

                context.RouteData = routeData;

                NavRouteHandler routeHandler = (NavRouteHandler)routeData.RouteHandler;
                Controller controller;
                try
                {
                    routeHandler.ProcessRoute(context);
                    controller = routeHandler.GetController(context);
                    context.Controller = controller;
                    controller.Context = context;

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
                //    controller.OnEnter(state, () =>
                {


                    var prev2 = node.Previous;
                    var replaceFrom = context.replaceFrom;
                    context.replaceFrom = null;
                    if (prev2 != null && replaceFrom == null)
                    {
                        if (prev2.Value.instanceEnter && !prev2.Value.instanceExit)
                        {
                            prev2.Value.instanceEnter = false;
                            prev2.Value.instanceExit = true;

                            OnNavExit(prev2.Value, mode, context);

                            //if (prev2.Value.View != null)
                            //{
                            //    var l = prev2.Value.View as IUINavListener;

                            //    if (l != null)
                            //    {
                            //        try
                            //        {
                            //            l.OnUINavExit();
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            Debug.LogException(ex);
                            //        }
                            //    }
                            //}
                        }
                    }

                    if (context.status == UINavStatus.Entering)
                    {
                        context.status = UINavStatus.Enter;
                    }
                    if (!context.instanceEnter)
                    {
                        context.instanceEnter = true;
                        context.instanceExit = false;


                        if (replaceFrom != null)
                        {
                            OnNavEnter(context, mode, replaceFrom);
                        }
                        else if (node.Previous != null)
                        {
                            OnNavEnter(context, mode, node.Previous.Value);
                        }
                        else
                        {
                            OnNavEnter(context, mode, null);
                        }

                        //if (state.View != null)
                        //{
                        //    var l = state.View as IUINavListener;
                        //    if (l != null)
                        //    {
                        //        try
                        //        {
                        //            l.OnUINavEnter();
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Debug.LogException(ex);
                        //        }
                        //    }
                        //}
                    }
                    if (node.Next != null)
                        Handle(node.Next, mode);
                }
                //);
            }
            else if (context.status == UINavStatus.WaitExit)
            {

                if (context.enter)
                {
                    currentHandle = context;
                    var instance = context.ViewResult;

                    context.status = UINavStatus.Exiting;
                    var replaceTo = context.replaceTo;
                    context.replaceTo = null;
                    if (context.instanceEnter && !context.instanceExit)
                    {
                        context.instanceEnter = false;
                        context.instanceExit = true;
                        if (replaceTo != null)
                        {
                            OnNavExit(context, mode, replaceTo);
                        }
                        else if (node.Previous != null)
                        {
                            OnNavExit(context, mode, node.Previous.Value);
                        }
                        else
                        {
                            OnNavExit(context, mode, null);
                        }



                        //if (instance != null)
                        //{
                        //    var l = instance as IUINavListener;
                        //    if (l != null)
                        //    {
                        //        try
                        //        {
                        //            l.OnUINavExit();
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Debug.LogException(ex);
                        //        }
                        //    }
                        //}
                    }


                    //   state.controller.OnExit(state, () =>
                    {
                        currentHandle = null;

                        context.status = UINavStatus.Exit;


                        var prev2 = node.Previous;
                        if (prev2 != null && replaceTo == null)
                        {
                            if (!prev2.Value.instanceEnter && prev2.Value.instanceExit)
                            {
                                prev2.Value.instanceEnter = true;
                                prev2.Value.instanceExit = false;


                                OnNavEnter(prev2.Value, mode, context);

                                //if (prev2.Value.View != null)
                                //{
                                //    var l = prev2.Value.View as IUINavListener;

                                //    if (l != null)
                                //    {
                                //        try
                                //        {
                                //            l.OnUINavEnter();
                                //        }
                                //        catch (Exception ex)
                                //        {
                                //            Debug.LogException(ex);
                                //        }
                                //    }
                                //}
                            }
                        }

                        var next = node.Next;
                        if (next == null)
                            next = node.Previous;
                        node.List.Remove(node);

                        if (next != null)
                            Handle(next, mode);
                    }//);
                }
                else
                {
                    context.status = UINavStatus.Exit;
                    var next = node.Next;
                    if (next == null)
                        next = node.Previous;
                    node.List.Remove(node);
                    if (next != null)
                        Handle(next, mode);
                }
            }
        }

        void OnNavEnter(_Context current, NavMode mode, Context from)
        {
            if (Nav.LogEnabled)
                Nav.Log($"OnNavEnter <{mode}> <{current.Url}>   <= <{ from?.Url}>");

            current.Controller.OnEnter(mode, from);

            //if (current.hasState)
            //{
            //    if (UINav.LogEnabled)
            //        UINav.Log("LoadState: " + current.Url);
            //    current.Controller.OnLoadState(current.State);
            //    current.hasState = false;
            //}
        }
        void OnNavExit(_Context current, NavMode mode, Context to)
        {
            if (Nav.LogEnabled)
                Nav.Log($"OnNavExit <{mode}> <{current.Url}>  => <{to?.Url}>");

            //if (mode == NavMode.Push)
            //{
            //    if (UINav.LogEnabled)
            //        UINav.Log("SaveState: " + current.Url);
            //    current.State = current.Controller.OnSaveState();
            //    current.hasState = true;
            //}
            current.Controller.OnExit(mode, to);
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

        public string Current
        {
            get
            {
                var state = CurrentState;
                if (state == null)
                    return null;
                return state.url;
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

        private void Awake()
        {
            if (!instance)
                instance = this;

        }



        public bool Contains(string url)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.url == url)
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
                if (current.Value.id == id)
                    return true;
                current = current.Next;
            }
            return false;
        }

        public bool Remove(string url)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.url == url)
                {
                    OnExit(current, NavMode.Back);
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
                if (current.Value.id == id)
                {
                    OnExit(current, NavMode.Back);
                    return true;
                }
                current = current.Previous;
            }
            return false;
        }

        private bool CanBack(_Context state)
        {
            NavBackingEventArgs args = new NavBackingEventArgs(this, state.url);
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
            var current = list.Last;
            if (current == null || current.Value.IsHome)
                return false;

            if (CanBack(current.Value))
            {
                OnExit(current, NavMode.Back);
                return true;
            }
            return false;
        }


        public bool Back(string url)
        {
            var current = list.Last;
            if (current == null || current.Value.IsHome)
                return false;

            if (current.Value.url == url)
            {
                if (CanBack(current.Value))
                {
                    OnExit(current, NavMode.Back);
                    return true;
                }
            }
            return false;
        }


        public bool Back(int id)
        {
            var current = list.Last;
            if (current == null || current.Value.IsHome)
                return false;

            if (current.Value.id == id)
            {
                if (CanBack(current.Value))
                {
                    OnExit(current, NavMode.Back);
                    return true;
                }
            }
            return false;
        }


        public bool BackTo(string url)
        {
            var current = list.Last;
            while (current != null)
            {
                var prev = current.Previous;
                if (current.Value.url == url)
                    return true;
                if (current.Value.IsHome)
                    break;
                if (!CanBack(current.Value))
                    break;
                OnExit(current, NavMode.Back);
                current = prev;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public bool BackTo(int id)
        {
            var current = list.Last;
            while (current != null)
            {
                var prev = current.Previous;
                if (current.Value.id == id)
                    return true;
                if (current.Value.IsHome)
                    break;
                if (!CanBack(current.Value))
                    break;
                OnExit(current, NavMode.Back);
                current = prev;
            }
            return false;
        }


        public void BackToHome()
        {
            var current = list.Last;
            while (current != null)
            {
                var prev = current.Previous;
                if (current.Value.IsHome)
                    break;
                if (!CanBack(current.Value))
                    break;
                OnExit(current, NavMode.Back);
                current = prev;
            }
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
                if (!CanBack(current.Value))
                    break;
                OnExit(current, NavMode.Back);
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


        private void OnExit(LinkedListNode<_Context> node, NavMode mode = NavMode.Back)
        {
            var state = node.Value;
            if (!(state.status == UINavStatus.WaitExit) || (state.status == UINavStatus.Exiting) || (state.status == UINavStatus.Exit))
            {
                if (Nav.LogEnabled)
                    Nav.Log(GetType().Name + " Exit " + state.url);
                state.status = UINavStatus.WaitExit;
                //queue.RemoveAt(index);
                //OnExit(state);
                //state.instance = null;
                Handle(node, mode);
            }
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

        public void SetFlags(int id, UINavFlags flags)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.id == id)
                {
                    current.Value.flags = flags;
                    return;
                }
                current = current.Previous;
            }
            throw new Exception("missing: " + id);
        }

        public UINavFlags GetFlags(int id)
        {
            var current = list.Last;
            while (current != null)
            {
                if (current.Value.id == id)
                {
                    return current.Value.flags;
                }
                current = current.Previous;
            }
            throw new Exception("missing: " + id);
        }

        public void SetHome(int id, bool isHome)
        {
            var current = list.Last;
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
            throw new Exception("missing: " + id);
        }


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