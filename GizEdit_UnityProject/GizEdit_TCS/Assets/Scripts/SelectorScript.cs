using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using SFB;
using UnityEditor.PackageManager;
using System;

public class SelectorScript : MonoBehaviour
{
    //PUBLIC
    public bool mouseHeld = false;
    public Vector2 mouseDelta;
    public float mouseSensitivity = 1-(800/600);
    public string selectedEditorGiz;
    public Transform gizParents;
    public GameObject edit_MoveGiz;

    //PUBLIC UI STUFF
    [Header("UI STUFF")]
    public GameObject loadingStuff,loadingBar;
    public GameObject propsPanel;
    public Transform propsSection;

    List<GameObject> props = new();
    List<object> propVals = new();

    //PRIVATE
    GameManager gm;
    FileParser fp;
    List<GameObject> gizmos = new();
    GameObject selectedGizmo;
    GameObject player;
    //undos
    List<string> undoStack = new();

    [NonSerialized]
    public bool loadingGizmos=false;
    Image barImg;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        fp = GameObject.Find("GizParser").GetComponent<FileParser>();
        player = gm.player;
        for (int i = 1; i < propsSection.childCount; i++) props.Add(propsSection.GetChild(i).gameObject);
        setProps();
        barImg = loadingBar.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedEditorGiz.Contains("move") && selectedGizmo!=null && mouseHeld)
        {
            char moveDir = selectedEditorGiz[^1];
            float mv = getCombinedMouseDelta()*Time.deltaTime*getMouseSensativity();
            switch (moveDir)
            {
                case ('X'): selectedGizmo.transform.Translate(mv, 0, 0); break;
                case ('Y'): selectedGizmo.transform.Translate(0, mv, 0); break;
                case ('Z'): selectedGizmo.transform.Translate(0, 0, mv); break;
            }
            Debug.Log(mv + ", " + moveDir);
        }
    }

    //PUBLIC METHODS
    public void SelectGizmo(GameObject _g)
    {
        selectedGizmo = _g;
        edit_MoveGiz.SetActive(true);
        edit_MoveGiz.transform.parent = selectedGizmo.transform;
        edit_MoveGiz.transform.localPosition = Vector3.zero;
        SetPropertiesMenu();
        OpenPopup(propsPanel);
    }
    public void DeselectGizmo()
    {
        selectedGizmo = null;
        selectedEditorGiz = "";
        edit_MoveGiz.transform.parent = null;
        edit_MoveGiz.SetActive(false);
    }
    public void EditGiz(string editName)
    {
        switch (editName)
        {
            case ("moveX"): break;
            case ("moveY"): break;
            case ("moveZ"): break;
        }
    }
    public void SetGizmos(List<GameObject> _gizs)
    {
        foreach (GameObject g in _gizs)
        {
            gizmos.Add(g);
        }
    }
    public void CreateGizmo(TMP_Dropdown _options)
    {
        int gizT = _options.value;
        //_options.value = 0;
        GameObject _g = new();
        switch (gizT)
        {
            case (0): break;
            case (2): _g.AddComponent<GizForce>(); break;
            case (4): _g.AddComponent<GizmoPickup>(); break;
        }
        StartCoroutine(delaySelectGiz(_g));
    }
    public void DeleteGizmo()
    {
        gizmos.Remove(selectedGizmo);
        GameObject _g = selectedGizmo;
        DeselectGizmo();
        Destroy(_g);
    }
    public void QuitApp()
    {
        Application.Quit();
    }
    public void NewFile()
    {
        SceneManager.LoadScene("GizView");
    }
    public void OpenFile()
    {
        //Clear current giz file
        Transform[] _ps = fp.gizmoParents;
        int _cnt = _ps.Length;
        for (int i = 0; i < _cnt; i++) {
            int _psc = _ps[i].childCount;
            for (int j = _psc-1; j >= 0; j--) Destroy(_ps[i].GetChild(j).gameObject);
        }

        var extensions = new[] {
            new ExtensionFilter("Gizmo Files", "GIZ"),
        };
        StandaloneFileBrowserWindows sfbw = new(); //YOOOOOOOOOOOOOO CHANGE FOR MAC / LINUX BUILDS
        string[] paths = sfbw.OpenFilePanel("Gizmo File Search", "", extensions, false);
        string path = (paths.Length>0)?paths[0]:"";
        if (path.Length != 0)
        {
            byte[] fbytes = System.IO.File.ReadAllBytes(path);
            Debug.Log("File Selected: "+path);
            loadingStuff.SetActive(true);
            StartCoroutine(delayOpenFile(fbytes));
            //fp.startLoadGizmos();
        }
        else
        {
            Debug.Log("No file selected");
        }
    }
    public void ExportFile()
    {

    }
    public void ClosePopup(GameObject _popupGroup)
    {
        _popupGroup.SetActive(false);
    }
    public void OpenPopup(GameObject _popupGroup)
    {
        _popupGroup.SetActive(true);
    }
    public void ToggleToolbarColumn(Transform _col)
    {
        bool toggl = !_col.GetChild(1).gameObject.activeSelf;
        for(int i=1; i<_col.childCount; i++)
        {
            _col.GetChild(i).gameObject.SetActive(toggl);
        }
    }
    public void SetShownGizmos(Transform togglesParent)
    {
        for(int i=0; i<20; i++)
            gizParents.GetChild(i).gameObject.SetActive(togglesParent.GetChild(i).gameObject.GetComponent<Toggle>().isOn);
    }
    public void changeLoadText(string txt)
    {
        loadingStuff.GetComponent<TMP_Text>().text = txt;
    }
    public void SetPropertiesMenu()
    {
        int giztype = -1;
        if (selectedGizmo.GetComponent<GizmoPickup>() != null) giztype = 4;
        for (int i = 0; i < props.Count; i++) props[i].SetActive(false);
        switch (giztype)
        {
            case (0): //Obstacles
                
                break;
            case (1): //build it
                
                break;
            case (2): //force
                
                break;
            case (3): //blowup
                
                break;
            case (4): //pick up
                GizmoPickup _pickup = selectedGizmo.GetComponent<GizmoPickup>();
                string _pickuptypes = "sgbpmcuhrt";
                props[0].SetActive(true);
                props[2].SetActive(true);
                props[3].SetActive(true);
                ((TMP_InputField)propVals[0]).text = _pickup.pickupName;
                ((List<TMP_InputField>)propVals[2])[0].text = _pickup.transform.position.x.ToString();
                ((List<TMP_InputField>)propVals[2])[1].text = _pickup.transform.position.y.ToString();
                ((List<TMP_InputField>)propVals[2])[2].text = _pickup.transform.position.z.ToString();
                ((TMP_Dropdown)propVals[3]).value = _pickuptypes.IndexOf(_pickup.pickupType);
                break;
            case (5): //lever
                
                break;
            case (6): //spinner
                
                break;
            case (7): //minicut
                
                break;
            case (8): //tube
                
                break;
            case (9): //zipup
                
                break;
            case (10): //turret
                
                break;
            case (11): //bomb generator
                
                break;
            case (12): //panel
                
                break;
            case (13): //hat machine
                
                break;
            case (14): //push blocks
                
                break;
            case (15): //torp machine
                
                break;
            case (16): //shadow editor
                
                break;
            case (17): //grapple
                
                break;
            case (18): //plug
                
                break;
            case (19): //techno
                
                break;
        }
    }
    public void SetSelectedProperties()
    {

    }

    //PRIVATE METHODS
    float getCombinedMouseDelta()
    {

        return (mouseDelta.x + mouseDelta.y);
    }

    IEnumerator delaySelectGiz(GameObject _g)
    {
        yield return null;
        SelectGizmo(_g);
    }

    IEnumerator delayOpenFile(byte[] _fbytes)
    {
        string _b = "";
        int _l = _fbytes.Length;
        int c = 0;
        while (c < _l)
        {
            _b += TypeConverter.Int8ToHex(_fbytes[c]) + " ";
            c++;
            if (c % 2000 == 0) { barImg.fillAmount = (float)c / _l; yield return null; }
        }
        gm.fhex = _b;
        //FileReader -> read/load the gizmo objects
        fp.startLoadGizmos();
        loadingStuff.SetActive(false);
    }

    float getMouseSensativity()
    {
        return (Vector3.Distance(player.transform.position,selectedGizmo.transform.position)*0.03f);
    }

    void setProps()
    {
        for(int i=0; i<props.Count; i++)
        {
            if (props[i].CompareTag("StringProp"))
            {
                propVals.Add(props[i].transform.GetChild(1).gameObject.GetComponent<TMP_InputField>());
            }
            else if (props[i].CompareTag("FloatProp"))
            {
                propVals.Add(props[i].transform.GetChild(1).gameObject.GetComponent<TMP_InputField>());
            }
            else if (props[i].CompareTag("IntProp"))
            {
                propVals.Add(props[i].transform.GetChild(1).gameObject.GetComponent<TMP_InputField>());
            }
            else if (props[i].CompareTag("Vec3Prop"))
            {
                TMP_InputField _x = props[i].transform.GetChild(1).gameObject.GetComponent<TMP_InputField>();
                TMP_InputField _y = props[i].transform.GetChild(2).gameObject.GetComponent<TMP_InputField>();
                TMP_InputField _z = props[i].transform.GetChild(3).gameObject.GetComponent<TMP_InputField>();
                List<TMP_InputField> _t = new(){_x,_y,_z};
                propVals.Add(_t);
            }
            else if (props[i].CompareTag("DropProp"))
            {
                propVals.Add(props[i].transform.GetChild(1).gameObject.GetComponent<TMP_Dropdown>());
            }
            else
            {
                Debug.Log("ERROR: Unknown property type");
            }
        }
    }
}
