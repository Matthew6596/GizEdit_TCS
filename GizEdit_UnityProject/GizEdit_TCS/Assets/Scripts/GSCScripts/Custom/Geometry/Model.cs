using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public List<SceneMesh> meshes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(SceneMesh[] meshes)
    {
        foreach(SceneMesh mesh in meshes)
        {
            mesh.transform.SetParent(transform);
        }
    }
}
