using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LWJ.UI.Navs;

public class TestView : Navigable
{
    public Text label;
    const string ValueKey = "Value";


    protected override void Refresh()
    {
        base.Refresh();
        if (label)
        {
            label.text = ViewData[ValueKey].ToString();
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (!ViewData.ContainsKey(ValueKey))
        {
            ViewData[ValueKey] = Random.Range(1, 100);
        }
    }

    public override void OnNavigationFrom(NavContext from)
    {
        base.OnNavigationFrom(from);
        Refresh();
    }

}
