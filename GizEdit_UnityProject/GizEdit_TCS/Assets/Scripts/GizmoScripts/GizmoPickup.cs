using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoPickup : MonoBehaviour
{
    //PICKUP PROPERTIES
    [Header("Pickup Properties")]
    public string pickupName="";
    public string pickupType="s"; //default to silver stud

    //PRIVATE PROPERTIES
    MeshRenderer mrender;
    MeshFilter mfilter;
    MeshCollider mcollider;

    static public string pickupTypes = "sgbpmcuhrt";
    static public string[] pickupTypeNames = { "Silver Stud", "Gold Stud", "Blue Stud", "Purple Stud",
        "Minikit", "Challenge Minikit", "Power Up", "Heart", "Red Brick", "Torpedo" };

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
        Transform p = GameObject.Find("pickups").transform;
        transform.parent = p;
        mrender.material = setMaterial();
    }

    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(Vector3.one*0.1f); //temp
        int m = pickupTypes.IndexOf(pickupType);
    }
    Material setMaterial()
    {
        int m = pickupTypes.IndexOf(pickupType);
        return transform.parent.GetComponent<MeshRenderer>().materials[m];
    }
}