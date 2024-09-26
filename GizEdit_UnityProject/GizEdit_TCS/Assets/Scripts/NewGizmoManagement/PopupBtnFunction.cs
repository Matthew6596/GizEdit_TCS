using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupBtnFunction : MonoBehaviour
{
    public UnityEvent popupConfirmEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Confirm()
    {
        popupConfirmEvent.Invoke();
    }
}
