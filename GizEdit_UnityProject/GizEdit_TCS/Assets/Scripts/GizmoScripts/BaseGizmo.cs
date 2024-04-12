using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseGizmo : MonoBehaviour
{
    abstract public GizProperty[] GizProperties { get; set; }
    abstract public string parentName { get; }
    public MeshRenderer mrender;
    public MeshFilter mfilter;
    public MeshCollider mcollider;
    abstract public void CheckValues();
    public string ConvertToHex()
    {
        string ret="";
        foreach(GizProperty prop in GizProperties)
        {
            ret+=prop.ConvertToHex();
        }
        return ret;
    }
    public void ReadFromHex()
    {
        foreach (GizProperty prop in GizProperties)
        {
            prop.ReadFromHex();
        }
    }
    public void CreateInEditor()
    {
        foreach (GizProperty prop in GizProperties)
        {
            prop.CreateInEditor(GameManager.gmInstance.propertyPanelContent);
        }
    }
    public void DestroyInEditor()
    {
        foreach (GizProperty prop in GizProperties)
        {
            prop.DeleteInEditor();
        }
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
}
