using System.Collections.Generic;
using UnityEngine;

namespace Kuxue.UI.Navs
{
    public class Reusable : MonoBehaviour
    {
        public static Dictionary<string, GameObject> cached = new();
        public string viewName;

        public string ViewName
        {
            get
            {
                if (!string.IsNullOrEmpty(viewName))
                    return viewName;
                string str = name;
                if (str.EndsWith("(Clone)"))
                {
                    str = str.Substring(0, str.Length - "(Clone)".Length);
                }
                return str;
            }
        }

  

        private void Awake()
        {
            Register(ViewName, gameObject);
        }

   
        private void OnDestroy()
        {
            Unregister(ViewName, gameObject);
        }

        public static void Register(string viewName, GameObject go)
        {
            cached[viewName] = go;
        }

        public static void Unregister(string viewName, GameObject go)
        {
            if (cached.TryGetValue(viewName, out var _go))
            {
                if (_go == go)
                {
                    cached.Remove(viewName);
                }
            }
        }

        public static bool TryGet(string viewName, out GameObject result)
        {
            if (cached.TryGetValue(viewName, out var reusable))
            {
                if (reusable)
                {
                    result = reusable.gameObject;
                    return true;
                }
                cached.Remove(viewName);
            }
            result = null;
            return false;
        }


    }
}