using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnumProp : GizProperty
{
    public string[] OptionNames { get; set; }
    public byte[] OptionVals { get; set; }
    public EnumProp(string name, int defaultValue, string[] optionNames, byte[] optionValues)
    {
        OptionNames = optionNames;
        OptionVals = optionValues;
        Set(name, defaultValue);
    }
    public void Set(string name, int value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        return new byte[] { OptionVals[(int)Value] };
    }
    public override void FromBin()
    {
        byte val = GameManager.ReadInt8();
        for(int i=0; i< OptionVals.Length; i++)
        {
            if (val == OptionVals[i])
            {
                SetValue(i);
                break;
            }
        }
    }
    public TMP_Dropdown Input { get; set; }
    public override void UpdateValue()
    {
        SetValue(Input.value);
    }
    public override void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[2], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_Dropdown>();
        List<TMP_Dropdown.OptionData> options=new();
        Input.ClearOptions();
        for (int i = 0; i < OptionNames.Length; i++)
        {
            options.Add(new TMP_Dropdown.OptionData(OptionNames[i]));
        }
        Input.AddOptions(options);
        Input.value = (int)Value;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((int val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
