using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kuxue.UI.Navs
{
    [RequireComponent(typeof(Slider))]
    public class UINavLoadingSlider : NavLoading
    {
        Slider slider;

        // Start is called before the first frame update
        protected override void Awake()
        {
            slider = GetComponent<Slider>();
            base.Awake();
        }
        protected override void OnProgress(float progress)
        {
            slider.value = progress;
        }
    }
}