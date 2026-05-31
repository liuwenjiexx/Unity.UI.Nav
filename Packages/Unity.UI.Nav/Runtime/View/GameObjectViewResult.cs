using UnityEngine;
namespace LWJ.UI.Navs
{
    public abstract class GameObjectViewResult : ViewResult
    {
        private GameObject gameObject;

        public GameObjectViewResult(string viewName, NavContext context)
            : base(viewName, context)
        {
        }

        public GameObject GameObject
        {
            get => gameObject;
            protected set
            {
                if (gameObject != value)
                {
                    gameObject = value;
                    if (gameObject)
                    {
                        View = gameObject.GetComponent<INavigable>();
                        if (View == null)
                            View = gameObject.AddComponent<Navigable>();
                    }
                    else
                    {
                        View = null;
                    }
                }
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (IsNavEnter)
            {
                if (gameObject && !gameObject.activeSelf)
                    gameObject.SetActive(true);
            }
        
        }

        public override void Unload()
        {
            if (GameObject)
            {
                var reusable = GameObject.GetComponent<Reusable>();
                if (!reusable)
                {
                    GameObject.Destroy(GameObject);
                    GameObject = null;
                }
                else
                {
                    GameObject.SetActive(false);
                }
            }

            base.Unload();
        }




        public static GameObjectViewResult FromGameObject(string viewName, GameObject go, NavContext context)
        {
            return new ImmediatedGameObjectViewResult(viewName, go, context);
        }

        class ImmediatedGameObjectViewResult : GameObjectViewResult
        {
            public ImmediatedGameObjectViewResult(string viewName, GameObject go, NavContext context)
                : base(viewName, context)
            {
                GameObject = go;
            }

            public override void Load(NavContext context)
            {
                base.Load(context);
                OnLoaded();
            }

            public override void Unload()
            {
                base.Unload();
                OnUnloaded();
            }

        }
    }


}
