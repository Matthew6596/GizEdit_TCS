using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    //Persist through scenes
    public static GameObject instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = gameObject;
            DontDestroyOnLoad(instance);
        }
    }

    //PUBLIC VARS
    [NonSerialized]
    public string fhex="";
    [NonSerialized]
    public TypeConverter c;
    [NonSerialized]
    public int greyboxLvl;
    [NonSerialized]
    public GameObject player;

    //PRIVATE VARS
    enum DType { Int8, Int16, Int32, Float32, String };

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    //basically substring, shorter to type in FileParser
    public int FSliceInt8(int start){return TypeConverter.HexToInt8(fhex.Substring(start, B(1)));}
    public int FSliceInt16(int start){return TypeConverter.HexToInt16(fhex.Substring(start, B(2)));}
    public int FSliceInt32(int start){return TypeConverter.HexToInt32(fhex.Substring(start, B(4)));}
    public float FSliceFloat32(int start){return TypeConverter.HexToFloat32(fhex.Substring(start, B(4)));}
    public string FSliceString(int start, int length){return TypeConverter.HexToString(fhex.Substring(start, B(length)));}

    static int B(int _bytes)
    {
        return (_bytes * 3);
    }

}
