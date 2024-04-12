using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class VarStringProp : GizProperty
{
    public string Name { get; set; }
    public string Value { get; set; }
    public VarStringProp(string name, string defaultValue)
    {
        Set(name, defaultValue);
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
        return TypeConverter.VarStringToHex(Value);
    }
    public void ReadFromHex()
    {
        uint length = GameManager.gmInstance.FSliceInt8(GizmosReader.ReadLocation);
        SetValue(GameManager.gmInstance.FSliceString(GizmosReader.ReadLocation+1,length));
        GizmosReader.ReadLocation += length+1;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_InputField Input { get; set; }
    public void UpdateValue()
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
        Input.characterLimit = 254;
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
