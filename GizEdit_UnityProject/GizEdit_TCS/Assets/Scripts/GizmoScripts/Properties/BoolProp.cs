using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoolProp : GizProperty
{
    public byte OnVal {  get; set; }
    public BoolProp(string name, bool defaultValue, byte onVal)
    {
        OnVal = onVal;
        Set(name, defaultValue);
    }
    public void Set(string name, bool value)
    {
        Name = name;
        Value = value;
    }
    public override byte[] ToBin()
    {
        return ((bool)Value)?(new byte[] { OnVal }):(new byte[] {0});
    }
    public override void FromBin()
    {
        byte tmp = GameManager.ReadInt8();
        SetValue(tmp != 0);
        if (tmp != 0) OnVal = tmp;
    }
    public Toggle Input { get; set; }
    public override void UpdateValue()
    {
        SetValue(Input.isOn);
    }
    public override void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[0], contentArea);
        Input = EditorInstance.transform.GetChild(1).GetComponent<Toggle>();
        Input.isOn = (bool)Value;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Set up event listeners
        Input.onValueChanged.AddListener((bool val) =>
        {
            UpdateValue();
            EditorManager.instance.CheckValues();
        });
    }
}
