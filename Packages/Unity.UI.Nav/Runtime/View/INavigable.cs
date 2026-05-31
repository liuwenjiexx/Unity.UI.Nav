using System.Collections.Generic;
using UnityEngine;

namespace Kuxue.UI.Navs
{
    public interface INavigable
    {
        int ViewId { get; }

        Dictionary<string, object> ViewData { get; set; }

        object Model { get; set; }

        NavContext Context { get; }

        void SetContext(NavContext context);

        void OnLoad();

        void OnUnload();

        void OnNavigationFrom(NavContext from);

        void OnNavigationTo(NavContext to);

    }
}