using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Navs;

public class DelayLoadingScene : NavLoading
{
    private int current = 0;
    private float next;
    public int count = 10;
    public override int GetRequreLoadItemsCount()
    {
        return count;
    }

    public override int GetLoadedItemsCount()
    {
        return current;
    }

    // Update is called once per frame
    void Update()
    {
        if (next > 0)
        {
            next -= Time.deltaTime;
        }
        else
        {
            next = 0.1f;
            current++;
        }
    }

}
