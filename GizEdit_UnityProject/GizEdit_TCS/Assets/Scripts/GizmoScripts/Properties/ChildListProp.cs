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
        if (inc03) GizmosReader.reader.ReadLocation++;
        Children = new();
        uint _l = GameManager.gmInstance.FSliceInt8(GizmosReader.reader.ReadLocation);
        GizmosReader.reader.ReadLocation++;
        for (int i=0; i<_l; i++)
        {
            GameObject c = new();
            BaseGizmo g = GizmosReader.instance.CreateGizmo(GizmosReader.GetGizType(DefaultChild.GetGizType()), c);
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
        if (Children.Count > 0)
        {
            DestroyChildrenProperties();
            SetValue(Input.value);
            CreateChildrenProperties();
        }
        else
        {

        }
    }
    public void DeleteInEditor()
    {
        GameObject.Destroy(EditorInstance);
    }
    public void CreateInEditor(Transform contentArea=null)
    {
        if (contentArea == null) contentArea = GameManager.gmInstance.propertyPanelContent;

        //Create prop prefab
        GameObject.Instantiate(GameManager.gmInstance.propPrefabs[7], contentArea); //adding spacing
        GameObject.Instantiate(GameManager.gmInstance.propPrefabs[7], contentArea);
        GameObject.Instantiate(GameManager.gmInstance.propPrefabs[7], contentArea);
        EditorInstance = GameObject.Instantiate(GameManager.gmInstance.propPrefabs[1], contentArea);
        GameObject.Instantiate(GameManager.gmInstance.propPrefabs[7], contentArea); //adding spacing
        GameObject.Instantiate(GameManager.gmInstance.propPrefabs[7], contentArea);
        GameObject.Instantiate(GameManager.gmInstance.propPrefabs[7], contentArea);
        //Get Inputs
        ChildrenParent = EditorInstance.transform.GetChild(0).GetChild(0).GetChild(0);
        Input = ChildrenParent.GetChild(1).GetChild(1).GetComponent<TMP_Dropdown>();
        AddBtn = ChildrenParent.GetChild(2).GetChild(0).GetComponent<Button>();
        RemoveBtn = ChildrenParent.GetChild(2).GetChild(1).GetComponent<Button>();
        //Create dropdown options
        List<TMP_Dropdown.OptionData> options = new();
        Input.options.Clear();
        if (Children == null) Children = new();
        foreach (BaseGizmo child in Children)
        {
            options.Add(new TMP_Dropdown.OptionData(child.GizProperties[0].GetValueString()));
        }
        Input.AddOptions(options);
        //Set name and selected option
        Input.value = Value;
        ChildrenParent.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Name;
        //Create children properties
        if(Children.Count>0)
            CreateChildrenProperties();
        //Set up event listeners
        AddBtn.onClick.AddListener(() => {
            if (Children.Count < 255)
            {
                //Create new child
                GameObject c = new();
                BaseGizmo g = GizmosReader.instance.CreateGizmo(GizmosReader.GetGizType(DefaultChild.GetGizType()), c);
                Children.Add(g);
                //Add child to dropdown
                List<TMP_Dropdown.OptionData> _o = new() { new TMP_Dropdown.OptionData(g.GizProperties[0].GetValueString()) };
                Input.AddOptions(_o);
                Input.value = Children.Count - 1;

                if (Children.Count == 1)
                {
                    UpdateValue();
                }
            }
        });
        RemoveBtn.onClick.AddListener(() => {
            if (Children.Count > 0)
            {
                int removedChild = Input.value;
                //Remove from this
                Children.RemoveAt(removedChild);
                //Remove from dropdown
                Input.options.RemoveAt(removedChild);

                Input.value = 0;
                UpdateValue();
                Input.value = 1;
                Input.value = 0;

                if (removedChild == 0)
                {
                    UpdateValue();
                    if (Children.Count <= 0)
                    {
                        DestroyChildrenProperties();
                    }
                }
            }
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
        for(int i=ChildrenParent.childCount-1; i>=3; i--)
        {
            GameObject.Destroy(ChildrenParent.GetChild(i).gameObject);
        }
        /*BaseGizmo child = Children[Value];
        foreach (GizProperty prop in child.GizProperties)
        {
            prop.DeleteInEditor();
        }*/
    }
    public string GetValueString()
    {
        return Value.ToString();
    }
}
