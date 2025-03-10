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
    public static int[] numGizSubSections = { 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

    public Transform moveGizmo;
    public int[][] numEachGiz = new int[17][];
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
        GameObject popup = GameManager.gm.defaultPopup;
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
        if (!GameManager.gm.propertyPanel.activeSelf) GameManager.gm.propertyPanel.SetActive(true);
        ClearPropPanel();
        SelectedGizmo.GetComponent<BaseGizmo>().CreateInEditor();
        moveGizmo.position = SelectedGizmo.transform.position;
    }
    private void ClearPropPanel()
    {
        if (GameManager.gm.propertyPanelContent.childCount > 0)
        {
            for (int i = GameManager.gm.propertyPanelContent.childCount - 1; i > 0; i--)
            {
                Destroy(GameManager.gm.propertyPanelContent.GetChild(i).gameObject);
            }
        }
    }
    public void UpdateSelectedPos()
    {
        if (SelectedGizmo != null)
        {
            SelectedGizmo.transform.position = moveGizmo.GetChild(0).position;
            BaseGizmo giz = SelectedGizmo.GetComponent<BaseGizmo>();
            giz.SetProp("Position", SelectedGizmo.transform.position);
            ClearPropPanel();
            giz.CreateInEditor();
        }
    }
    public void ExportGizmos()
    {
        StartCoroutine(GizmosWriter.instance.CompileGizmos(GetGizmos(), numEachGiz));
    }
    public BaseGizmo[] GetGizmos()
    {
        List<Transform> parents = new();
        int numGizs = 0;
        for(int i=0; i<17; i++)
        {
            numEachGiz[i] = new int[numGizSubSections[i]];
            GameObject p = GameObject.Find(GizmoParentNames[i]);
            if (p== null) continue;
            parents.Add(p.transform);

            int childCnt = parents[^1].childCount;
            if (i == 3) //blowup
            {
                int l = FindObjectsOfType<blowupFx>().Length;
                numEachGiz[i][0] = l;
                numEachGiz[i][1] = childCnt-l;
            }
            else //not blowup
            {
                numEachGiz[i][0] = childCnt;
            }
            numGizs += childCnt;
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

    public void TeleportPlayer(Transform pos)
    {
        CamMovement.player.transform.position = pos.position;
    }
    public void TeleportPlayerFirstGiz()
    {
        BaseGizmo giz = GetGizmos()[0];
        Transform player = CamMovement.player.transform;
        Transform cam = player.GetChild(0);
        player.position = giz.transform.position + new Vector3(-1, 1, -1);
        //player.LookAt(giz.transform.position);
        cam.LookAt(giz.transform.position);
        player.rotation = Quaternion.Euler(0, cam.eulerAngles.y,0);
        cam.localRotation = Quaternion.Euler(cam.eulerAngles.x, 0, 0);
        
    }

    PopupBtnFunction bufferFunction;
    public void BufferPopupFunction(PopupBtnFunction btn){bufferFunction = btn;}
    public void ExecuteBufferFunction(){if (bufferFunction != null) bufferFunction.Confirm();}
    public void OpenURL(string url){Application.OpenURL(url);}
}
