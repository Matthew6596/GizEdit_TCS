using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObj : MonoBehaviour
{
    public static MoveObj selected;

    public Action<GameObject> OnDeselect = (obj) => { };
    public Action<GameObject> OnSelect = (obj) => { };

    private Transform prevParent;

    public bool applyGizOffset = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selected == this)
        {
            //Vector3 offset = (applyGizOffset) ? AdvMouseInput.GetSelectOffset() : Vector3.zero;
            transform.position = AdvMouseInput.GetMoveGizPos();
        }
    }

    public void Select()
    {
        selected = this;
        if (transform.parent==null) prevParent = null;
        else prevParent = transform.parent;

        OnSelect.Invoke(gameObject);

        GameObject moveGiz = AdvMouseInput.instance.moveGiz.gameObject;
        moveGiz.SetActive(true);
        moveGiz.transform.position = transform.position;
        transform.parent = moveGiz.transform;

    }
    public static void Deselect()
    {
        if (selected == null) return;
        selected.transform.SetParent(selected.prevParent);
        selected.OnDeselect.Invoke(selected.gameObject);
        selected = null;
        AdvMouseInput.instance.moveGiz.gameObject.SetActive(false);
    }
}
