using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Navs;

public class TestView : View
{
    public int value;
    public Text label;
     
    private void OnEnable()
    {
        value = Random.Range(1, 100);
        UpdateUI();
    }
    void UpdateUI()
    {
        if (label)
        {
            label.text = value.ToString();
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateUI();
    }

}
