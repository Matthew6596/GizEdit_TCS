using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoolProp : GizProperty
{
    public string Name { get; set; }
    public bool Value { get; set; }
    public string OnVal {  get; set; }
    public BoolProp(string name, bool defaultValue, string onVal)
    {
        OnVal = onVal;
        Set(name, defaultValue);
    }
    public void SetValue(bool value)
    {
        Value = value;
    }
    public void Set(string name, bool value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool[] value) { }
    public void SetValue(int value) { }
    public void SetValue(uint value) { }
    public void SetValue(float value) { }
    public void SetValue(string value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        return (Value)?(OnVal):("00 ");
    }
    public void ReadFromHex()
    {
        SetValue(GameManager.gmInstance.fhex.Substring(GizmosReader.B(GizmosReader.reader.ReadLocation),3)!="00 ");
        GizmosReader.reader.ReadLocation += 1;
    }
    public GameObject EditorInstance { get; set; }
    public Toggle Input { get; set; }
    public void UpdateValue()
    {
        SetValue(Input.isOn);
    }
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[0], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<Toggle>();
        Input.isOn = Value;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((bool val) =>
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
