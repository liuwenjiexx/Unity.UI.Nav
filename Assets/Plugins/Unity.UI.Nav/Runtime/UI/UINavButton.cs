using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI.Navs
{

    [RequireComponent(typeof(Button))]
    public class UINavButton : MonoBehaviour
    {
        public NavMode mode = NavMode.Replace;
        public string url;
        public int button;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ButtonClick);
        }

        void ButtonClick()
        {
            var nav = NavBehaviour.Get(gameObject);
            
            //if (mode == NavMode.Back)
            //{
            //    nav.Back(button);
            //}
            //else
            {
                nav.Navigation(mode, url);
            }

        }

    }
}