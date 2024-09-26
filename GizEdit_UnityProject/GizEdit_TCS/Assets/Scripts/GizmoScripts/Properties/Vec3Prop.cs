using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vec3Prop : GizProperty
{
    public Vec3Prop(string name, Vector3 defaultValue)
    {
        Set(name, defaultValue);
    }
    public void Set(string name, Vector3 value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        Vector3 val = (Vector3)Value;
        List<byte> ret = new();
        ret.AddRange(BitConverter.GetBytes(val.x));
        ret.AddRange(BitConverter.GetBytes(val.y));
        ret.AddRange(BitConverter.GetBytes(val.z));
        return ret.ToArray();
    }
    public override void FromBin()
    {
        SetValue(GameManager.ReadVec3());
    }
    public TMP_InputField[] Inputs { get; set; }
    public override void UpdateValue()
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
            if (Inputs[0].text != "." && Inputs[0].text != "")
                EditorManager.ThrowError("ERROR: " + Name + " property.x must be a floating point number");
        }
        if (float.TryParse(Inputs[1].text, out float v2))
        {
            val.y = v2;
            passed[1] = true;
        }
        else
        {
            if (Inputs[1].text != "." && Inputs[1].text != "")
                EditorManager.ThrowError("ERROR: " + Name + " property.y must be a floating point number");
        }
        if (float.TryParse(Inputs[2].text, out float v3))
        {
            val.z = v3;
            passed[2] = true;
        }
        else
        {
            if (Inputs[2].text != "." && Inputs[2].text != "")
                EditorManager.ThrowError("ERROR: " + Name + " property.z must be a floating point number");
        }
        if (passed[0] && passed[1] && passed[2]) SetValue(val);
    }
    public override void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[6], contentArea);
        Inputs = new TMP_InputField[3];
        Inputs[0] = EditorInstance.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>();
        Inputs[1] = EditorInstance.transform.GetChild(2).gameObject.GetComponent<TMP_InputField>();
        Inputs[2] = EditorInstance.transform.GetChild(3).gameObject.GetComponent<TMP_InputField>();
        Vector3 val = (Vector3)Value;
        Inputs[0].text = val.x.ToString();
        Inputs[1].text = val.y.ToString();
        Inputs[2].text = val.z.ToString();
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
}
