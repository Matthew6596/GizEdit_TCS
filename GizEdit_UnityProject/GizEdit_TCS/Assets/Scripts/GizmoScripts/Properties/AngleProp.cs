using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AngleProp : GizProperty
{
    public string Name { get; set; }
    public float Value { get; set; }
    public AngleProp(string name, float defaultValue)
    {
        Set(name, defaultValue);
    }
    public void SetValue(float value)
    {
        Value = value;
    }
    public void Set(string name, float value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(bool[] value) { }
    public void SetValue(int value) { }
    public void SetValue(uint value) { }
    public void SetValue(string value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        return TypeConverter.Int16ToHex(TypeConverter.FloatToInt16Angle(Value));
    }
    public void ReadFromHex()
    {
        SetValue(TypeConverter.Int16AngleToFloat(GameManager.gmInstance.FSliceInt16(GizmosReader.reader.ReadLocation)));
        GizmosReader.reader.ReadLocation += 2;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_InputField Input { get; set; }
    public void UpdateValue()
    {
        if (float.TryParse(Input.text, out float val)) {
            while (val < 0) val += 360;
            while (val >= 360) val -= 360;
            SetValue(val);
        }
        else
        {
            if(Input.text!=".")
                EditorManager.ThrowError("ERROR: " + Name + " property must be a floating point number");
        }
    }
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;

        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[3], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = Value.ToString();
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
