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

}
