using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kuxue.UI.Navs
{
    public class NavBackingEventArgs : EventArgs
    {
        internal NavBackingEventArgs(INavigation nav, NavContext context, string url)
        {
            this.Navigation = context.Navigation;
            this.Context = context;
            this.Url = url;
        }
        public NavContext Context { get; private set; }
        public INavigation Navigation { get; private set; }

        public string Url { get; private set; }

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }

}