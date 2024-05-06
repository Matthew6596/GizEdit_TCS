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
    public string fhex="";
    [NonSerialized]
    public GameObject player;

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

    //basically substring, shorter to type in FileParser
    public uint FSliceInt8(int start){return TypeConverter.HexToInt8(fhex.Substring(B(start), B(1)));}
    public uint FSliceInt16(int start){return TypeConverter.HexToInt16(fhex.Substring(B(start), B(2)));}
    public uint FSliceInt32(int start){return TypeConverter.HexToInt32(fhex.Substring(B(start), B(4)));}
    public float FSliceFloat32(int start){return TypeConverter.HexToFloat32(fhex.Substring(B(start), B(4)));}
    public string FSliceString(int start, int length){return TypeConverter.HexToString(fhex.Substring(B(start), B(length)));}
    public uint FSliceInt8(uint start) { return TypeConverter.HexToInt8(fhex.Substring(B((int)start), B(1))); }
    public uint FSliceInt16(uint start) { return TypeConverter.HexToInt16(fhex.Substring(B((int)start), B(2))); }
    public uint FSliceInt32(uint start) { return TypeConverter.HexToInt32(fhex.Substring(B((int)start), B(4))); }
    public float FSliceFloat32(uint start) { return TypeConverter.HexToFloat32(fhex.Substring(B((int)start), B(4))); }
    public string FSliceString(uint start, int length) { return TypeConverter.HexToString(fhex.Substring(B((int)start), B(length))); }
    public string FSliceString(int start, uint length) { return TypeConverter.HexToString(fhex.Substring(B(start), B((int)length))); }
    public string FSliceString(uint start, uint length) { return TypeConverter.HexToString(fhex.Substring(B((int)start), B((int)length))); }

    static int B(int _bytes)
    {
        return (_bytes * 3);
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
