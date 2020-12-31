using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Navs;

namespace UnityEngine.UI.Navs
{
    public class ResourcesViewResult : ViewResult
    { 
        public string Path { get; set; }

        public override void Load(Context context)
        {
            base.Load(context);
            if (Status == ViewResultStatus.Loading)
            {
                GameObject prefab = Resources.Load<GameObject>(Path);
                var go = GameObject.Instantiate(prefab);
                View = go.GetComponent<View>();
                if (!View)
                    View = go.AddComponent<View>();
                View.ViewData = ViewData;
                View.Model = Model;
                GameObject = go;

                OnLoaded();
            }
        }

        public override void Unload()
        {
            base.Unload();
            if (Status == ViewResultStatus.Unloading)
            {
                GameObject go = this.GameObject;
                if (go)
                    GameObject.Destroy(go);
                GameObject = null;
                OnUnloaded();
            }
        }

    }
}