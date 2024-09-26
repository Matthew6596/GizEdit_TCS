using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance;
    public static Transform canvas;
    public static string[] GizmoParentNames = { "obstacles","buildits","forces","blowups",
        "pickups","levers","spinners","minicuts","tubes","zipups","turrets","bombgenerators",
        "panels","hatmachines","pushblocks","torpmachines","shadoweditors",
        "grapples", "plugs", "technos" //unused
    };
    public Transform moveGizmo;
    public int[] numEachGiz = new int[17];
    //public bool[] SectionsChanged = new bool[17];

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

    public static void ThrowError(string errorMsg)
    {
        //create error popup
        GameObject popup = GameManager.gmInstance.defaultPopup;
        popup.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Error";
        popup.transform.GetChild(1).GetComponent<TMP_Text>().text = errorMsg;
        popup.SetActive(true);
    }
    public void OpenPopup(GameObject _popupGroup){_popupGroup.SetActive(true);}
    public void ClosePopup(GameObject _popupGroup){_popupGroup.SetActive(false); if (bufferFunction != null) bufferFunction = null; }
    public void CreatePopup(GameObject _popupGroup){Instantiate(_popupGroup,canvas);}
    public void DestroyPopup(GameObject _popupGroup){Destroy(_popupGroup);}

    public void CreateGizmo(TMP_Dropdown _options)
    {
        int gizType = _options.value;
        GameObject obj = new();
        BaseGizmo newGiz = GizmosReader.instance.CreateGizmo(gizType,obj);
    }
    public void DeleteGizmo()
    {
        if (SelectedGizmo != null)
        {
            if (LastDeletedGizmo != null) Destroy(LastDeletedGizmo);
            LastDeletedGizmo = SelectedGizmo;
            SelectedGizmo.SetActive(false);
            moveGizmo.gameObject.SetActive(false);
        }
    }
    public void SelectGizmo(GameObject giz)
    {
        moveGizmo.gameObject.SetActive(true);
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
        moveGizmo.position = SelectedGizmo.transform.position;
    }
    public void UpdateSelectedPos()
    {
        if(SelectedGizmo!=null) SelectedGizmo.transform.position = moveGizmo.GetChild(0).position;
    }
    public void ExportGizmos()
    {
        StartCoroutine(GizmosWriter.instance.CompileGizmos(GetGizmos(), numEachGiz));
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

    PopupBtnFunction bufferFunction;
    public void BufferPopupFunction(PopupBtnFunction btn){bufferFunction = btn;}
    public void ExecuteBufferFunction(){if (bufferFunction != null) bufferFunction.Confirm();}
    public void OpenURL(string url){Application.OpenURL(url);}
}
