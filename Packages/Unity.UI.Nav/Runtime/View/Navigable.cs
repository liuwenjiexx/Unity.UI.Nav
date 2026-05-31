using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kuxue.UI.Navs
{

    public class Navigable : MonoBehaviour, INavigable
    {

        private GameObject lastSelected;
        public int ViewId => Context.Id;
        private int lastRefreshFrame;


        public Dictionary<string, object> ViewData { get; set; }

        public object Model { get; set; }

        public NavContext Context { get; private set; }
         

        public void SetContext(NavContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 准备数据
        /// </summary>
        public virtual void OnLoad()
        {
            NavUtility.Log($"[OnLoad] [{name}]");
         
            //Refresh();
        }


        public virtual void OnUnload()
        {
            NavUtility.Log($"[OnUnload] [{name}]");

        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public virtual void OnNavigationFrom(NavContext from)
        {
            NavUtility.Log($"[OnNavigationFrom] [{name}] \nFrom: {from?.Url}");
    
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            if (lastSelected)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
            if (lastRefreshFrame != Time.frameCount)
            {
                Refresh();
            }
        }

        public virtual void OnNavigationTo(NavContext to)
        {
            NavUtility.Log($"[OnNavigationTo] [{name}] \nTo: {to?.Url}");

            lastSelected = EventSystem.current.currentSelectedGameObject;
            bool isActive = false;
            if ((Context.Flags & NavFlags.Home) != 0)
            {
                isActive = true;
            }
            if (to != null && (to.Flags & NavFlags.Float) != 0)
            {
                isActive = true;
            }
            if (gameObject.activeSelf != isActive)
                gameObject.SetActive(isActive);

        }

        protected virtual void Refresh()
        {
            lastRefreshFrame = Time.frameCount;
        }

        protected virtual void Update()
        {
        

        }

     
    }
}