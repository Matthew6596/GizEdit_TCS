using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ChildListProp : GizProperty
{
    public string Name { get; set; }
    public int Value { get; set; } //Selected child
    public List<BaseGizmo> Children { get; set; }
    bool inc03;
    BaseGizmo DefaultChild { get; set; }
    public ChildListProp(string name, int defaultValue, BaseGizmo defaultChild, bool include03=true)
    {
        DefaultChild = defaultChild;
        Set(name, defaultValue);
        inc03 = include03;
    }
    public void SetValue(int value)
    {
        Value = value;
    }
    public void Set(string name, int value)
    {
        Name = name;
        Value = value;
    }
    //Unused SetValues
    public void SetValue(bool value) { }
    public void SetValue(bool[] value) { }
    public void SetValue(uint value) { }
    public void SetValue(float value) { }
    public void SetValue(string value) { }
    public void SetValue(Vector3 value) { }
    //
    public string ConvertToHex()
    {
        string ret=TypeConverter.Int8ToHex((uint)Children.Count)+" ";
        if (inc03) ret = "03 " + ret;
        foreach(BaseGizmo _c in Children)
        {
            foreach(GizProperty _p in _c.GizProperties)
            {
                ret += _p.ConvertToHex();
            }
        }
        return ret;
    }
    public void ReadFromHex()
    {
        if (inc03) GizmosReader.ReadLocation++;
        Children = new();
        uint _l = GameManager.gmInstance.FSliceInt8(GizmosReader.ReadLocation);
        GizmosReader.ReadLocation++;
        for (int i=0; i<_l; i++)
        {
            GameObject c = new();
            BaseGizmo g = c.AddComponent<BaseGizmo>();
            g = DefaultChild;
            g.ReadFromHex();
            Children.Add(g);
        }
    }
    public GameObject EditorInstance { get; set; }
    public TMP_Dropdown Input { get; set; }
    public Button AddBtn { get; set; }
    public Button RemoveBtn { get; set; }
    public Transform ChildrenParent { get; set; }
    public void UpdateValue()
    {
        DestroyChildrenProperties();
        SetValue(Input.value);
        CreateChildrenProperties();
    }
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;

        //Create prop prefab
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[1], contentArea);
        //Get Inputs
        ChildrenParent = EditorInstance.transform.GetChild(0).GetChild(0).GetChild(0);
        Input = ChildrenParent.GetChild(1).GetComponent<TMP_Dropdown>();
        AddBtn = ChildrenParent.GetChild(2).GetChild(0).GetComponent<Button>();
        RemoveBtn = ChildrenParent.GetChild(2).GetChild(1).GetComponent<Button>();
        //Create dropdown options
        List<TMP_Dropdown.OptionData> options = new();
        foreach (BaseGizmo child in Children)
        {
            options.Add(new TMP_Dropdown.OptionData(child.GizProperties[0].GetValueString()));
        }
        Input.AddOptions(options);
        //Set name and selected option
        Input.value = Value;
        EditorInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Create children properties
        CreateChildrenProperties();
        //Set up event listeners
        AddBtn.onClick.AddListener(() => {
            //Create new child
            GameObject c = new();
            BaseGizmo g = c.AddComponent<BaseGizmo>();
            g = DefaultChild;
            Children.Add(g);
            //Add child to dropdown
            List<TMP_Dropdown.OptionData> _o = new(){new TMP_Dropdown.OptionData(g.GizProperties[0].GetValueString())};
            Input.AddOptions(_o);
            Input.value = Children.Count-1;
        });
        RemoveBtn.onClick.AddListener(() => {
            //Remove from this
            Children.RemoveAt(Input.value);
            //Remove from dropdown
            Input.options.RemoveAt(Input.value);
            Input.value = 0;
            UpdateValue();
        });
        Input.onValueChanged.AddListener((int val) =>
        {
            int _cnt = Input.options.Count;
            for(int i=0; i<_cnt; i++)
            {
                Input.options[i].text = Children[i].GizProperties[0].GetValueString();
            }
            UpdateValue();
        });
    }
    void CreateChildrenProperties()
    {
        BaseGizmo child = Children[Value];
        foreach(GizProperty prop in child.GizProperties)
        {
            prop.CreateInEditor(ChildrenParent);
        }
    }
    void DestroyChildrenProperties()
    {
        BaseGizmo child = Children[Value];
        foreach (GizProperty prop in child.GizProperties)
        {
            prop.DeleteInEditor();
        }
    }
    public string GetValueString()
    {
        return Value.ToString();
    }
}