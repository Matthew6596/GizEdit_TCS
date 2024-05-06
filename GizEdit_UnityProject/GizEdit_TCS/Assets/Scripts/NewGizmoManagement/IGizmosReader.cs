using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGizmosReader
{
    public bool[] sectionReady { get; set; }
    public string[] headerData { get; set; }
    public int[] headerLengths { get; set; }
    public uint ReadLocation { get; set; }
    public string getHeader(string titleName);
    public IEnumerator ReadGizmos();
    public BaseGizmo CreateGizmo(int section, GameObject obj);
}
