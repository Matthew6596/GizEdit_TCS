using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Float32Prop : GizProperty
{
    public Float32Prop(string name, float defaultValue)
    {
        Set(name, defaultValue);
    }
    public void Set(string name, float value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        return BitConverter.GetBytes((float)Value);
    }
    public override void FromBin()
    {
        SetValue(GameManager.ReadFloat());
    }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (float.TryParse(Input.text, out float val))
        {
            SetValue(val);
        }
        else
        {
            if (Input.text != ".")
                EditorManager.ThrowError("ERROR: " + Name + " property must be a floating point number");
        }
    }
    public override void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[3], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = Value.ToString();
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
