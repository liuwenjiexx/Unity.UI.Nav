using System;
using System.Collections;
using System.Collections.Generic;
using LWJ.UI.Routing;
using UnityEngine;

namespace LWJ.UI.Navs
{

    /// <summary>
    /// UI 导航加载器
    /// </summary>
    public class ViewResult
    {
        private Action<ViewResult> loadedCallback;
        private Action<ViewResult> unloadedCallback;

        public ViewResult(string viewName, NavContext context)
        {
            Context = context;
            ViewName = viewName;
        }

        public NavContext Context { get; private set; }

        public string ViewName { get; private set; }

        public Dictionary<string, object> ViewData => Context.ViewData;

        /// <summary>
        /// 用户与视图之间传递数据
        /// </summary>
        public object Model { get; set; }

        public INavigable View { get; set; }

        public int SortOrder { get; set; }
        public ViewResultStatus Status { get; private set; }

        public bool IsLoad { get; set; }

        public bool IsUnload { get; set; }
        public bool IsNavEnter { get; set; }
        public bool IsActive { get; set; }
        public NavContext NavTo { get; set; }
        public NavContext NavFrom { get; set; }

        public event Action<ViewResult> Loaded
        {
            add
            {
                if (Status == ViewResultStatus.Loaded)
                {
                    value(this);
                }
                else
                {
                    loadedCallback += value;
                }
            }
            remove
            {
                loadedCallback -= value;
            }
        }
        public event Action<ViewResult> Unloaded
        {
            add
            {
                if (Status == ViewResultStatus.Unloaded)
                {
                    value(this);
                }
                else
                {
                    unloadedCallback += value;
                }
            }
            remove
            {
                unloadedCallback -= value;
            }
        }
        /// <summary>
        /// 加载视图，<see cref="ViewResultStatus.Loading"/> 状态
        /// </summary>
        public virtual void Load(NavContext context)
        {
            if (Status == ViewResultStatus.None || Status == ViewResultStatus.Unloaded)
            {
                Status = ViewResultStatus.Loading;

                NavUtility.Log($"[Loading] {ViewName}");
            }
        }

        /// <summary>
        /// 通知视图加载完成，<see cref="ViewResultStatus.Loaded"/> 状态
        /// </summary>
        protected virtual void OnLoaded()
        {
            if (Status == ViewResultStatus.None || Status == ViewResultStatus.Loading)
            {
                NavUtility.Log($"[Loaded] {ViewName}");
                Status = ViewResultStatus.Loaded;
                var view = View;
                if (!IsLoad)
                {
                    IsLoad = true;
                    if (view != null)
                    {
                        SetViewData();
                        view.OnLoad();
                    }
                }

                if (view != null)
                {

                    if (IsNavEnter)
                    {
                        SetViewData();
                        view.OnNavigationFrom(NavFrom);
                    }
                }
                var tmp = loadedCallback;
                loadedCallback = null;
                tmp?.Invoke(this);
            }

        }


        /// <summary>
        /// 卸载视图，<see cref="ViewResultStatus.Unloading"/> 状态
        /// </summary>
        public virtual void Unload()
        {
            if (!(Status == ViewResultStatus.Unloading || Status == ViewResultStatus.Unloaded))
            {
                NavUtility.Log($"[Unloading] {ViewName}");
                Status = ViewResultStatus.Unloading;

                if (IsLoad)
                {
                    IsLoad = false;
                    if (View != null)
                    {
                        View.OnUnload();
                    }
                }
            }
        }

        /// <summary>
        /// 通知视图卸载完成，<see cref="ViewResultStatus.Unloaded"/> 状态
        /// </summary>
        protected void OnUnloaded()
        {
            if (Status == ViewResultStatus.Unloading)
            {
                NavUtility.Log($"[Unloaded] {ViewName}");
                Status = ViewResultStatus.Unloaded;
                loadedCallback = null;
                if (!IsUnload)
                {
                    IsUnload = true;
                    var tmp = unloadedCallback;
                    unloadedCallback = null;
                    tmp?.Invoke(this);
                }
            }
        }

        //复用界面时需要每次重置数据
        void SetViewData()
        {
            var view = View;
            if (view != null)
            {
                view.SetContext(Context);
                view.ViewData = ViewData;
                view.Model = Model;
            }
        }

        public virtual void OnNavEnter(NavContext from)
        {
            NavFrom = from;
            NavTo = null;

            var view = View;


            if (!IsNavEnter)
            {
                IsNavEnter = true;
                IsActive = true;
                if (IsLoad && view != null)
                {
                    SetViewData();

                    view.OnNavigationFrom(from);
                }
            }
        }

        public virtual void OnNavLeave(NavContext to)
        {
            NavFrom = null;
            NavTo = to;
            if (IsNavEnter)
            {
                IsNavEnter = false;
                //IsActive = active;
                if (IsLoad && View != null)
                {
                    View.OnNavigationTo(NavTo);
                }
            }

        }
        /*
        public virtual void OnActive()
        {
            if (!IsActive && IsNavEnter)
            {
                IsActive = true;
                var view = View;
                if (view != null)
                {
                    view.OnActive();
                }
            }
        }



        public virtual void OnInactive()
        {
            if (IsActive && IsNavEnter)
            {
                IsActive = false ;
                var view = View;
                if (view != null)
                {
                    view.OnInactive();
                }
            }
        }*/


    }

    public enum ViewResultStatus
    {
        None,
        Loading,
        Loaded,
        Unloading,
        Unloaded
    }

}