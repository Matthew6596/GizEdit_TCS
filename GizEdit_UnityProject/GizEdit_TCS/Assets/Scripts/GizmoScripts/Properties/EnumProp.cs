using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnumProp : GizProperty
{
    public string Name { get; set; }
    public int Value { get; set; }
    public string[] OptionNames { get; set; }
    public string[] OptionHex { get; set; }
    public EnumProp(string name, int defaultValue, string[] optionNames, string[] optionHexValues)
    {
        OptionNames = optionNames;
        OptionHex = optionHexValues;
        Set(name, defaultValue);
    }
    public void SetValue(int value)
    {
        Value = value;
    }
    public void Set(string name, int value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(bool[] value) { }
    public void SetValue(uint value) { }
    public void SetValue(float value) { }
    public void SetValue(string value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        return OptionHex[Value];
    }
    public void ReadFromHex()
    {
        string val = GameManager.gmInstance.fhex.Substring(GizmosReader.B(GizmosReader.reader.ReadLocation), OptionHex[0].Length);
        for(int i=0; i<OptionHex.Length; i++)
        {
            if (val == OptionHex[i])
            {
                SetValue(i);
                break;
            }
        }
        GizmosReader.reader.ReadLocation += (uint)OptionHex[0].Length/3;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_Dropdown Input { get; set; }
    public void UpdateValue()
    {
        SetValue(Input.value);
    }
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea = null)
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
        Input.value = Value;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((int val) =>
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
