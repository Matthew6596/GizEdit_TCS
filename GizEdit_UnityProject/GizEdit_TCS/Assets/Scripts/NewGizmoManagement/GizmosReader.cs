using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosReader : MonoBehaviour
{
    static public GizmosReader instance;
    static public uint ReadLocation;
    static public string LastReadPath;
    GameManager gm;
    public static int B(int _bytes)
    {
        return (_bytes * 3);
    }
    public static int B(uint _bytes)
    {
        return ((int)_bytes * 3);
    }
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gmInstance;
        instance = this;
    }

    public void OpenGizFile()
    {
        //Clear current giz file
            /*for (int i = 0; i < changedGizmoSections.Length; i++) changedGizmoSections[i] = false;
            Transform[] _ps = fp.gizmoParents;
            int _cnt = _ps.Length;
            for (int i = 0; i < _cnt; i++)
            {
                int _psc = _ps[i].childCount;
                for (int j = _psc - 1; j >= 0; j--) Destroy(_ps[i].GetChild(j).gameObject);
            }*/

        var extensions = new[] {
            new ExtensionFilter("Gizmo Files", new string[]{"GIZ","giz","Giz"}),
        };
        string[] paths = gm.FileBrowser.OpenFilePanel("Gizmo File Search", "", extensions, false);
        string path = (paths.Length > 0) ? paths[0] : "";
        if (path.Length != 0)
        {
            LastReadPath = path;
            byte[] fbytes = System.IO.File.ReadAllBytes(path);
            /*Debug.Log("File Selected: " + path);
            loadingStuff.SetActive(true);
            changeLoadText("LOADING...");*/
            StartCoroutine(BytesToHex(fbytes));
            //fp.startLoadGizmos();
        }
        else
        {
            Debug.Log("No file selected");
        }
    }
    public IEnumerator BytesToHex(byte[] _fbytes)
    {
        yield return null; //<<temp
        string _b = "";
        int _l = _fbytes.Length;
        int c = 0;
        while (c < _l)
        {
            _b += TypeConverter.Int8ToHex(_fbytes[c]) + " ";
            c++;

            //if (c % 2000 == 0) { barImg.fillAmount = (float)c / _l; yield return null; }
        }
        gm.fhex = _b;
        StartCoroutine(ReadGizmos());
    }
    bool[] sectionReady = {false,false,false, false, true, false, false, false, false, false, false, false, false, false, false, false, false };
    public IEnumerator ReadGizmos()
    {
        ReadLocation = 4; //skip 01 00 00 00

        for (int i = 0; i < 17; i++)
        {
            yield return null;
            if (validateSection(i))
            {
                uint sectionLength = gm.FSliceInt32(ReadLocation); ReadLocation += 4;
                if (sectionReady[i])
                    ReadGizmoSection(i);
                else
                    ReadLocation += sectionLength;
            }
            else
            {
                EditorManager.ThrowError("ERROR: Couldn't validate header for " + TypeConverter.HexToString(TypeConverter.headerHex[i][4..]));
            }

            //progress bar
        }
    }
    public void ReadGizmoSection(int readSection)
    {
        uint numGizs = ReadExtraHeaderStuff(readSection);
        for(int i=0; i<numGizs; i++)
        {
            GameObject obj = new();
            BaseGizmo giz = CreateGizmo(readSection, obj);
            giz.ReadFromHex();
        }
    }
    bool validateSection(int n)
    {
        uint hl = gm.FSliceInt32(ReadLocation); ReadLocation += 4;
        string header = gm.FSliceString(ReadLocation, hl); ReadLocation+=hl;
        return (TypeConverter.getHeader(header) == TypeConverter.headerHex[n]);
    }
    uint ReadExtraHeaderStuff(int gizSection)
    {
        uint n = 0;
        switch (gizSection)
        {
            //Obstacle
            case 0: return n;
            //Buildit
            case 1:
                n = gm.FSliceInt16(ReadLocation + 1);
                ReadLocation += 3;
                return n;
            //Force
            case 2: return n;
            //Blowup
            case 3: return n;
            //Pickup
            case 4:
                uint pickupH = gm.FSliceInt32(ReadLocation); ReadLocation += 4;
                n = gm.FSliceInt32(ReadLocation); ReadLocation += 8;
                if (pickupH == 7) ReadLocation += 8;
                return n;
            //Lever
            case 5: return n;
            //Spinner
            case 6: return n;
            //Minicut
            case 7: return n;
            //Tube
            case 8: return n;
            //ZipUp
            case 9: return n;
            //Turret
            case 10: return n;
            //BombGenerator
            case 11: return n;
            //Panel
            case 12: return n;
            //HatMachine
            case 13: return n;
            //PushBlocks
            case 14: return n;
            //TorpMachine
            case 15: return n;
            //ShadowEditor
            case 16: return n;
        }
        return n;
    }
    public static BaseGizmo CreateGizmo(int section, GameObject obj)
    {
        BaseGizmo g=null; //<<temp!!!
        switch (section)
        {
            //Obstacle
            case 0: return g;
            //Buildit
            case 1: return g;
            //Force
            case 2: return g;
            //Blowup
            case 3: return g;
            //Pickup
            case 4: return obj.AddComponent<GizmoPickup>();
            //Lever
            case 5: return g;
            //Spinner
            case 6: return g;
            //Minicut
            case 7: return g;
            //Tube
            case 8: return g;
            //ZipUp
            case 9: return g;
            //Turret
            case 10: return g;
            //BombGenerator
            case 11: return g;
            //Panel
            case 12: return g;
            //HatMachine
            case 13: return g;
            //PushBlocks
            case 14: return g;
            //TorpMachine
            case 15: return g;
            //ShadowEditor
            case 16: return g;
        }
        Destroy(obj);
        return null;
    }
}
