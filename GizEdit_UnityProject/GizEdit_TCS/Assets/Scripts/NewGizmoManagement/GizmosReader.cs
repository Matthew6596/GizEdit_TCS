using SFB;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GizmosReader : MonoBehaviour
{
    static public GizmosReader instance;
    static public string LastReadPath;
    GameManager gm;

    public static IGizmosReader reader;

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

        reader = new TCSGizmosReader(); //temp - TCS default
    }

    public void OpenGizFile()
    {
        //Clear current giz file
        /*int len = EditorManager.GizmoParentNames.Length;
        Transform[] _ps = new Transform[len];
        for (int i = 0; i < len; i++)
        {
            _ps[i] = GameObject.Find(EditorManager.GizmoParentNames[i]).transform;
            int _cnt = _ps.Length;
            for (int j = 0; j < _cnt; j++)
            {
                int _psc = _ps[j].childCount;
                for (int k = _psc - 1; k >= 0; k--) Destroy(_ps[j].GetChild(k).gameObject);
            }
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
    public BaseGizmo CreateGizmo(int section, GameObject obj)
    {
        return reader.CreateGizmo(section, obj);
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
        StartCoroutine(reader.ReadGizmos());
    }
    
    static List<string> gizTypes = new List<string>{"GizObstacle","GizBuildit","GizForce","blowup","GizmoPickup","Lever","Spinner","MiniCut",
    "Tube","ZipUp","GizTurret","BombGenerator","Panel","HatMachine","PushBlocks","Torp Machine","ShadowEditor","GizObstacleChild",
        "GizBuilditChild","GizForceChild","blowupChild"};
    public static int GetGizType(string typeName)
    {
        return gizTypes.IndexOf(typeName);
    }
}
