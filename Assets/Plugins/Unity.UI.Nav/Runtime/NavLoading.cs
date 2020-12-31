using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace UnityEngine.UI.Navs
{

    public abstract class NavLoading : MonoBehaviour
    {
        internal static List<NavLoading> all = new List<NavLoading>();
        internal static bool isLoading;
        public static LoadSceneHandlerDelegate LoadSceneHandler;
        public delegate void LoadSceneHandlerDelegate(string sceneName, LoadSceneMode mode, Action<float> progressCallback);

        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {
            all.Add(this);
        }

        protected virtual void OnDisable()
        {
            all.Remove(this);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            OnProgress(0f);
        }

        public virtual int GetRequreLoadItemsCount()
        {
            return 0;
        }

        public virtual int GetLoadedItemsCount()
        {
            return 0;
        }
        public virtual float GetLoadItemsWeight()
        {
            return 1f;
        }

        protected virtual void OnProgress(float progress)
        {

        }

        protected virtual void OnDestroy()
        {

        }

        /// <summary>
        /// 使用 <paramref name="loadingSceneName"/> 过度场景可以避免两个场景的内存叠加峰值
        /// </summary>
        public static void LoadScene(string sceneName, string loadingSceneName, Action callback)
        {
            NavBehaviour.Instance.StartCoroutine(_LoadScene(sceneName, loadingSceneName, callback));
        }

        static IEnumerator _LoadScene(string sceneName, string loadingSceneName, Action callback)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName));
            Debug.Assert(!isLoading);
            if (isLoading)
                yield break;
            isLoading = true;
            all.Clear();
            if (!string.IsNullOrEmpty(loadingSceneName))
            {
                if (LoadSceneHandler == null)
                {
                    yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
                }
                else
                {
                    float p = 0f;

                    LoadSceneHandler(loadingSceneName, LoadSceneMode.Single, (v) =>
                    {
                        p = v;
                    });
                    while (p < 1f)
                        yield return null;
                }
            }
            NavBehaviour.Instance.Clear();
            Debug.Assert(!string.IsNullOrEmpty(sceneName));
            float progress = 0f;
            bool isLoadingScene = false;
            AsyncOperation loadScene = null;
            bool loadSceneCB = false;

            float sceneProgress = 0f;
            bool sceneDone = false;

            //等待一帧才能获得所有NavLoading组件
            yield return null;

            while (progress < 1f)
            {
                if (!isLoadingScene)
                {
                    isLoadingScene = true;
                    sceneProgress = 0f;
                    if (LoadSceneHandler == null)
                    {
                        loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                        loadScene.allowSceneActivation = true;
                    }
                    else
                    {
                        loadSceneCB = true;
                        LoadSceneHandler(sceneName, LoadSceneMode.Additive, (p) =>
                         {
                             sceneProgress = p;
                         });
                    }
                }

                if (!sceneDone)
                {
                    if (sceneProgress < 1f)
                    {
                        if (loadScene != null)
                        {
                            if (!loadScene.isDone)
                            {
                                sceneProgress = loadScene.progress;
                            }
                            else
                            {
                                sceneProgress = 1f;
                            }
                        }
                        else if (loadSceneCB)
                        {
                        }
                        else
                        {
                            sceneProgress = 0f;
                        }
                    }
                    if (sceneProgress >= 1f)
                    {
                        sceneDone = true;
                        if (SceneManager.GetActiveScene().name != sceneName)
                        {
                            //编辑器下需要等待帧
                            yield return null;
                            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                        }
                    }
                }

                progress = CalculateProgress(sceneProgress);
                foreach (var item in all)
                {
                    if (item.isActiveAndEnabled)
                        item.OnProgress(progress);
                }
                yield return null;
            }


            if (!string.IsNullOrEmpty(loadingSceneName))
            {
                yield return SceneManager.UnloadSceneAsync(loadingSceneName);
            }

            isLoading = false;
            callback?.Invoke();
        }

        static float CalculateProgress(float sceneProgress)
        {

            int total = 0, current = 0;
            List<float> list = new List<float>();
            float totalWeight = 1f;
            foreach (var item in all)
            {
                int requre = item.GetRequreLoadItemsCount();
                if (requre <= 0)
                    continue;
                float weight = item.GetLoadItemsWeight();
                totalWeight += weight;
                list.Add(weight);
                int count = Mathf.Clamp(item.GetLoadedItemsCount(), 0, requre);
                list.Add(count);
                list.Add(requre);
                total += requre;
                current += count;
            }

            if (sceneProgress >= 1f && current >= total)
                return 1f;

            float progress = sceneProgress * (1f / totalWeight);
            for (int i = 0; i < list.Count; i += 3)
            {
                float weight = list[i] / totalWeight;
                progress += (list[i + 1] / list[i + 2]) * weight;
            }
             
            progress = Mathf.Clamp01(progress);
            return progress;
        }



    }
}