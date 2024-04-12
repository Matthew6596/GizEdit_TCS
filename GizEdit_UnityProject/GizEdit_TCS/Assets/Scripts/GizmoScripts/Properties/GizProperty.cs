using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum PropertyValueType
{
  String, VarString, Int8, Int16, Int32, Float32, Vec3, Angle, Bool, ChildList, Enum, Hex, BoolGroup
};*/

public interface GizProperty
{
    public string Name { get; set; }
    //Overloaded sets
    public void SetValue(bool value);
    public void SetValue(bool[] value);
    public void SetValue(int value);
    public void SetValue(uint value);
    public void SetValue(float value);
    public void SetValue(string value);
    public void SetValue(Vector3 value);
    public string GetValueString();
    public void UpdateValue();
    public string ConvertToHex();
    public void ReadFromHex();
    public void DeleteInEditor();
    public void CreateInEditor(Transform contentArea);

}
