using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using SFB;

public class GameManager : MonoBehaviour
{
    //Persist through scenes
    public static GameObject instance;
    public static GameManager gmInstance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = gameObject;
            gmInstance = this;
            DontDestroyOnLoad(instance);
        }
    }

    //PUBLIC VARS
    [NonSerialized]
    public GameObject player;
    [NonSerialized]
    public byte[] bytes;

    //CHANGE FOR MAC / LINUX
    public IStandaloneFileBrowser FileBrowser = new StandaloneFileBrowserWindows();

    public GameObject[] propPrefabs;
    public GameObject propertyPanel;
    public Transform propertyPanelContent;

    public GameObject defaultPopup;

    //PRIVATE VARS
    enum DType { Int8, Int16, Int32, Float32, String };

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }


    //NEW FILE READ METHODS
    public static byte ReadInt8() { GizmosReader.reader.ReadLocation++; return gmInstance.bytes[GizmosReader.reader.ReadLocation - 1]; }
    public static short ReadInt16() 
    { 
        GizmosReader.reader.ReadLocation += 2; 
        return BitConverter.ToInt16(gmInstance.bytes, GizmosReader.reader.ReadLocation - 2); 
    }
    public static int ReadInt32() 
    { 
        GizmosReader.reader.ReadLocation += 4; 
        return BitConverter.ToInt32(gmInstance.bytes, GizmosReader.reader.ReadLocation - 4);
    }
    public static float ReadFloat() 
    { 
        GizmosReader.reader.ReadLocation += 4; 
        return BitConverter.ToSingle(gmInstance.bytes, GizmosReader.reader.ReadLocation - 4); 
    }
    public static string ReadString()
    {
        byte len = gmInstance.bytes[GizmosReader.reader.ReadLocation]; GizmosReader.reader.ReadLocation++;
        string ret = "";
        for(int i=0; i<len; i++)
        {
            ret += (char)gmInstance.bytes[GizmosReader.reader.ReadLocation];
            GizmosReader.reader.ReadLocation++;
        }
        return ret;
    }
    public static string ReadFixedString(int len)
    {
        string ret = "";
        for (int i = 0; i < len; i++)
        {
            ret += (char)gmInstance.bytes[GizmosReader.reader.ReadLocation];
            GizmosReader.reader.ReadLocation++;
        }
        return ret;
    }
    public static Vector3 ReadVec3() { return new(ReadFloat(), ReadFloat(), ReadFloat()); }
    public static byte[] ReadSlice(int len)
    {
        List<byte> ret = new();
        for (int i = 0; i < len; i++, GizmosReader.reader.ReadLocation++)
            ret.Add(gmInstance.bytes[GizmosReader.reader.ReadLocation]);
        return ret.ToArray();
    }

    public void DelayAction(Action action, float secs=0)
    {
        StartCoroutine(delayA(action, secs));
    }
    IEnumerator delayA(Action act, float secs)
    {
        yield return new WaitForSeconds(secs);
        act.Invoke();
    }
}
