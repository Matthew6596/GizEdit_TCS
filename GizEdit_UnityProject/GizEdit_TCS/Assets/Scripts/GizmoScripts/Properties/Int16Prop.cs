using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Int16Prop : GizProperty
{
    public Int16Prop(string name, short defaultValue)
    {
        Set(name, defaultValue);
    }
    public void Set(string name, short value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        return BitConverter.GetBytes((short)Value);
    }
    public override void FromBin()
    {
        SetValue(GameManager.ReadInt16());
    }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (int.TryParse(Input.text, out int val))
        {
            if (val > 65535 || val < 0)
            {
                EditorManager.ThrowError("ERROR: " + Name + " property must be between 0-65535 (inclusive)");
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
        Input.characterLimit = 5;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
