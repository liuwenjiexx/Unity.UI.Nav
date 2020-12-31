using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.UI.Navs
{

    public static partial class Extensions
    {

        public static int Push(this INav nav, string url, object parameter)
        {
            return nav.Navigation(NavMode.Push, url, parameter);
        }

        public static int Replace(this INav nav, string url, object parameter)
        {
            return nav.Navigation(NavMode.Replace, url, parameter);
        }

    }
}