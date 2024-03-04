using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizForceChild : MonoBehaviour
{
    public Transform gizParent;
    [Header("Force Child Properties")]
    public string gizName = "";
    //unknown1
    public bool isSelected = false;
    //unknown2

    void Start()
    {
        StartCoroutine(FindParent());
        tag = "gizChild";
    }
    IEnumerator FindParent()
    {
        yield return null;
        transform.parent = gizParent;
    }

}
