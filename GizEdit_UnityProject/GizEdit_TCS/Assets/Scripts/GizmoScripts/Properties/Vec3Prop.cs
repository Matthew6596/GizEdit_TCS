using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vec3Prop : GizProperty
{
    public string Name { get; set; }
    public Vector3 Value { get; set; }
    public Vec3Prop(string name, Vector3 defaultValue)
    {
        Set(name, defaultValue);
    }
    public void SetValue(Vector3 value)
    {
        Value = value;
    }
    public void Set(string name, Vector3 value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(bool[] value) { }
    public void SetValue(int value) { }
    public void SetValue(uint value) { }
    public void SetValue(float value) { }
    public void SetValue(string value) { }
    //
    public string ConvertToHex()
    {
        return TypeConverter.Float32ToHex(Value.x)+ TypeConverter.Float32ToHex(Value.y)+ TypeConverter.Float32ToHex(Value.z);
    }
    public void ReadFromHex()
    {
        GameManager gm = GameManager.gmInstance;
        SetValue(new Vector3(gm.FSliceFloat32(GizmosReader.ReadLocation), gm.FSliceFloat32(GizmosReader.ReadLocation+4), gm.FSliceFloat32(GizmosReader.ReadLocation+8)));
        GizmosReader.ReadLocation += 12;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_InputField[] Inputs { get; set; }
    public void UpdateValue()
    {
        Vector3 val=new();
        bool[] passed=new bool[] {false,false,false};
        if (float.TryParse(Inputs[0].text, out float v1))
        {
            val.x = v1;
            passed[0] = true;
        }
        else
        {
            if (Inputs[0].text != ".")
                EditorManager.ThrowError("ERROR: " + Name + " property.x must be a floating point number");
        }
        if (float.TryParse(Inputs[1].text, out float v2))
        {
            val.y = v2;
            passed[1] = true;
        }
        else
        {
            if (Inputs[1].text != ".")
                EditorManager.ThrowError("ERROR: " + Name + " property.y must be a floating point number");
        }
        if (float.TryParse(Inputs[2].text, out float v3))
        {
            val.z = v3;
            passed[2] = true;
        }
        else
        {
            if (Inputs[2].text != ".")
                EditorManager.ThrowError("ERROR: " + Name + " property.z must be a floating point number");
        }
        if (passed[0] && passed[1] && passed[2]) SetValue(val);
    }
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[6], contentArea);
        Inputs = new TMP_InputField[3];
        Inputs[0] = EditorInstance.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>();
        Inputs[1] = EditorInstance.transform.GetChild(2).gameObject.GetComponent<TMP_InputField>();
        Inputs[2] = EditorInstance.transform.GetChild(3).gameObject.GetComponent<TMP_InputField>();
        Inputs[0].text = Value.x.ToString();
        Inputs[1].text = Value.y.ToString();
        Inputs[2].text = Value.z.ToString();
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Inputs[0].onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
        Inputs[1].onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
        Inputs[2].onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
    public string GetValueString()
    {
        return Value.ToString();
    }
}
