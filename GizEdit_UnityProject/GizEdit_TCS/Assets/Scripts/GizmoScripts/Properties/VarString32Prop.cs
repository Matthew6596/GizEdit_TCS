using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VarString32Prop : GizProperty
{
    private bool endSpace;
    public VarString32Prop(string name, string defaultValue, bool endSpacing=false)
    {
        endSpace = endSpacing;
        Set(name, defaultValue);
    }
    public void Set(string name, string value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        List<byte> ret = new();
        string str = (string)Value;
        ret.AddRange(BitConverter.GetBytes(str.Length));
        for (int i = 0; i < str.Length; i++) ret.Add((byte)str[i]);
        if (endSpace && ret[^1] != 0) ret.Add(0);
        return ret.ToArray();
    }
    public override void FromBin()
    {
        int len = GameManager.ReadInt32();
        SetValue(GameManager.ReadFixedString(len).Trim());
    }
    public TMP_InputField Input { get; set; }
    public override void UpdateValue()
    {
        if (Input.text.Length > int.MaxValue)
        {
            EditorManager.ThrowError("ERROR: " + Name + " property has a max length of... alot?");
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
        Input.characterLimit = int.MaxValue;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
