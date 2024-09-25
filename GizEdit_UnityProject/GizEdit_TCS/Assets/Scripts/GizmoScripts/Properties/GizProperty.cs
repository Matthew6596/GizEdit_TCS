using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum PropertyValueType
{
  String, VarString, Int8, Int16, Int32, Float32, Vec3, Angle, Bool, ChildList, Enum, Hex, BoolGroup
};*/

public abstract class GizProperty
{
    public object Value { get; set; }
    public string Name { get; set; }

    //Methods
    public void SetValue(object value) { Value = value; }
    public T GetValue<T>() { return (T)Convert.ChangeType(Value,typeof(T)); }
    public abstract void UpdateValue();
    public abstract byte[] ToBin();
    public abstract void FromBin();

    //Editor stuff
    public GameObject EditorInstance { get; set; }
    public void DeleteInEditor() { GameObject.Destroy(EditorInstance); }
    public abstract void CreateInEditor(Transform contentArea);

}
