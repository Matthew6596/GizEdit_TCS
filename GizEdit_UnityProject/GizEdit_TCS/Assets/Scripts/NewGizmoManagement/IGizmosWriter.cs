using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGizmosWriter
{
    public byte[][] headerBytes { get; set; }
    public byte[] GetExtraHeaderStuff(int gizSection, int[] numOfEachGizmo);
}
