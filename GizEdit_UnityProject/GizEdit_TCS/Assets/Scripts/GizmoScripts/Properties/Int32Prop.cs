using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Int32Prop : GizProperty
{
    public Int32Prop(string name, int defaultValue)
    {
        Set(name, defaultValue);
    }
    public void Set(string name, int value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin() { return BitConverter.GetBytes((int)Value); }
    public override void FromBin() { SetValue(GameManager.ReadInt32()); }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (int.TryParse(Input.text, out int val))
        {
            SetValue(val);
        }
        else
        {
            if(Input.text!="")
                EditorManager.ThrowError("ERROR: " + Name + " property must be an integer number");
        }
    }
    public override void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gm.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gm.propPrefabs[4], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = Value.ToString();
        Input.characterLimit = 11;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
