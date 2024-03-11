using Palmmedia.ReportGenerator.Core.Parser.Filtering;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GizForce : MonoBehaviour
{
    //PICKUP PROPERTIES
    [Header("Force Properties")]
    public string referenceName="force0";
    public float resetTime = 0f;
    public float shakeTime = 0f;
    public float range = 0f;
    public bool darkSide = false;
    public uint endState = 0;
    public string unknown1 = "00 00 00 ";
    public string toggleForceUnknown = "FF ";
    public string unknown2 = "00 00 03 ";
    public List<GameObject> childrenList = new();
    public float forceSpeed = 1f;
    public float resetSpeed = 1f;
    public string unknown3 = "00 00 00 00 ";
    public float effectScale = 1f;
    public string unknown4 = "00 00 00 00 ";
    public uint minStudValue = 0;
    public uint maxStudValue = 0;
    public string unknown5 = "";
    public float studAngle = 0;
    public Vector3 studSpawnPosition = Vector3.zero;
    public float studSpeed = 1.75f;
    public string duringSfx = "";
    public string endSfx = "";
    public string unknown6 = "";
    
    [Header("Other")]
    public Mesh[] typeMeshes;
    public Material[] typeMaterials;
    MeshRenderer mrender;
    MeshFilter mfilter;
    MeshCollider mcollider;

    public int selectedChild = 0;

    public static string[] endStateNames = { };


    public void SetDarkSide(bool _b)
    {
        darkSide = _b;
        mrender.material = setMaterial();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindParent());
        tag = "objectGiz";
        mrender = gameObject.AddComponent<MeshRenderer>();
        mfilter = gameObject.AddComponent<MeshFilter>();
        mcollider = gameObject.AddComponent<MeshCollider>();
        mfilter.mesh = setMesh();
        mcollider.sharedMesh = mfilter.mesh;
    }

    IEnumerator FindParent()
    {
        yield return null;
        Transform p = GameObject.Find("forces").transform;
        transform.parent = p;
        mrender.material = setMaterial();
    }

    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(Vector3.one * 0.15f); //temp
        //int m = pickupTypes.IndexOf(pickupType);
    }
    Material setMaterial()
    {
        int m = darkSide?1 : 0;
        return transform.parent.GetComponent<MeshRenderer>().materials[m];
        //int m = pickupTypes.IndexOf(pickupType);
        //return transform.parent.GetComponent<MeshRenderer>().materials[m];
    }
}
