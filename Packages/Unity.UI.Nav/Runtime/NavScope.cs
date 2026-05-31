using System;
using UnityEngine;

namespace Kuxue.UI.Navs
{
    internal class NavScope : IDisposable
    {
        private INavigation nav;
        private string name;
        private bool disposed;

        public NavScope(string name, INavigation nav)
        {
            this.nav = nav;
            Nav.navScopes.Add(this);
            Nav.Current = nav;
            if (string.IsNullOrEmpty(name))
                name = nav.GetType().Name;
            this.name = name;
            NavUtility.Log($"Push Scope <{name}>");
        }

        public INavigation Provider => nav;

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                NavUtility.Log($"Pop Scope <{name}>");
                nav.Clear();
                Nav.navScopes.Remove(this);
                INavigation current = null;
                if (Nav.navScopes.Count > 0)
                {
                    current = Nav.navScopes[Nav.navScopes.Count - 1].nav;
                }
                if (Nav.Current != current)
                {
                    Nav.Current = current;
                }
            }
        }
        /*
        ~NavScope()
        {
            if (!disposed)
            {
                //try
                //{
                    Dispose();
                //}
                //catch { }
            }
        }*/
    }
}