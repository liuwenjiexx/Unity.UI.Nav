using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Unity.UI.Navs
{
    public class ResourcesViewResult : GameObjectViewResult
    {
        private Transform parent;
        public ResourcesViewResult(string viewName, string resourcePath, Transform parent, NavContext context)
            : base(viewName, context)
        {
            ResourcePath = resourcePath;
            this.parent = parent;
        }

        public string ResourcePath { get; private set; }

        public override void Load(NavContext context)
        {
            base.Load(context);
            if (Status == ViewResultStatus.Loading)
            {
                GameObject prefab = Resources.Load<GameObject>(ResourcePath);
                if (!prefab)
                    Debug.LogError("Resources.Load null, path: " + ResourcePath);
  
                GameObject go;
                if (parent)
                {
                    go = GameObject.Instantiate(prefab, parent, false);
                }
                else
                {
                    go = GameObject.Instantiate(prefab);
                }
                GameObject = go;

                OnLoaded();
            }
        }
        /*
        public override void Unload()
        {
            base.Unload();
            if (Status == ViewResultStatus.Unloading)
            {
                GameObject go = this.GameObject;
                try
                {
                    if (go)
                    {
                        var reusable = go.GetComponent<ReusableView>();
                        if (!reusable)
                            GameObject.Destroy(go);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                GameObject = null;
                OnUnloaded();
            }
        }
        */
    }
}