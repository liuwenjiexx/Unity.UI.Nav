using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityEngine.UI.Navs
{

    [RequireComponent(typeof(Text))]
    public class UINavLoadingText : NavLoading
    {
        private Text text;
        public string format = "Loading {0:P0}";

        protected override void Awake()
        {
            text = GetComponent<Text>();
            base.Awake();
        }


        protected override void OnProgress(float progress)
        {
            text.text = string.Format(format, progress);
        }

    }
}