using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoolGroupProp : GizProperty
{
    public string Name {  get; set; }
    public string[] Names { get; set; }
    public bool[] Values { get; set; }
    public bool[] ValueActive { get; set; }
    public BoolGroupProp(string[] names, bool[] defaultValues, bool[] valueActive)
    {
        Set(names, defaultValues);
        ValueActive = valueActive;
    }
    public void SetValue(bool[] value)
    {
        Values = value;
    }
    public void Set(string[] names, bool[] value)
    {
        Names = names;
        Values = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(int value) { }
    public void SetValue(uint value) { }
    public void SetValue(float value) { }
    public void SetValue(string value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        uint sum = 0;
        if (Values[0]) sum += 1;
        if (Values[1]) sum += 2;
        if (Values[2]) sum += 4;
        if (Values[3]) sum += 8;
        if (Values[4]) sum += 16;
        if (Values[5]) sum += 32;
        if (Values[6]) sum += 64;
        if (Values[7]) sum += 128;
        return TypeConverter.Int8ToHex(sum)+" ";
    }
    public void ReadFromHex()
    {
        uint num = GameManager.gmInstance.FSliceInt8(GizmosReader.reader.ReadLocation);
        if (num % 2 == 1) { Values[0] = true; num -= 1; }
        if (num % 4 == 2) { Values[1] = true; num -= 2; }
        if (num % 8 == 4) { Values[2] = true; num -= 4; }
        if (num % 16 == 8) { Values[3] = true; num -= 8; }
        if (num % 32 == 16) { Values[4] = true; num -= 16; }
        if (num % 64 == 32) { Values[5] = true; num -= 32; }
        if (num % 128 == 64) { Values[6] = true; num -= 64; }
        if (num == 128) { Values[7] = true;}
        GizmosReader.reader.ReadLocation += 1;
    }
    public GameObject[] EditorInstances { get; set; }

    public Toggle[] Inputs { get; set; }
    public void UpdateValue()
    {
        for (int i = 0; i < 8; i++)
            if (ValueActive[i])
                Values[i] = Inputs[i].isOn;
    }
    public void DeleteInEditor()
    {
        for (int i = 0; i < 8; i++)
            if (ValueActive[i])
                GameObject.Destroy(EditorInstances[i]);
    }
    public void CreateInEditor(Transform contentArea = null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;

        GameObject[] insts = new GameObject[8];
        Toggle[] inps = new Toggle[8];
        for(int i=0; i<8; i++)
        {
            if (ValueActive[i])
            {
                insts[i] = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[0], contentArea);
                inps[i] = insts[i].transform.GetChild(1).GetComponent<Toggle>();
                inps[i].isOn = Values[i];
                insts[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Names[i];
            }
        }
        EditorInstances = insts;
        Inputs = inps;

        //Set up event listeners
        for (int i = 0; i < 8; i++)
        {
            if (ValueActive[i])
            {
                Inputs[i].onValueChanged.AddListener((bool val) =>
                {
                    UpdateValue();
                    EditorManager.instance.CheckValues();
                });
            }
        }
    }
    public string GetValueString()
    {
        string ret = "";
        for (int i = 0; i < 8; i++)
        {
            if (ValueActive[i])
            {
                ret += Values[i].ToString()+",";
            }
        }
        return ret;
    }
}
