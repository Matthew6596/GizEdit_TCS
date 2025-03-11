using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class GizmosReader : MonoBehaviour
{
    static public GizmosReader instance;
    static public string LastReadPath;
    GameManager gm;

    public static IGizmosReader reader;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gm;
        instance = this;

        reader = new TCSGizmosReader(); //temp - TCS default
    }

    public void OpenGizFile(string file="")
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
        if (file != string.Empty)
        {
            LastReadPath = file;
            GameManager.gm.bytes = System.IO.File.ReadAllBytes(file);
            StartCoroutine(reader.ReadGizmos());
        }
        else
        {
            var extensions = new[] {
            new ExtensionFilter("Gizmo Files", new string[]{"GIZ","giz","Giz"}),
            };
            string[] paths = gm.FileBrowser.OpenFilePanel("Gizmo File Search", "", extensions, false);
            string path = (paths.Length > 0) ? paths[0] : "";
            if (path.Length != 0)
            {
                LastReadPath = path;
                GameManager.gm.bytes = System.IO.File.ReadAllBytes(path);
                StartCoroutine(reader.ReadGizmos());
            }
            else
            {
                Debug.Log("No file selected");
            }
        }
    }

    public void OpenGSCFile()
    {
        var extensions = new[] {
            new ExtensionFilter("Scene Files", new string[]{"GSC","gsc","Gsc"}),
        };
        string[] paths = gm.FileBrowser.OpenFilePanel("Scene File Search", "", extensions, false);
        string path = (paths.Length > 0) ? paths[0] : "";
        if (path.Length != 0)
        {
            LastReadPath = path;
            GameManager.gm.gscBytes = System.IO.File.ReadAllBytes(path);
            NTBLReader.inst.ReadTable();
        }
        else
        {
            Debug.Log("No file selected");
        }
    }

    public BaseGizmo CreateGizmo(int section, GameObject obj,int subsection=0){return reader.CreateGizmo(section, obj,subsection);}
    
    static List<string> gizTypes = new(){"GizObstacle","GizBuildit","GizForce","blowup","GizmoPickup","Lever","Spinner","MiniCut",
    "Tube","ZipUp","GizTurret","BombGenerator","Panel","HatMachine","PushBlocks","Torp Machine","ShadowEditor","GizObstacleChild",
        "GizBuilditChild","GizForceChild","blowupChild"};
    public static int GetGizType(string typeName){return gizTypes.IndexOf(typeName);}
}
