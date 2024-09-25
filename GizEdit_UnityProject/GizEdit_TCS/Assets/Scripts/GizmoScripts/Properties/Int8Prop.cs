using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Int8Prop : GizProperty
{
    public Int8Prop(string name, byte defaultValue)
    {
        Set(name, defaultValue);
    }
    public void Set(string name, byte value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        return new byte[] { (byte)Value };
    }
    public override void FromBin()
    {
        SetValue(GameManager.ReadInt8());
    }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (int.TryParse(Input.text, out int val))
        {
            if (val > 255 || val < 0)
            {
                EditorManager.ThrowError("ERROR: " + Name + " property must be between 0-255 (inclusive)");
            }
            else
            {
                SetValue(val);
            }
        }
        else
        {
            EditorManager.ThrowError("ERROR: " + Name + " property must be an integer number");
        }
    }
    public override void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[4], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = Value.ToString();
        Input.characterLimit = 3;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
