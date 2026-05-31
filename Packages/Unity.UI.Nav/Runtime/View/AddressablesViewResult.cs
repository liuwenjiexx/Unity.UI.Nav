#if UNITY_ADDRESSABLES
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



namespace LWJ.UI.Navs
{
    public class AddressablesViewResult : GameObjectViewResult
    {

        public static Dictionary<string, PrefabState> cachedPrefabHandles;
        static List<string> tmpList;
        static float Timeout = 10;
        static float NextCheckTime;
        static float CheckInterval = 5f;

        public class PrefabState
        {
            public AsyncOperationHandle<GameObject> Handle;
            public GameObject Instance;
            public int RefCount;
            public double AtTimeout;
        }

        public string AssetPath { get; private set; }

        private Transform parent;

        public AddressablesViewResult(string viewName, string assetPath, Transform parent, NavContext context)
            : base(viewName, context)
        {
            AssetPath = assetPath;
            this.parent = parent;
            if (cachedPrefabHandles == null)
            {
                cachedPrefabHandles = new();
                tmpList = new();
            }
        }

        public override async void Load(NavContext context)
        {
            base.Load(context);
            AsyncOperationHandle<GameObject> prefabHandle;
            if (!cachedPrefabHandles.TryGetValue(AssetPath, out var state))
            {
                NavUtility.Log("LoadAssetAsync: " + AssetPath);
                prefabHandle = Addressables.LoadAssetAsync<GameObject>(AssetPath);
                state = new PrefabState()
                {
                    Handle = prefabHandle,
                };
                cachedPrefabHandles[AssetPath] = state;
            }
            prefabHandle = state.Handle;
            state.AtTimeout = Time.time + Timeout;

            if (!prefabHandle.IsDone)
            {
                await prefabHandle.Task;
#if UNITY_EDITOR
                if (!Application.isPlaying) return;
#endif
            }

            
            if (Status == ViewResultStatus.Unloading || Status == ViewResultStatus.Unloaded)
            {
                return;
            }

            if (!state.Instance)
            {
                GameObject go;
                if (parent)
                {
                    go = GameObject.Instantiate(prefabHandle.Result, parent, false);
                }
                else
                {
                    go = GameObject.Instantiate(prefabHandle.Result);
                }
                state.Instance = go;
            }
            GameObject = state.Instance;
            OnLoaded();
        }

        public override void Unload()
        {
            base.Unload();

            if (cachedPrefabHandles.TryGetValue(AssetPath, out var state))
            {
                state.AtTimeout = Time.time + Timeout;
            }

            if (Time.time > NextCheckTime)
            {
                NextCheckTime = Time.time + CheckInterval;
                CheckTimeout();
            }
            OnUnloaded();
            /*
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
             
            */
        }

        public static void CheckTimeout()
        {
            tmpList.Clear();
            foreach (var it in cachedPrefabHandles)
            {
                var s = it.Value;
                if (!s.Instance && Time.time > s.AtTimeout)
                {
                    s.Handle.Release();
                    tmpList.Add(it.Key);
                    NavUtility.Log($"[Addressable] Timeout Release: {it.Key}");
                }
            }
            foreach (var k in tmpList)
            {
                cachedPrefabHandles.Remove(k);
            }
        }

    }
}

#endif