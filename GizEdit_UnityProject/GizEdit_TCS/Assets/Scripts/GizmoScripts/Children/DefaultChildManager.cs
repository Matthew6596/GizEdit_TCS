using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultChildManager : MonoBehaviour
{
    public static List<BaseGizmo> defaultChildrenGizmos;
    // Start is called before the first frame update
    void Start()
    {
        defaultChildrenGizmos = new();
        for (int i = 17; i < 21; i++) {
            GameObject obj = new();
            defaultChildrenGizmos.Add(GizmosReader.instance.CreateGizmo(i, obj));
        }
    }
}
