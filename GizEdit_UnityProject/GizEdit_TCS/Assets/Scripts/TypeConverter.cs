using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TypeConverter : MonoBehaviour
{
    static public float Int16AngleToFloat(short angle)
    {
        return ((float)angle * 360f) / 65536f;
    }
    static public short FloatToInt16Angle(float angle)
    {
        return (short)(angle * 65536 / 360);
    }

    public static bool ByteSliceEqual(byte[] s1, byte[] s2)
    {
        if (s1.Length != s2.Length) return false;
        for(int i=0; i<s1.Length; i++) if (s1[i] != s2[i]) return false;
        return true;
    }
    public static bool ByteSliceEqual(byte[] s1, byte[] byteArr, int byteArrInd)
    {
        for (int i = 0; i < s1.Length; i++, byteArrInd++) if (s1[i] != byteArr[byteArrInd]) return false;
        return true;
    }

    public static byte ReadInt8(byte[] bytes, ref int readLocation) { readLocation++; return bytes[readLocation - 1]; }
    public static short ReadInt16(byte[] bytes, ref int readLocation)
    {
        readLocation += 2;
        return BitConverter.ToInt16(bytes, readLocation - 2);
    }
    public static int ReadInt32(byte[] bytes, ref int readLocation)
    {
        readLocation += 4;
        return BitConverter.ToInt32(bytes, readLocation - 4);
    }
    public static float ReadFloat(byte[] bytes, ref int readLocation)
    {
        readLocation += 4;
        return BitConverter.ToSingle(bytes, readLocation - 4);
    }
    public static string ReadString(byte[] bytes, ref int readLocation)
    {
        byte len = bytes[readLocation]; readLocation++;
        string ret = "";
        for (int i = 0; i < len; i++)
        {
            ret += (char)bytes[readLocation];
            readLocation++;
        }
        return ret;
    }
    public static string ReadFixedString(byte[] bytes, ref int readLocation, int len)
    {
        string ret = "";
        for (int i = 0; i < len; i++)
        {
            ret += (char)bytes[readLocation];
            readLocation++;
        }
        return ret;
    }
    public static Vector3 ReadVec3(byte[] bytes, ref int readLocation) { return new(ReadFloat(bytes,ref readLocation), ReadFloat(bytes, ref readLocation), ReadFloat(bytes, ref readLocation)); }
    public static byte[] ReadSlice(byte[] bytes, ref int readLocation, int len)
    {
        List<byte> ret = new();
        for (int i = 0; i < len; i++, readLocation++)
            ret.Add(bytes[readLocation]);
        return ret.ToArray();
    }
}
