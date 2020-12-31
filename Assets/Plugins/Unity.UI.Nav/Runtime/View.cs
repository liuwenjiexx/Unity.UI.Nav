using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Navs
{

    public class View : MonoBehaviour
    {
        [SerializeField]
        private bool hideOnExit;
        private bool isShow;

        private int id;

        public int Id { get => id; set => id = value; }

        public Dictionary<string, object> ViewData { get; set; }

        public object Model { get; set; }

        public bool IsShow
        {
            get => isShow;
        }

        public bool HideOnExit
        {
            get => hideOnExit;
            set => hideOnExit = value;
        }

        public virtual void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            if (!isShow)
            {
                isShow = true;
                if (Nav.LogEnabled)
                    Nav.Log($"OnShow <{name}>");
                OnShow();
            }

        }

        public virtual void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            if (isShow)
            {
                isShow = false;
                if (Nav.LogEnabled)
                    Nav.Log($"OnHide: <{name}>");
                OnHide();
            }
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

    }
}