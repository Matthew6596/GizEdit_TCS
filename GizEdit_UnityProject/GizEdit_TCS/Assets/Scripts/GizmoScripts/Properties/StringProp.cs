using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StringProp : GizProperty
{
    public int Length { get; set; }
    public StringProp(string name,string defaultValue, int length)
    {
        Set(name, defaultValue);
        Length = length;
    }
    public void Set(string name, string value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        byte[] b = new byte[Length];
        string str = (string)Value;
        for (int i = 0; i < Length; i++) if (i < str.Length) b[i] = (byte)str[i]; else b[i] = 0;
        return b;
    }
    public override void FromBin()
    {
        SetValue(GameManager.ReadFixedString(Length));
    }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (Input.text.Length > Length)
        {
            EditorManager.ThrowError("ERROR: " + Name + " property has a max length of "+Length+" characters");

        }
        else
        {
            SetValue(Input.text);
        }
    }
    public override void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gm.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gm.propPrefabs[5], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = (string)Value;
        Input.characterLimit = Length;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
