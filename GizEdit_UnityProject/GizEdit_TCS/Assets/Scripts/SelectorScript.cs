using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using SFB;
using System;
using Unity.VisualScripting;

public class SelectorScript : MonoBehaviour
{
    //PUBLIC
    public bool mouseHeld = false;
    public Vector2 mouseDelta;
    public float mouseSensitivity = 1 - (800 / 600);
    public string selectedEditorGiz;
    public Transform gizParents;
    public GameObject edit_MoveGiz;

    //PUBLIC UI STUFF
    [Header("UI STUFF")]
    public GameObject loadingStuff, loadingBar;
    public GameObject propsPanel;
    public Transform propsSection;
    public GameObject[] propPrefabs;

    List<GameObject> props = new();
    List<object> propVals = new();

    //PRIVATE
    GameManager gm;
    FileParser fp;
    List<GameObject> gizmos = new();
    GameObject selectedGizmo;
    string GitStr="";
    public GameObject SelectedGizmo{
        get
        {
            if (selectedGizmo == null) return prevSelectedGizmo;
            return selectedGizmo;
        }
        set
        {
            if(value!=null)
                selectedGizmo = value;
        }
    }
    GameObject prevSelectedGizmo;
    bool gizEditing = false;
    GameObject player;
    string gizPath;
    [NonSerialized]
    public bool[] changedGizmoSections = new bool[20];
    //undos
    List<string> undoStack = new();

    [NonSerialized]
    public bool loadingGizmos=false;
    Image barImg;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance.GetComponent<GameManager>();
        fp = GameObject.Find("GizParser").GetComponent<FileParser>();
        player = gm.player;
        for (int i = 1; i < propsSection.childCount; i++) props.Add(propsSection.GetChild(i).gameObject);
        //setProps();
        barImg = loadingBar.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) player = GameObject.Find("Player");
        if (selectedEditorGiz.Contains("move") && SelectedGizmo!=null && mouseHeld)
        {
            gizEditing = true;
            char moveDir = selectedEditorGiz[^1];
            float mv = getCombinedMouseDelta()*Time.deltaTime*getMouseSensativity();
            switch (moveDir)
            {
                case ('X'): SelectedGizmo.transform.Translate(mv, 0, 0); break;
                case ('Y'): SelectedGizmo.transform.Translate(0, mv, 0); break;
                case ('Z'): SelectedGizmo.transform.Translate(0, 0, mv); break;
            }
        }
        else if(gizEditing && SelectedGizmo!=null)
        {
            gizEditing = false;
            SetPropertiesMenu();
        }
    }

    //PUBLIC METHODS
    public void SelectGizmo(GameObject _g)
    {
        SelectedGizmo = _g;
        edit_MoveGiz.SetActive(true);
        edit_MoveGiz.transform.parent = SelectedGizmo.transform;
        edit_MoveGiz.transform.localPosition = Vector3.zero;
        SetPropertiesMenu();
        OpenPopup(propsPanel);
    }
    public void DeselectGizmo()
    {/*
        prevSelectedGizmo = SelectedGizmo;
        SelectedGizmo = null;
        selectedEditorGiz = "";
        edit_MoveGiz.transform.parent = null;
        edit_MoveGiz.SetActive(false);*/
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
            //case (0): break;
            case (1): _g.AddComponent<GizBuildit>(); break;
            case (2): _g.AddComponent<GizForce>(); break;
            case (4): _g.AddComponent<GizmoPickup>(); break;
            default: Destroy(_g); break;
        }
        if (_g != null)
        {
            _g.transform.position = player.transform.position+((player.transform.GetChild(0).forward));
            StartCoroutine(delaySelectGiz(_g));
        }
    }
    public void DeleteGizmo()
    {
        gizmos.Remove(SelectedGizmo);
        GameObject _g = SelectedGizmo;
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
        for (int i = 0; i < changedGizmoSections.Length; i++) changedGizmoSections[i] = false;
        Transform[] _ps = fp.gizmoParents;
        int _cnt = _ps.Length;
        for (int i = 0; i < _cnt; i++) {
            int _psc = _ps[i].childCount;
            for (int j = _psc-1; j >= 0; j--) Destroy(_ps[i].GetChild(j).gameObject);
        }

        var extensions = new[] {
            new ExtensionFilter("Gizmo Files", "GIZ"),
        };
        string[] paths = gm.FileBrowser.OpenFilePanel("Gizmo File Search", "", extensions, false);
        string path = (paths.Length>0)?paths[0]:"";
        if (path.Length != 0)
        {
            gizPath = path[..(path.LastIndexOf(@"\")+1)];
            byte[] fbytes = System.IO.File.ReadAllBytes(path);
            Debug.Log("File Selected: "+path);
            loadingStuff.SetActive(true);
            changeLoadText("LOADING...");
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
        loadingStuff.SetActive(true);
        changeLoadText("COMPILING...");
        string cHex = fp.CompileGizmos();
        StartCoroutine(hextobytes(cHex));
    }
    public void OpenGit()
    {
        //Clear current git file
        GitStr = "";

        var extensions = new[] {
            new ExtensionFilter("Git Files", "GIT"),
        };
        string[] paths = gm.FileBrowser.OpenFilePanel("Git File Search", "", extensions, false);
        string path = (paths.Length > 0) ? paths[0] : "";
        if (path.Length != 0)
        {
            gizPath = path[..(path.LastIndexOf(@"\") + 1)];
            GitStr = System.IO.File.ReadAllText(path);
        }
        else
        {
            Debug.Log("No file selected");
        }
    }
    public void ExportGit()
    {
        var extensions = new[] { new ExtensionFilter("Git Files", "GIT"), };
        string path = gm.FileBrowser.SaveFilePanel("Git File Export", "", "CustomGit.GIT", extensions);
        if (path.Length != 0)
        {
            System.IO.File.WriteAllText(path, GitStr);
        }
    }
    IEnumerator hextobytes(string _h)
    {
        int _l = _h.Length / 3;
        byte[] _b = new byte[_l];
        for (int i = 0; i < _l; i++)
        {
            _b[i] = Convert.ToByte(TypeConverter.HexToInt8(_h.Substring(i * 3, 3)));
            if (i % 2000 == 0) { barImg.fillAmount = (float)i / _l; yield return null; }
        }
        finishExport(_b);
    }
    void finishExport(byte[] compiledBytes)
    {
        var extensions = new[] {new ExtensionFilter("Gizmo Files", "GIZ"),};
        string path = gm.FileBrowser.SaveFilePanel("Gizmo File Export", "", "CustomGizmos.GIZ", extensions);
        if (path.Length != 0)
        {
            System.IO.File.WriteAllBytes(path, compiledBytes);
        }
        loadingStuff.SetActive(false);
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
        //StartCoroutine(setPropsMenu());
    }
   
    int getGizType()
    {
        if (SelectedGizmo.GetComponent<GizBuildit>() != null) return 1;
        if (SelectedGizmo.GetComponent<GizForce>() != null) return 2;
        if (SelectedGizmo.GetComponent<GizmoPickup>() != null) return 4;
        return -1;
    }


    void ssp(int _q){//SetSelectedProperties();
    }
    void ssp(string _q){//SetSelectedProperties();
    }
    void ssp(bool _q){//SetSelectedProperties();
    }
    void AddEventListenerButton(GameObject _g)
    {
        propBtn _p = _g.GetComponent<propBtn>();
        _p.btnSelf = _p.GetComponent<Button>();
        _p.GetComponent<propBtn>().AddListener();
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
        if (SelectedGizmo == null) return 0;
        return (Vector3.Distance(player.transform.position,SelectedGizmo.transform.position)*0.03f);
    }

    GameObject getPropPrefab(string tagname) //this function reminds me of javascript :(
    {
        switch (tagname)
        {
            case ("StringProp"): return propPrefabs[0];
            case ("FloatProp"): return propPrefabs[1];
            case ("IntProp"): return propPrefabs[2];
            case ("BoolProp"): return propPrefabs[3];
            case ("DropProp"): return propPrefabs[4];
            case ("Vec3Prop"): return propPrefabs[5];
            case ("ChildProp"): return propPrefabs[6];
            case ("UnknownProp"): return propPrefabs[7];
            case ("spacingProp"): return propPrefabs[8];
            default: Debug.Log("Error: Invalid prefab type"); return null;
        }
    }
    GameObject InstantiateProp(string tagname,Transform _parent=null)
    {
        if (_parent == null) _parent = propsSection;
        return Instantiate(getPropPrefab(tagname),_parent);
    }
    void AddChildInputListeners(GameObject _prop)
    {
        //AddDropdownListener(TypeConverter.Child_GetPropParent(_prop,-2));
        //AddEventListenerButton(TypeConverter.Child_GetBtns(_prop).GetChild(0).gameObject);
        //AddEventListenerButton(TypeConverter.Child_GetBtns(_prop).GetChild(1).gameObject);
        //List<GameObject> _childProps = TypeConverter.Child_GetProperties(_prop);
        //AddInputListeners(_childProps.ToArray());
    }
    void AddInputListener(GameObject _prop)
    {
        string _t = _prop.tag;
        switch (_t)
        {
            case ("StringProp"): AddInputFieldListener(_prop); break;
            case ("FloatProp"): AddInputFieldListener(_prop); break;
            case ("IntProp"): AddInputFieldListener(_prop); break;
            case ("BoolProp"): AddToggleListener(_prop); break;
            case ("DropProp"): AddDropdownListener(_prop); break;
            case ("Vec3Prop"): AddVector3Listener(_prop); break;
            case ("ChildProp"): AddChildInputListeners(_prop); break;
            case ("UnknownProp"): AddInputFieldListener(_prop); break;
            case ("spacingProp"): break;
        }
    }
    void AddInputListeners(GameObject[] _props)
    {
        foreach (GameObject _p in _props)
        {
            AddInputListener(_p);
        }
    }

    void AddInputFieldListener(GameObject _prop)
    {
        TMP_InputField tmp = _prop.transform.GetChild(1).GetComponent<TMP_InputField>();
        AddInputFieldListener(tmp);
    }
    void AddInputFieldListener(TMP_InputField _propInp)
    {
        _propInp.onEndEdit.AddListener(ssp);
    }
    void AddVector3Listener(GameObject _prop)
    {
        TMP_InputField i1 = _prop.transform.GetChild(1).GetComponent<TMP_InputField>();
        TMP_InputField i2 = _prop.transform.GetChild(2).GetComponent<TMP_InputField>();
        TMP_InputField i3 = _prop.transform.GetChild(3).GetComponent<TMP_InputField>();
        AddInputFieldListener(i1);
        AddInputFieldListener(i2);
        AddInputFieldListener(i3);
    }
    void AddDropdownListener(GameObject _prop)
    {
        TMP_Dropdown tmp = _prop.transform.GetChild(1).GetComponent<TMP_Dropdown>();
        tmp.onValueChanged.AddListener(ssp);
    }
    void AddToggleListener(GameObject _prop)
    {
        Toggle tmp = _prop.transform.GetChild(1).GetComponent<Toggle>();
        tmp.onValueChanged.AddListener(ssp);
    }

}
