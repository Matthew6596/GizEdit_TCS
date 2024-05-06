using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCSGizmosWriter : IGizmosWriter
{
    public string[] headerHex { get;set; }
    public TCSGizmosWriter()
    {
        headerHex = new string[]{
            "0B 00 00 00 47 69 7A 4F 62 73 74 61 63 6C 65 ", "0A 00 00 00 47 69 7A 42 75 69 6C 64 69 74 ",
        "08 00 00 00 47 69 7A 46 6F 72 63 65 ", "06 00 00 00 62 6C 6F 77 75 70 ", "0B 00 00 00 47 69 7A 6D 6F 50 69 63 6B 75 70 ",
        "05 00 00 00 4C 65 76 65 72 ","07 00 00 00 53 70 69 6E 6E 65 72 ","07 00 00 00 4D 69 6E 69 43 75 74 ",
        "04 00 00 00 54 75 62 65 ","05 00 00 00 5A 69 70 55 70 ","09 00 00 00 47 69 7A 54 75 72 72 65 74 ",
        "0D 00 00 00 42 6F 6D 62 47 65 6E 65 72 61 74 6F 72 ","05 00 00 00 50 61 6E 65 6C ","0A 00 00 00 48 61 74 4D 61 63 68 69 6E 65 ",
        "0A 00 00 00 50 75 73 68 42 6C 6F 63 6B 73 ","0C 00 00 00 54 6F 72 70 20 4D 61 63 68 69 6E 65 ",
        "0C 00 00 00 53 68 61 64 6F 77 45 64 69 74 6F 72 ", "07 00 00 00 47 72 61 70 70 6C 65 ",
        "04 00 00 00 50 6C 75 67 ","06 00 00 00 54 65 63 68 6E 6F "
        };
    }
    public string GetExtraHeaderStuff(int gizSection, int[] numOfEachGiz)
    {
        string ret = "";
        string helper1 = "";
        switch (gizSection)
        {
            //Obstacle
            case 0: return "";
            //Buildit
            case 1: return "09 " + TypeConverter.Int16ToHex((uint)numOfEachGiz[1]);
            //Force
            case 2:
                helper1 = TypeConverter.Int16ToHex((uint)numOfEachGiz[2]);
                return "10 " + helper1;
            //Blowup
            case 3: return "";
            //Pickup
            case 4:
                helper1 = GizmosReader.reader.headerData[4];
                if (helper1.Length > 0)
                {
                    if (helper1.Length > 1) ret = "07 00 00 00 " + TypeConverter.Int32ToHex((uint)numOfEachGiz[4]) + "01 00 00 00 " + helper1;
                    else { ret = "0" + helper1 + " 00 00 00 " + TypeConverter.Int32ToHex((uint)numOfEachGiz[4]) + "01 00 00 00 "; }
                }
                else
                {
                    ret = "07 00 00 00 " + TypeConverter.Int32ToHex((uint)numOfEachGiz[4]) + "01 00 00 00 00 00 20 41 00 00 80 3F ";
                }
                return ret;
            //Lever
            case 5: return "";
            //Spinner
            case 6: return "";
            //Minicut
            case 7: return "";
            //Tube
            case 8: return "";
            //ZipUp
            case 9: return "";
            //Turret
            case 10: return "";
            //BombGenerator
            case 11: return "";
            //Panel
            case 12: return "";
            //HatMachine
            case 13: return "";
            //PushBlocks
            case 14: return "";
            //TorpMachine
            case 15: return "";
            //ShadowEditor
            case 16: return "";
        }
        return "";
    }
}
