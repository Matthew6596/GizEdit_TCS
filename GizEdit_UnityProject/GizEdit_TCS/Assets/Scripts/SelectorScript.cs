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
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        fp = GameObject.Find("GizParser").GetComponent<FileParser>();
        player = gm.player;
        for (int i = 1; i < propsSection.childCount; i++) props.Add(propsSection.GetChild(i).gameObject);
        //setProps();
        barImg = loadingBar.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
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
            case (2): _g.AddComponent<GizForce>(); break;
            case (4): _g.AddComponent<GizmoPickup>(); break;
            default: Destroy(_g); break;
        }
        if(_g!=null)
            StartCoroutine(delaySelectGiz(_g));
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
        StandaloneFileBrowserWindows sfbw = new(); //YOOOOOOOOOOOOOO CHANGE FOR MAC / LINUX BUILDS
        string[] paths = sfbw.OpenFilePanel("Gizmo File Search", "", extensions, false);
        string path = (paths.Length>0)?paths[0]:"";
        if (path.Length != 0)
        {
            gizPath = path[..(path.LastIndexOf(@"\")+1)];
            Debug.Log("Yuh: " + gizPath);
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
        StandaloneFileBrowserWindows sfbw = new(); //YOOOOOOOOOOOOOO CHANGE FOR MAC / LINUX BUILDS
        var extensions = new[] {new ExtensionFilter("Gizmo Files", "GIZ"),};
        string path = sfbw.SaveFilePanel("Gizmo File Export", "", "CustomGizmos.GIZ", extensions);
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
        StartCoroutine(setPropsMenu());
    }
    public void SetSelectedProperties()
    {
        int giztype = getGizType();
        changedGizmoSections[giztype]= true;
        for (int i = propsSection.childCount - 1; i >= 0; i--) Destroy(propsSection.GetChild(i).gameObject); //clear properties
        switch (giztype)
        {
            case (0): //Obstacles

                break;
            case (1): //build it

                break;
            case (2): //force
                GizForce _force = SelectedGizmo.GetComponent<GizForce>();
                //Get and read property fields
                //GameObject force_refName = propsSection.GetChild(1).gameObject;
                GameObject force_pos = propsSection.GetChild(2).gameObject;
                GameObject force_resetTime = propsSection.GetChild(3).gameObject;
                GameObject force_shakeTime = propsSection.GetChild(4).gameObject;
                GameObject force_range = propsSection.GetChild(5).gameObject;
                GameObject force_darkSide = propsSection.GetChild(6).gameObject;
                GameObject force_unknown1 = propsSection.GetChild(7).gameObject;
                GameObject force_unknown2  = propsSection.GetChild(8).gameObject;
                GameObject force_children  = propsSection.GetChild(9).gameObject;
                GameObject force_forceSpd = propsSection.GetChild(10).gameObject;
                GameObject force_resetSpd  = propsSection.GetChild(11).gameObject;
                GameObject force_unknown3 = propsSection.GetChild(12).gameObject;
                GameObject force_effectScale  = propsSection.GetChild(13).gameObject;
                GameObject force_unknown4  = propsSection.GetChild(14).gameObject;
                GameObject force_minStud  = propsSection.GetChild(15).gameObject;
                GameObject force_maxStud = propsSection.GetChild(16).gameObject;
                GameObject force_unknown5  = propsSection.GetChild(17).gameObject;
                GameObject force_studSpawn = propsSection.GetChild(18).gameObject;
                GameObject force_studSpd = propsSection.GetChild(19).gameObject;
                //Set force data
                _force.transform.position = TypeConverter.Prop_GetVec3(force_pos);
                _force.resetTime = float.Parse(TypeConverter.Prop_GetInputField(force_resetTime));
                _force.shakeTime = float.Parse(TypeConverter.Prop_GetInputField(force_shakeTime));
                _force.range = float.Parse(TypeConverter.Prop_GetInputField(force_range));
                _force.SetDarkSide(TypeConverter.Prop_GetToggle(force_darkSide));
                _force.unknown1 = TypeConverter.Prop_GetInputField(force_unknown1);
                _force.unknown2 = TypeConverter.Prop_GetInputField(force_unknown2);
                int _forcechild_prevSelect = _force.selectedChild;
                _force.selectedChild = TypeConverter.Child_GetSelected(force_children);
                GizForceChild _forceC = _force.childrenList[_force.selectedChild].GetComponent<GizForceChild>();
                if(_forcechild_prevSelect==_force.selectedChild)
                    _forceC.gizName = TypeConverter.Prop_GetInputField(TypeConverter.Child_GetPropParent(force_children,0));
                _force.forceSpeed = float.Parse(TypeConverter.Prop_GetInputField(force_forceSpd));
                _force.resetSpeed = float.Parse(TypeConverter.Prop_GetInputField(force_resetSpd));
                _force.unknown3 = TypeConverter.Prop_GetInputField(force_unknown3);
                _force.effectScale = float.Parse(TypeConverter.Prop_GetInputField(force_effectScale));
                _force.unknown4 = TypeConverter.Prop_GetInputField(force_unknown4);
                _force.minStudValue = (uint)int.Parse(TypeConverter.Prop_GetInputField(force_minStud));
                _force.minStudValue = (uint)int.Parse(TypeConverter.Prop_GetInputField(force_maxStud));
                _force.unknown5 = TypeConverter.Prop_GetInputField(force_unknown5);
                _force.studSpawnPosition = TypeConverter.Prop_GetVec3(force_studSpawn);
                _force.studSpeed = float.Parse(TypeConverter.Prop_GetInputField(force_studSpd));

                break;
            case (3): //blowups

                break;
            case (4): //pick up
                GizmoPickup _pickup = SelectedGizmo.GetComponent<GizmoPickup>();
                //Get and read property fields
                GameObject pickup_name = propsSection.GetChild(1).gameObject;
                GameObject pickup_position = propsSection.GetChild(2).gameObject;
                GameObject pickup_type = propsSection.GetChild(3).gameObject;
                GameObject pickup_spawntype = propsSection.GetChild(4).gameObject;
                GameObject pickup_unknown1 = propsSection.GetChild(5).gameObject;
                //Set pickup data
                _pickup.pickupName = TypeConverter.Prop_GetInputField(pickup_name);
                _pickup.transform.position = TypeConverter.Prop_GetVec3(pickup_position);
                _pickup.SetType(TypeConverter.Prop_GetDropdown(pickup_type));
                _pickup.SpawnType = TypeConverter.Prop_GetDropdown(pickup_spawntype);
                _pickup.spawnGroup = (uint)int.Parse(TypeConverter.Prop_GetInputField(pickup_unknown1));

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
    int getGizType()
    {
        if (SelectedGizmo.GetComponent<GizForce>() != null) return 2;
        if (SelectedGizmo.GetComponent<GizmoPickup>() != null) return 4;
        return -1;
    }
    void removeDropOption(GameObject _prop, int _index)
    {
        TMP_Dropdown _d = _prop.transform.GetChild(1).gameObject.GetComponent<TMP_Dropdown>();
        _d.options.RemoveAt(_index);
        switch (getGizType())
        {
            case (2):
                SelectedGizmo.GetComponent<GizForce>().childrenList.RemoveAt(_index); break;
            case (4): break;
        }
    }
    void addDropOption(GameObject _prop, string _option)
    {
        TMP_Dropdown _d = _prop.transform.GetChild(1).gameObject.GetComponent<TMP_Dropdown>();
        _d.options.Add(new(_option));
        switch (getGizType())
        {
            case (2):GameObject _c = new(); GizForceChild _g = _c.AddComponent<GizForceChild>(); _g.gizName = _option;
                SelectedGizmo.GetComponent<GizForce>().childrenList.Add(_c); break;
            case (4): break;
        }
    }

    void ssp(int _q){SetSelectedProperties();}
    void ssp(string _q){SetSelectedProperties();}
    void ssp(bool _q){SetSelectedProperties();}
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
        AddDropdownListener(TypeConverter.Child_GetPropParent(_prop,-2));
        AddEventListenerButton(TypeConverter.Child_GetBtns(_prop).GetChild(0).gameObject);
        AddEventListenerButton(TypeConverter.Child_GetBtns(_prop).GetChild(1).gameObject);
        List<GameObject> _childProps = TypeConverter.Child_GetProperties(_prop);
        AddInputListeners(_childProps.ToArray());
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
        Debug.Log(_prop.name);
        TMP_Dropdown tmp = _prop.transform.GetChild(1).GetComponent<TMP_Dropdown>();
        tmp.onValueChanged.AddListener(ssp);
    }
    void AddToggleListener(GameObject _prop)
    {
        Toggle tmp = _prop.transform.GetChild(1).GetComponent<Toggle>();
        tmp.onValueChanged.AddListener(ssp);
    }

    IEnumerator setPropsMenu()
    {
        int giztype = getGizType();
        for (int i = propsSection.childCount - 1; i >= 0; i--) Destroy(propsSection.GetChild(i).gameObject); //clear properties
        if (giztype == -1) { ClosePopup(propsPanel); }
        else
        {
            InstantiateProp("spacingProp");
            switch (giztype)
            {
                case (0): //Obstacles

                    break;
                case (1): //build it

                    break;
                case (2): //force
                          //Create property fields
                    GameObject force_refName = InstantiateProp("StringProp");
                    GameObject force_position = InstantiateProp("Vec3Prop");
                    GameObject force_resetTime = InstantiateProp("FloatProp");
                    GameObject force_shakeTime = InstantiateProp("FloatProp");
                    GameObject force_range = InstantiateProp("FloatProp");
                    GameObject force_darkSide = InstantiateProp("BoolProp");
                    //GameObject force_endState = InstantiateProp("DropProp");
                    GameObject force_unknown1 = InstantiateProp("UnknownProp");
                    //GameObject force_toggleForce
                    GameObject force_unknown2 = InstantiateProp("UnknownProp");
                    GameObject force_children = InstantiateProp("ChildProp");
                        GameObject forceChild_gizName = InstantiateProp("StringProp",TypeConverter.Child_GetParent(force_children));
                    GameObject force_forceSpd = InstantiateProp("FloatProp");
                    GameObject force_resetSpd = InstantiateProp("FloatProp");
                    GameObject force_unknown3 = InstantiateProp("UnknownProp");
                    GameObject force_effectScale = InstantiateProp("FloatProp");
                    GameObject force_unknown4 = InstantiateProp("UnknownProp");
                    GameObject force_minStud = InstantiateProp("IntProp");
                    GameObject force_maxStud = InstantiateProp("IntProp");
                    GameObject force_unknown5 = InstantiateProp("UnknownProp");
                    //GameObject force_unknown6 = InstantiateProp("UnknownProp");
                    GameObject force_studSpawn = InstantiateProp("Vec3Prop");
                    GameObject force_studSpd = InstantiateProp("FloatProp");
                    //GameObject force_sfxs = InstantiateProp("")
                    yield return null;
                    TypeConverter.Prop_SetLabel(force_refName, "Gizmo Name");
                    TypeConverter.Prop_SetLabel(force_position, "Position");
                    TypeConverter.Prop_SetLabel(force_resetTime, "Reset Time");
                    TypeConverter.Prop_SetLabel(force_shakeTime, "Shake Time");
                    TypeConverter.Prop_SetLabel(force_range, "Range");
                    TypeConverter.Prop_SetLabel(force_darkSide, "Dark Side");
                    TypeConverter.Prop_SetLabel(TypeConverter.Child_GetPropParent(force_children,-2), "Children");
                        TypeConverter.Prop_SetLabel(forceChild_gizName, "Child Name");
                    TypeConverter.Prop_SetLabel(force_forceSpd, "Force Speed");
                    TypeConverter.Prop_SetLabel(force_resetSpd, "Reset Speed");
                    TypeConverter.Prop_SetLabel(force_effectScale, "Effect Size");
                    TypeConverter.Prop_SetLabel(force_minStud, "Min Stud Value");
                    TypeConverter.Prop_SetLabel(force_maxStud, "Max Stud Value");
                    TypeConverter.Prop_SetLabel(force_studSpawn, "Stud Spawn");
                    TypeConverter.Prop_SetLabel(force_studSpd, "Stud Speed");

                    TypeConverter.Child_GetBtns(force_children).GetChild(0).GetComponent<propBtn>().function = () => { addDropOption(TypeConverter.Child_GetPropParent(force_children,-2), "newForceChild"); };
                    TypeConverter.Child_GetBtns(force_children).GetChild(1).GetComponent<propBtn>().function = () => { removeDropOption(TypeConverter.Child_GetPropParent(force_children,-2), TypeConverter.Child_GetSelected(force_children)); };

                    //Set properties data
                    GizForce _force = SelectedGizmo.GetComponent<GizForce>();

                    TypeConverter.Prop_SetInputField(force_refName, _force.referenceName);
                    TypeConverter.Prop_SetVec3(force_position, _force.transform.position);
                    TypeConverter.Prop_SetInputField(force_resetTime, _force.resetTime);
                    TypeConverter.Prop_SetInputField(force_shakeTime, _force.shakeTime);
                    TypeConverter.Prop_SetInputField(force_range, _force.range);
                    TypeConverter.Prop_SetToggle(force_darkSide, _force.darkSide);
                    TypeConverter.Prop_SetInputField(force_unknown1, _force.unknown1);
                    TypeConverter.Prop_SetInputField(force_unknown2, _force.unknown2);
                    TypeConverter.Prop_SetChildren(force_children, _force.childrenList,_force.selectedChild);
                        TypeConverter.Prop_SetInputField(forceChild_gizName, _force.childrenList[TypeConverter.Child_GetSelected(force_children)].GetComponent<GizForceChild>().gizName);
                    TypeConverter.Prop_SetInputField(force_forceSpd, _force.forceSpeed);
                    TypeConverter.Prop_SetInputField(force_resetSpd, _force.resetSpeed);
                    TypeConverter.Prop_SetInputField(force_unknown3, _force.unknown3);
                    TypeConverter.Prop_SetInputField(force_effectScale, _force.effectScale);
                    TypeConverter.Prop_SetInputField(force_unknown4, _force.unknown4);
                    TypeConverter.Prop_SetInputField(force_minStud, _force.minStudValue);
                    TypeConverter.Prop_SetInputField(force_maxStud, _force.maxStudValue);
                    TypeConverter.Prop_SetInputField(force_unknown5, _force.unknown5);
                    TypeConverter.Prop_SetVec3(force_studSpawn, _force.studSpawnPosition);
                    TypeConverter.Prop_SetInputField(force_studSpd, _force.studSpeed);

                    GameObject[] _forceProps = { force_refName, force_position, force_resetTime, force_shakeTime,
                force_range, force_darkSide, force_forceSpd, force_resetSpd, force_effectScale,
                force_minStud, force_maxStud, force_studSpawn, force_studSpd , force_children, forceChild_gizName};
                    AddInputListeners(_forceProps);
                    break;
                case (3): //blowup

                    break;
                case (4): //pick up
                          //Create property fields
                    GameObject pickup_name = InstantiateProp("StringProp");
                    GameObject pickup_position = InstantiateProp("Vec3Prop");
                    GameObject pickup_type = InstantiateProp("DropProp");
                    GameObject pickup_spawntype = InstantiateProp("DropProp");
                    GameObject pickup_unknown1 = InstantiateProp("IntProp");
                    yield return null;
                    TypeConverter.Prop_SetLabel(pickup_name, "Gizmo Name");
                    TypeConverter.Prop_SetLabel(pickup_position, "Position");
                    TypeConverter.Prop_SetLabel(pickup_type, "Pickup Type");
                    TypeConverter.Prop_SetLabel(pickup_spawntype, "Spawn Type");
                    TypeConverter.Prop_SetLabel(pickup_unknown1, "Spawn Group");

                    //Set properties data
                    GizmoPickup _pickup = SelectedGizmo.GetComponent<GizmoPickup>();

                    TypeConverter.Prop_SetInputField(pickup_name, _pickup.pickupName);
                    TypeConverter.Prop_SetVec3(pickup_position, _pickup.transform.position);
                    TypeConverter.Prop_SetDropdown(pickup_type, GizmoPickup.pickupTypeNames, GizmoPickup.pickupTypes.IndexOf(_pickup.pickupType));
                    TypeConverter.Prop_SetDropdown(pickup_spawntype, GizmoPickup.spawnTypeNames, _pickup.SpawnType);
                    TypeConverter.Prop_SetInputField(pickup_unknown1, _pickup.spawnGroup);

                    GameObject[] _pickupProps = { pickup_name, pickup_position, pickup_type, pickup_spawntype, pickup_unknown1 };
                    AddInputListeners(_pickupProps);
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
    }
}
