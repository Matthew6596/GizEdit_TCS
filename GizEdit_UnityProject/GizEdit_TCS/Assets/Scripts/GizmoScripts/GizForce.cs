using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizForce : MonoBehaviour
{
    //PICKUP PROPERTIES
    [Header("Force Properties")]
    public string[] sfxs;
    public string referenceName="force0";
    public float resetTime = 0f;
    public float shakeTime = 0f;
    public float range = 0f;
    public bool darkSide = false;
    public int endState = 0;
    //unknown1
    public List<GameObject> childrenList = new();
    public float forceSpeed = 1f;
    public float resetSpeed = 1f;
    //unknown2
    public float effectScale = 1f;
    //unknown3
    public int minStudValue = 0;
    public int maxStudValue = 0;
    //unknown4
    public Vector3 studSpawnPosition = Vector3.zero;
    public float studSpeed = 1.75f;
    
    [Header("Other")]
    public Mesh[] typeMeshes;
    public Material[] typeMaterials;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindParent());
        tag = "objectGiz";
    }

    IEnumerator FindParent()
    {
        yield return null;
        Transform p = GameObject.Find("forces").transform;
        transform.parent = p;
    }
}
