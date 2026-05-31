using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LWJ.UI.Navs
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
            //var nav = NavProvider.Get(gameObject);
            
            //if (mode == NavMode.Back)
            //{
            //    nav.Back(button);
            //}
            //else
            {
                Nav.Navigate(mode, url);
            }

        }

    }
}