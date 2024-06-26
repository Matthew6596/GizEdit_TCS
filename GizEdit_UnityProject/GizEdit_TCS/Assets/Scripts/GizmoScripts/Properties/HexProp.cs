using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexProp : GizProperty
{
    public string Name { get; set; }
    public string Value { get; set; }
    public int Length { get; set; }
    public HexProp(string name, string defaultValue)
    {
        Length = defaultValue.Length;
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
        return Value[..Length];
    }
    public void ReadFromHex()
    {
        SetValue(GameManager.gmInstance.fhex.Substring((int)GizmosReader.reader.ReadLocation * 3, Length));
        GizmosReader.reader.ReadLocation += (uint)Length/3;
    }
    public GameObject EditorInstance { get; set; }
    public TMP_InputField Input { get; set; }
    bool endEditValidate;
    public void UpdateValue()
    {
        Input.text = Input.text.ToUpper();
        if (endEditValidate)
        {
            if (Input.text.Length == Length - 1)
            {
                Input.text += " ";
            }
            if (Input.text.Length != Length)
            {
                EditorManager.ThrowError("ERROR: " + Name + " property must have exactly " + Length + " characters");
            }
            else if (stringNotHex(Input.text))
            {
                EditorManager.ThrowError("ERROR: " + Name + " property must be valid hexidecimal");
            }
            else
            {
                SetValue(Input.text);
            }
        }
        endEditValidate = false;
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
        //validation
        Input.contentType = TMP_InputField.ContentType.Custom;
        Input.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        HexValidator v = (HexValidator)ScriptableObject.CreateInstance(typeof(HexValidator));
        Input.inputValidator = v;
        //name
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((string val) =>
        {
            UpdateValue();
        });
        Input.onEndEdit.AddListener((string val) =>
        {
            endEditValidate = true;
            UpdateValue();
        });
    }
    bool stringNotHex(string txt)
    {
        string goodVals = "0123456789ABCDEF ";
        for(int i=0; i<txt.Length; i++)
        {
            if (!goodVals.Contains(txt[i])) return false;
        }
        return true;
    }
    public string GetValueString()
    {
        return Value.ToString();
    }
}
