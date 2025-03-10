using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NTBLReader : MonoBehaviour
{
    public static NTBLReader inst;

    public string[] NameTable;
    private int ReadLocation;
    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }

    public void ReadTable()
    {
        byte[] bytes = GameManager.gm.gscBytes;
        int headIndex = 0;
        for(int i=0; i < bytes.Length; i++) //Locate the header
        {
            if (0x4E == bytes[i] && 0x54 == bytes[i + 1] && 0x42 == bytes[i+2] && 0x4C == bytes[i+3])
            {
                headIndex = i;
                break;
            }
        }
        ReadLocation = headIndex+4;
        int n1 = TypeConverter.ReadInt32(bytes, ref ReadLocation); //Read first 2 ints
        int n2 = TypeConverter.ReadInt32(bytes, ref ReadLocation);
        bool nextValid = true;
        List<string> lst = new();
        while (nextValid) //Read name table
        {
            string str = "";
            byte v = TypeConverter.ReadInt8(bytes, ref ReadLocation);
            if (v == 0) break;
            while (v != 0)
            {
                str += (char)v;
                v = TypeConverter.ReadInt8(bytes, ref ReadLocation);
            }
            lst.Add(str);
        }
        NameTable = lst.ToArray();
    }
}
