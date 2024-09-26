using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseGizmo : MonoBehaviour
{
    public GizProperty[] GizProperties { get; set; }
    abstract public string parentName { get; }
    public MeshRenderer mrender;
    public MeshFilter mfilter;
    public MeshCollider mcollider;
    abstract public void CheckValues();
    public void SetProp(int num, object value) { GizProperties[num].SetValue(value); }
    public byte[] ToBin()
    {
        List<byte> bytes = new();
        foreach(GizProperty prop in GizProperties) bytes.AddRange(prop.ToBin());
        return bytes.ToArray();
    }
    public void FromBin()
    {
        foreach (GizProperty prop in GizProperties) prop.FromBin();
    }
    public void CreateInEditor()
    {
        foreach (GizProperty prop in GizProperties) prop.CreateInEditor(GameManager.gmInstance.propertyPanelContent);
    }
    public void DestroyInEditor()
    {
        foreach (GizProperty prop in GizProperties) prop.DeleteInEditor();
    }
    public void Start()
    {
        StartCoroutine(FindParent());
        tag = "objectGiz";
        mrender = gameObject.AddComponent<MeshRenderer>();
        mfilter = gameObject.AddComponent<MeshFilter>();
        mcollider = gameObject.AddComponent<MeshCollider>();
    }
    IEnumerator FindParent()
    {
        yield return null;
        Transform p = GameObject.Find(parentName).transform;
        transform.parent = p;
        CheckValues();
    }
    abstract public string GetGizType();
}
