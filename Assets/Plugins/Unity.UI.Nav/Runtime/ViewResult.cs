using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Navs.Routing;
using UnityEngine;

namespace UnityEngine.UI.Navs
{

    /// <summary>
    /// UI 导航加载器
    /// </summary>
    public class ViewResult
    {
        private Action<ViewResult> loadedCallback;

        public string ViewName { get; set; }

        public Dictionary<string, object> ViewData { get; set; } = new Dictionary<string, object>();

        public object Model { get; set; }

        public View View { get; set; }

        /// <summary>
        /// 编辑器调试用
        /// </summary>
        public GameObject GameObject { get; protected set; }

        public Transform Transform { get => GameObject ? GameObject.transform : null; }

        public ViewResultStatus Status { get; private set; }


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

        /// <summary>
        /// 加载视图，<see cref="ViewResultStatus.Loading"/> 状态
        /// </summary>
        public virtual void Load(Context context)
        {
            if (Status == ViewResultStatus.None || Status == ViewResultStatus.Unloaded)
            {
                Status = ViewResultStatus.Loading;
            }
        }

        /// <summary>
        /// 通知视图加载完成，<see cref="ViewResultStatus.Loaded"/> 状态
        /// </summary>
        protected void OnLoaded()
        {
            if (Status == ViewResultStatus.Loading)
            {
                Status = ViewResultStatus.Loaded;
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
            if (Status != ViewResultStatus.Unloading || Status != ViewResultStatus.Unloaded)
            {
                Status = ViewResultStatus.Unloading;
            }
        }

        /// <summary>
        /// 通知视图卸载完成，<see cref="ViewResultStatus.Unloaded"/> 状态
        /// </summary>
        protected void OnUnloaded()
        {
            if (Status == ViewResultStatus.Unloading)
            {
                Status = ViewResultStatus.Unloaded;
                loadedCallback = null;
            }
        }



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