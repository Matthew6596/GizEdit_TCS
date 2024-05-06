using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StringProp : GizProperty
{
    public string Name { get; set; }
    public string Value { get; set; }
    public int Length { get; set; }
    public StringProp(string name,string defaultValue, int length)
    {
        Set(name, defaultValue);
        Length = length;
    }
    public void SetValue(string value)
    {
        Value = value;
    }
    public void Set(string name, string value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(bool[] value) { }
    public void SetValue(int value) { }
    public void SetValue(uint value) { }
    public void SetValue(float value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        return TypeConverter.StringToHex(Value,Length);
    }
    public void ReadFromHex()
    {
        SetValue(GameManager.gmInstance.FSliceString(GizmosReader.reader.ReadLocation,Length));
        GizmosReader.reader.ReadLocation += (uint)Length;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_InputField Input { get; set; }
    public void UpdateValue()
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
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[5], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = Value;
        Input.characterLimit = Length;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
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
