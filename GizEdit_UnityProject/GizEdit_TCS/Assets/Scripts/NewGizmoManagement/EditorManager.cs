using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance;
    public static Transform canvas;
    public static string[] GizmoParentNames = { "obstacles","buildits","forces","blowups",
        "pickups","levers","spinners","minicuts","tubes","zipups","turrets","bombgenerators",
        "panels","hatmachines","pushblocks","torpmachines","shadoweditors",
        "grapples", "plugs", "technos" //unused
    };
    public int[] numEachGiz = new int[17];
    public bool[] SectionsChanged = new bool[17]; //temp?

    public GameObject SelectedGizmo;
    public GameObject PreviousGizmo;
    public GameObject CopiedGizmo;
    public GameObject LastDeletedGizmo;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        canvas = GameObject.Find("Canvas").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ThrowError(string errorMsg)
    {
        //create error popup
        Instantiate(GameManager.gmInstance.popupPrefab, canvas);
    }
    public void OpenPopup(GameObject _popupGroup){_popupGroup.SetActive(true);}
    public void ClosePopup(GameObject _popupGroup){_popupGroup.SetActive(false);}
    public void CreatePopup(GameObject _popupGroup){Instantiate(_popupGroup,canvas);}
    public void DestroyPopup(GameObject _popupGroup){Destroy(_popupGroup);}

    public void CreateGizmo(TMP_Dropdown _options)
    {
        int gizType = _options.value;
        GameObject obj = new();
        BaseGizmo newGiz = GizmosReader.CreateGizmo(gizType,obj);
    }
    public void DeleteGizmo()
    {
        if (SelectedGizmo != null)
        {
            if (LastDeletedGizmo != null) Destroy(LastDeletedGizmo);
            LastDeletedGizmo = SelectedGizmo;
            SelectedGizmo.SetActive(false);
        }
    }
    public void SelectGizmo(GameObject giz)
    {
        PreviousGizmo = SelectedGizmo;
        SelectedGizmo = giz;
        if (!GameManager.gmInstance.propertyPanel.activeSelf) GameManager.gmInstance.propertyPanel.SetActive(true);
        if (GameManager.gmInstance.propertyPanelContent.childCount>0)
        {
            for(int i=GameManager.gmInstance.propertyPanelContent.childCount-1; i>0; i--)
            {
                Destroy(GameManager.gmInstance.propertyPanelContent.GetChild(i).gameObject);
            }
        }
        SelectedGizmo.GetComponent<BaseGizmo>().CreateInEditor();
    }
    public void ExportGizmos()
    {
        StartCoroutine(GizmosWriter.instance.CompileGizmos(GetGizmos(), numEachGiz, SectionsChanged));
    }
    public BaseGizmo[] GetGizmos()
    {
        Transform[] parents = new Transform[17];
        int numGizs = 0;
        for(int i=0; i<17; i++)
        {
            parents[i] = GameObject.Find(GizmoParentNames[i]).transform;
            numEachGiz[i] = parents[i].childCount;
            numGizs += parents[i].childCount;
        }
        BaseGizmo[] gizs = new BaseGizmo[numGizs];
        int cnt=0;
        foreach(Transform parent in parents)
        {
            for(int i=0; i< parent.childCount; i++)
            {
                gizs[cnt] = parent.GetChild(i).GetComponent<BaseGizmo>();
                cnt++;
            }
        }
        return gizs;
    }
    public void CheckValues()
    {
        BaseGizmo[] gizs = GetGizmos();
        foreach(BaseGizmo giz in gizs)
        {
            giz.CheckValues();
        }
    }
}
