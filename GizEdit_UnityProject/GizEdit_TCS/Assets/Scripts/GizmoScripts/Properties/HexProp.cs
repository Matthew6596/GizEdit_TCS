using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexProp : GizProperty
{
    public int Length { get; set; }
    public HexProp(string name, string defaultValue)
    {
        Length = defaultValue.Length;
        Set(name, defaultValue);
    }
    public void Set(string name, string value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        string val = (string)Value;
        string[] hexValues = val.Trim().Split(' ');
        byte[] ret = new byte[hexValues.Length];
        for (int i = 0; i < hexValues.Length; i++) ret[i] = Convert.ToByte(hexValues[i], 16);
        return ret;
    }
    public override void FromBin()
    {
        SetValue(BitConverter.ToString(GameManager.ReadSlice(Length)).Replace("-", " "));
    }
    public TMP_InputField Input { get; set; }
    bool endEditValidate;
    public override void UpdateValue()
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
    public override void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[5], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<TMP_InputField>();
        Input.text = (string)Value;
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
}
