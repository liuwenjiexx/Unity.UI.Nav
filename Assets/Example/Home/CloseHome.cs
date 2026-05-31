using System;
using Kuxue.UI.Navs;
using UnityEngine;
using UnityEngine.UI;

public class CloseHome : MonoBehaviour
{
 
    void Start()
    {
       var button= GetComponent<Button>();
        button.onClick.AddListener(() =>
        { 
            Nav.Clear();
        });
    }

  
}
