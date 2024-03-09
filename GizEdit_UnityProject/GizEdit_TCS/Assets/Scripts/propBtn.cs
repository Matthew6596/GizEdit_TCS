using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class propBtn : MonoBehaviour
{
    public Button btnSelf;
    public Action function;

    void btnFunction()
    {
        function.Invoke();
    }

    public void AddListener()
    {
        btnSelf.onClick.AddListener(btnFunction);
    }
}
