using System;
using System.Collections;
using System.Collections.Generic;
using Kuxue.UI.Routing;
using UnityEngine;


namespace Kuxue.UI.Navs
{

    public interface INavigation
    {
        NavContext Current { get; }
        string CurrentUrl { get; }
        int Count { get; }
        RouteCollection Routes { get; }

        event Action<NavBackingEventArgs> Backing;

        event Action<NavBeforeEventArgs> BeforeNavigation;
        event Action<NavAfterEventArgs> AfterNavigation;

        //IEnumerable<NavContext> Contexts { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="url"></param>
        /// <param name="parameter"></param>
        /// <returns>实例 ID</returns>
        int Navigate(NavOptions options);

        /// <summary>
        /// 卸载当前的UI <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        bool Back(int id);

        /// <summary>
        /// 卸载直到为 <paramref name="id"/> 的UI, 同时保留
        /// </summary>
        bool BackTo(int id);
        void BackToHome();


        NavContext FindById(int id);

        NavContext FindByUrl(string url);

         
        bool Remove(int id);

        void Clear();

        //bool IsEnter();



        //UINavFlags GetFlags(int id);

        //UINavFlags AddFlags(int id, UINavFlags flags);

        //UINavFlags RemoveFlags(int id, UINavFlags flags);

    }


}