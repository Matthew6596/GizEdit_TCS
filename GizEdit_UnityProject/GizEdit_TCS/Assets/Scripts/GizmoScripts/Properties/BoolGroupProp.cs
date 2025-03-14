using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoolGroupProp : GizProperty
{
    public string[] Names { get; set; }
    public bool[] Values { get; set; }
    public bool[] ValueActive { get; set; }
    public BoolGroupProp(string[] names, bool[] defaultValues, bool[] valueActive)
    {
        Set(names, defaultValues);
        ValueActive = valueActive;
    }
    public void Set(string[] names, bool[] value)
    {
        Names = names;
        Values = value;
        SetValue(Values);
    }
    public override byte[] ToBin()
    {
        Values = GetValue<bool[]>();
        byte sum = 0;
        if (Values[0]) sum += 1;
        if (Values[1]) sum += 2;
        if (Values[2]) sum += 4;
        if (Values[3]) sum += 8;
        if (Values[4]) sum += 16;
        if (Values[5]) sum += 32;
        if (Values[6]) sum += 64;
        if (Values[7]) sum += 128;
        return new byte[] {sum};
    }
    public override void FromBin()
    {
        Values = new bool[8];
        byte num = GameManager.ReadInt8();
        if (num % 2 == 1) { Values[0] = true; num -= 1; }
        if (num % 4 == 2) { Values[1] = true; num -= 2; }
        if (num % 8 == 4) { Values[2] = true; num -= 4; }
        if (num % 16 == 8) { Values[3] = true; num -= 8; }
        if (num % 32 == 16) { Values[4] = true; num -= 16; }
        if (num % 64 == 32) { Values[5] = true; num -= 32; }
        if (num % 128 == 64) { Values[6] = true; num -= 64; }
        if (num == 128) { Values[7] = true;}
        SetValue(Values);
    }
    //public GameObject[] EditorInstances { get; set; }
    public Toggle[] Inputs { get; set; }
    public override void UpdateValue()
    {
        for (int i = 0; i < 8; i++)
            if (ValueActive[i])
                Values[i] = Inputs[i].isOn;
        SetValue(Values);
    }
    public override void CreateInEditor(Transform contentArea = null) //<<<<EDITORINSTANCE NEEDS UPDATING, MAKE PARENT FOR GROUP
    {
        if (contentArea == null) contentArea = GameManager.gm.propertyPanelContent;

        GameObject[] insts = new GameObject[8];
        Toggle[] inps = new Toggle[8];
        for(int i=0; i<8; i++)
        {
            if (ValueActive[i])
            {
                insts[i] = GameObject.Instantiate(GameManager.gm.propPrefabs[0], contentArea);
                inps[i] = insts[i].transform.GetChild(1).GetComponent<Toggle>();
                inps[i].isOn = Values[i];
                insts[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Names[i];
            }
        }
        //EditorInstances = insts;
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
}
