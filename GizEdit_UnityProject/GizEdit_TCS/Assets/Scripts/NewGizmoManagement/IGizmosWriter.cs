using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGizmosWriter
{
    public string[] headerHex { get; set; }
    public string GetExtraHeaderStuff(int gizSection, int[] numOfEachGizmo);
}
