using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Navs
{
    public class NavBackingEventArgs : EventArgs
    {
        internal NavBackingEventArgs(INav nav, string url)
        {
            this.Navigation = nav;
            this.Url = url;
        }

        public INav Navigation { get; private set; }

        public string Url { get; private set; }

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }

}