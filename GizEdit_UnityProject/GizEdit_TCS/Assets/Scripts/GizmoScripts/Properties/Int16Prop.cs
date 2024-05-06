using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Int16Prop : GizProperty
{
    public string Name { get; set; }
    public uint Value { get; set; }
    public Int16Prop(string name, uint defaultValue)
    {
        Set(name, defaultValue);
    }
    public void SetValue(uint value)
    {
        Value = value;
    }
    public void Set(string name, uint value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(bool[] value) { }
    public void SetValue(int value) { SetValue((uint)value); }
    public void SetValue(float value) { }
    public void SetValue(string value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        return TypeConverter.Int16ToHex(Value);
    }
    public void ReadFromHex()
    {
        SetValue(GameManager.gmInstance.FSliceInt16(GizmosReader.reader.ReadLocation));
        GizmosReader.reader.ReadLocation += 2;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_InputField Input { get; set; }
    public void UpdateValue()
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
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea = null)
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
    public string GetValueString()
    {
        return Value.ToString();
    }
}
