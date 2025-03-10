using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObj : MonoBehaviour
{
    public Action<GameObject> OnSelect = (obj) => { };
    public Action<GameObject> OnDeselect = (obj) => { };

    static SelectObj selected;


    public void MainSelect()
    {
        selected = this;
        OnSelect.Invoke(gameObject);
        Select();
    }
    public virtual void Select(){}

    public static void Deselect()
    {
        if (selected == null) return;
        selected.OnDeselect.Invoke(selected.gameObject);
        selected = null;
    }
}
