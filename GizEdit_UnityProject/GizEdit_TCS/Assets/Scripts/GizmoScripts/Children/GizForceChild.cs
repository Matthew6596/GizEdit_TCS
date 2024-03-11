using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizForceChild : MonoBehaviour
{
    public Transform gizParent;
    [Header("Force Child Properties")]
    public string gizName = "";
    public string unknown1= "00 00 80 3F 00 00 20 41 ";
    public bool isSelected = false;
    public string unknown2 = "00 00 00 00 00 ";

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
