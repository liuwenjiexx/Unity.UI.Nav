using System;
using System.Reflection;
using LWJ.UI.Routing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LWJ.UI.Navs
{

    /// <summary>
    /// 导航控制器
    /// </summary>
    public abstract class Controller
    {
        public virtual void Initialize(NavContext context)
        {
            Context = context;
            context.Controller = this;

        }

        public NavContext Context { get; private set; }

        /// <summary>
        /// 视图共享数据，视图与控制器之间传递数据
        /// </summary>
        public Dictionary<string, object> ViewData { get; } = new();

        /// <summary>
        /// viewName 优先为 {action}，如果action为空则为 {controller}
        /// </summary>
        /// <returns></returns>
        public virtual ViewResult View()
        {
            string viewName = Context.ActionName;

            if (Context.RouteData.Values.ContainsKey("action"))
                viewName = Context.RouteData["action"] as string;

            if (string.IsNullOrEmpty(viewName))
            {
                if (Context.RouteData.Values.ContainsKey("controller"))
                    viewName = Context.RouteData["controller"] as string;
            }
            return View(viewName);
        }

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <param name="viewName">界面名称</param>
        /// <returns></returns>
        public abstract ViewResult View(string viewName);

        /// <summary>
        /// 视图名称和模型加载
        /// </summary> 
        public virtual ViewResult View(string viewName, object model)
        {
            var result = View(viewName);
            result.Model = model;
            return result;
        }
        /// <summary>
        /// 通过模型加载视图
        /// </summary> 
        public virtual ViewResult View(object model)
        {
            var result = View();
            result.Model = model;
            return result;
        }


        public virtual ViewResult GetActionResult(string actionName, NavContext context)
        {
            ViewResult view = null;
            if (!string.IsNullOrEmpty(actionName))
            {
                object target = this;
                Type type = target.GetType();

                var mInfo = type.GetMethod(actionName, BindingFlags.Public | BindingFlags.Instance);
                if (mInfo != null)
                {
                    //throw new MissingMethodException(type.Name, context.Action);
                    var ps = mInfo.GetParameters();
                    object[] args = new object[ps.Length];

                    for (int i = 0; i < ps.Length; i++)
                    {

                        var pInfo = ps[i];

                        object value = null;
                        if (context.RouteData.Values.TryGetValue(pInfo.Name, out value))
                        {
                            if (value == Route.Optional)
                            {
                                value = Type.Missing;
                            }
                        }
                        else
                        {
                            value = Type.Missing;
                            if (ps.Length == 1 && context.RouteData.Values.TryGetValue(Route.ParameterName, out var p) && p != null)
                            {
                                if (pInfo.ParameterType.IsAssignableFrom(p.GetType()))
                                {
                                    value = p;
                                }
                            }
                        }

                        if (value == Type.Missing && !pInfo.IsDefined(typeof(OptionalAttribute)))
                            throw new Exception($"not found parameter method <{type.Name}.{mInfo.Name}>");

                        if (value != null)
                        {
                            if (value != Type.Missing)
                            {
                                if (!pInfo.ParameterType.IsAssignableFrom(value.GetType()))
                                {
                                    value = Convert.ChangeType(value, pInfo.ParameterType);
                                }
                            }
                        }

                        args[i] = value;
                    }

                    view = (ViewResult)mInfo.Invoke(target, args);
                    if (view == null)
                        throw new Exception($"require return View, Action: {actionName}");
                }
                //else
                //{
                //    Debug.LogError($"Not found action. type: {type}, method: {actionName}");
                //}
            }
            //调用默认的 Action
            if (view == null)
                view = View();

            return view;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="from"></param>
        public virtual void OnEnter(NavMode mode, NavContext from)
        {
            var viewResult = Context.ViewResult;

            if (viewResult.Status == ViewResultStatus.None)
            {
                viewResult.Load(Context);
            }
            viewResult.OnNavEnter(from);

        }

        public virtual void OnExit(NavMode mode, NavContext to, bool unload)
        {
            var viewResult = Context.ViewResult;
            // if (!Nav.IsHome(Context.Id))
            {
                if (viewResult != null)
                {
                    viewResult.OnNavLeave(to);
                    if (unload)
                    {
                        if (!(viewResult.Status == ViewResultStatus.Unloading || viewResult.Status == ViewResultStatus.Unloaded))
                        {
                            viewResult.Unload();
                        }
                    }
                }

            }
        }


    }

}
