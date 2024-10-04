using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGizmosReader
{
    public bool[] sectionReady { get; set; }
    public byte[][] headerData { get; set; }
    public int[] headerLengths { get; set; }
    public int ReadLocation { get; set; }
    public byte[] getHeader(string titleName);
    public IEnumerator ReadGizmos();
    public BaseGizmo CreateGizmo(int section, GameObject obj, int subsection);
}
