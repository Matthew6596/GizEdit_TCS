using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class VarStringProp : GizProperty
{
    public VarStringProp(string name, string defaultValue)
    {
        Set(name, defaultValue);
    }
    public void Set(string name, string value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        string str = (string)Value;
        byte[] b = new byte[str.Length+2];
        b[0] = (byte)str.Length;
        for (int i = 1; i < str.Length+1; i++) b[i] = (byte)str[i];
        b[str.Length - 1] = 0;
        return b;
    }
    public override void FromBin()
    {
        SetValue(GameManager.ReadString());
    }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (Input.text.Length > 254)
        {
            EditorManager.ThrowError("ERROR: " + Name + " property has a max length of 254 characters");

        }
        else
        {
            SetValue(Input.text);
        }
    }
    public override void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[5], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = (string)Value;
        Input.characterLimit = 254;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
