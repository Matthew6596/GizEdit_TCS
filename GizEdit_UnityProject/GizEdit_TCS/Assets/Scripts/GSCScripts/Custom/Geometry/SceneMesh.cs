using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SceneMesh : MoveObj
{
    public Texture txtr;

    MeshRenderer mRender;
    MeshFilter mFilter;
    MeshCollider mCollider;

    public MeshInfo preloadInfo;
    private Material preloadMat;

    public int fileIndex;

    // Start is called before the first frame update
    void Start()
    {
        applyGizOffset = true;
        mRender = gameObject.AddComponent<MeshRenderer>();
        mFilter = gameObject.AddComponent<MeshFilter>();
        mCollider = gameObject.AddComponent<MeshCollider>();
        SelectObj select = gameObject.AddComponent<SelectObj>();

        /*
        Load(txtr, new Vector3[] { 
            new(0, 0, 0), new(1, 0, 0), new(0, 1, 0),new(1,1,0) }, //list of vertices
            new int[] { 2, 1, 0,2,3,1 }, //indices, index of a vertex, 3 per tri
            new Vector2[] { new(0, 0), new(1, 0), new(0, 1),new(1,1) }); //uvs align w/ vertices
        */

        select.OnSelect += (obj) => { ShowSelection(); };
        select.OnDeselect += (obj) => { HideSelection(); };
        //OnDeselect += (obj) => { HideSelection(); };
        //OnSelect += (obj) => { ShowSelection(); };
    }

    public void ShowSelection()
    {
        CreateSelection();
        //for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(true);
    }
    private void HideSelection()
    {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        //for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
    }

    public void PrepareLoad(MeshInfo preloadInfo)
    {
        this.preloadInfo = preloadInfo;
    }

    public void Load(Texture texture, Vector3[] vertices, int[] indices, Vector2[] uvs)
    {
        Mesh mesh = new();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices,MeshTopology.Triangles,0);
        if(uvs!=null&&uvs.Length>0) mesh.SetUVs(0, uvs);
        mFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
        mRender.material = preloadMat;
        //if (texture == null) texture = SceneLoader.inst.textures[Random.Range(0, SceneLoader.inst.textures.Count)];
        //SetTexture(texture, texture.name.Contains("DXT3")||texture.name.Contains("DXT5"));
        //CreateSelection();
    }

    public void SetTexture(Texture texture,bool isTransparent=false)
    {
        mRender.material = SceneLoader.inst.materials[Random.Range(0, SceneLoader.inst.materials.Count)];
        return;
        txtr = texture;
        mRender.material = (isTransparent)?MaterialExt.GetStandardTransparent(texture):MaterialExt.GetStandard(texture);
    }
    public void SetMaterial(Material material)
    {
        preloadMat = material;
    }
    public Texture GetTexture()
    {
        return mRender.material.mainTexture;
    }

    public void CreateSelection()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) Destroy(transform.GetChild(i).gameObject);
        foreach (Vector3 vert in mFilter.mesh.vertices)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.GetComponent<Renderer>().material = MaterialExt.GetUnlit(Color.red);
            obj.transform.localScale = Vector3.one*0.05f;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = vert;
            MoveObj moveObj = obj.AddComponent<MoveObj>();
            moveObj.OnSelect += (obj) =>
            {
                selectedVert = obj;
                for(int i=0; i<mFilter.mesh.vertexCount; i++)
                {
                    if (selectedVert.transform.localPosition == mFilter.mesh.vertices[i])
                    {
                        selectedVertIndex = i;
                        break;
                    }
                }
            };
            moveObj.OnDeselect += (obj) => { selectedVert = null; };
            //obj.SetActive(false);
        }
    }

    private GameObject selectedVert;
    private int selectedVertIndex;
    private void Update()
    {
        if (selectedVert != null)
        {
            Vector3[] verts = mFilter.mesh.vertices;
            verts[selectedVertIndex] = selectedVert.transform.position-transform.position;
            mFilter.mesh.vertices = verts;
        }
    }
}

public static class MaterialExt
{
    public static Material GetUnlit() { return new(Shader.Find("Unlit/Texture")); }
    public static Material GetStandard() { return new(Shader.Find("Standard")); }
    public static Material GetUnlit(Color col) {return new(Shader.Find("Unlit/Color")) { color = col };}
    public static Material GetStandard(Color col) { return new(Shader.Find("Standard")) { color=col}; }
    public static Material GetUnlit(Texture texture) { return new(Shader.Find("Unlit/Texture")) { mainTexture=texture}; }
    public static Material GetStandard(Texture texture) { return new(Shader.Find("Standard")) { mainTexture=texture}; }
    public static Material GetStandardTransparent(Texture texture) { return new(GameManager.gm.transparentTextureMaterial) { mainTexture=texture}; }
    public static Material GetStandard(Texture mainTexture, Texture normalTexture, Texture specularTexture, Color color)
    {
        Material mat = new(Shader.Find("Standard")) { mainTexture = mainTexture,color=color };
        mat.SetTexture("_BumpMap", normalTexture);
        return mat;
    }
    public static Material GetStandardTransparent(Texture mainTexture, Texture normalTexture, Texture specularTexture, Color color)
    {
        Material mat = new(GameManager.gm.transparentTextureMaterial) { mainTexture = mainTexture, color = color };
        mat.SetTexture("_BumpMap", normalTexture);
        return mat;
    }
}