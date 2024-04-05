using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizBuilditChild : MonoBehaviour
{
    public Transform gizParent;
    [Header("Buildit Child Properties")]
    public string gizName = "";
    public string unknown1 = "00 00 80 3F ";
    public float animateLength = 2f;
    public bool isSelected = false;
    public string unknown2 = "00 00 00 ";

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
