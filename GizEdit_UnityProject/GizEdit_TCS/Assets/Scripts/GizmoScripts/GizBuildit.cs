using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizBuildit : MonoBehaviour
{
    //PROPERTIES
    [Header("Force Properties")]
    public string referenceName = "buildit0";
    //pos
    public List<GameObject> childrenList = new();
    public float jumpPow = 1;
    public uint minStudValue = 0;
    public uint maxStudValue = 0;
    public string unknown1 = "01 64 ";
    public string unknown2 = "00 00 00 00 00 ";
    public float studPitch = 0;
    public float studYaw = 0;
    public Vector3 studSpawnPosition = Vector3.zero;
    public float studSpeed = 1.75f;
    public string unknown3 = "00 00 00 00 00 ";

    [Header("Other")]
    public Mesh[] typeMeshes;
    public Material[] typeMaterials;
    MeshRenderer mrender;
    MeshFilter mfilter;
    MeshCollider mcollider;

    public int selectedChild = 0;

    public static string[] endStateNames = { };


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
        Transform p = GameObject.Find("buildits").transform;
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
        return transform.parent.GetComponent<MeshRenderer>().material;
        //int m = pickupTypes.IndexOf(pickupType);
        //return transform.parent.GetComponent<MeshRenderer>().materials[m];
    }
}
