using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Navs.Routing;
using UnityEngine;


namespace UnityEngine.UI.Navs
{

    public interface INav
    {
        string Current { get; }
        int Count { get; }
        RouteCollection Routes { get; }

        event Action<NavBackingEventArgs> Backing;

        event Action<UINavBeforeEventArgs> BeforeNavigation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <returns>实例 ID</returns>
        int Navigation(NavMode mode, string url, object parameter = null);


        /// <summary>
        /// 卸载当前的UI 同时名称等于 <paramref name="name"/>
        /// </summary>
        /// <param name="name"></param>
        bool Back(string name);
        /// <summary>
        /// 卸载当前的UI <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        bool Back(int id);

        /// <summary>
        /// 卸载直到为 <paramref name="name"/> 的UI, 同时保留
        /// </summary>
        /// <param name="name"></param>
        bool BackTo(string name);
        /// <summary>
        /// 卸载直到为 <paramref name="id"/> 的UI, 同时保留
        /// </summary>
        /// <param name="name"></param>
        bool BackTo(int id);
        void BackToHome();

        bool Contains(string name);
        bool Contains(int id);

        bool Remove(string name);
        bool Remove(int id);

        void Clear();

        bool IsEnter();

        UINavFlags GetFlags(int id);
        //UINavFlags SetFlags(int id, UINavFlags flags);

        void SetHome(int id, bool isHome);

    }
 

}