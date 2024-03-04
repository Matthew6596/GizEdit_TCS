using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildScript : MonoBehaviour
{
    public string parentName;
    GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name.Contains("(Clone)")) gameObject.name = gameObject.name[..^7];
        StartCoroutine(findParent());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator findParent()
    {
        yield return new WaitForSeconds(1);
        parent = GameObject.Find(parentName);
        transform.SetParent(parent.transform);
    }
}
