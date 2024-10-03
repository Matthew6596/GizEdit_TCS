using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCSGizmosWriter : IGizmosWriter
{
    public byte[][] headerBytes { get;set; }
    public TCSGizmosWriter()
    {
        headerBytes = new byte[][]
        {
            new byte[]{0x0B, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x4F, 0x62, 0x73, 0x74, 0x61, 0x63, 0x6C, 0x65},
            new byte[]{0x0A, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x42, 0x75, 0x69, 0x6C, 0x64, 0x69, 0x74},
            new byte[]{0x08, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x46, 0x6F, 0x72, 0x63, 0x65},
            new byte[]{0x06, 0x00, 0x00, 0x00, 0x62, 0x6C, 0x6F, 0x77, 0x75, 0x70},
            new byte[]{0x0B, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x6D, 0x6F, 0x50, 0x69, 0x63, 0x6B, 0x75, 0x70},
            new byte[]{0x05, 0x00, 0x00, 0x00, 0x4C, 0x65, 0x76, 0x65, 0x72},
            new byte[]{0x07, 0x00, 0x00, 0x00, 0x53, 0x70, 0x69, 0x6E, 0x6E, 0x65, 0x72},
            new byte[]{0x07, 0x00, 0x00, 0x00, 0x4D, 0x69, 0x6E, 0x69, 0x43, 0x75, 0x74},
            new byte[]{0x04, 0x00, 0x00, 0x00, 0x54, 0x75, 0x62, 0x65},
            new byte[]{0x05, 0x00, 0x00, 0x00, 0x5A, 0x69, 0x70, 0x55, 0x70},
            new byte[]{0x09, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x54, 0x75, 0x72, 0x72, 0x65, 0x74},
            new byte[]{0x0D, 0x00, 0x00, 0x00, 0x42, 0x6F, 0x6D, 0x62, 0x47, 0x65, 0x6E, 0x65, 0x72, 0x61, 0x74, 0x6F, 0x72},
            new byte[]{0x05, 0x00, 0x00, 0x00, 0x50, 0x61, 0x6E, 0x65, 0x6C},
            new byte[]{0x0A, 0x00, 0x00, 0x00, 0x48, 0x61, 0x74, 0x4D, 0x61, 0x63, 0x68, 0x69, 0x6E, 0x65},
            new byte[]{0x0A, 0x00, 0x00, 0x00, 0x50, 0x75, 0x73, 0x68, 0x42, 0x6C, 0x6F, 0x63, 0x6B, 0x73},
            new byte[]{0x0C, 0x00, 0x00, 0x00, 0x54, 0x6F, 0x72, 0x70, 0x20, 0x4D, 0x61, 0x63, 0x68, 0x69, 0x6E, 0x65},
            new byte[]{0x0C, 0x00, 0x00, 0x00, 0x53, 0x68, 0x61, 0x64, 0x6F, 0x77, 0x45, 0x64, 0x69, 0x74, 0x6F, 0x72},
            new byte[]{0x07, 0x00, 0x00, 0x00, 0x47, 0x72, 0x61, 0x70, 0x70, 0x6C, 0x65},
            new byte[]{0x04, 0x00, 0x00, 0x00, 0x50, 0x6C, 0x75, 0x67},
            new byte[]{0x06, 0x00, 0x00, 0x00, 0x54, 0x65, 0x63, 0x68, 0x6E, 0x6F}
        };
    }
    public byte[] GetExtraHeaderStuff(int gizSection, int[] numOfEachGiz)
    {
        List<byte> ret = new();
        switch (gizSection)
        {
            //Obstacle
            case 0: return new byte[] { };
            //Buildit
            case 1:
                ret.Add(9); ret.AddRange(BitConverter.GetBytes((short)numOfEachGiz[1]));
                return ret.ToArray();
            //Force
            case 2:
                ret.Add(0x10); ret.AddRange(BitConverter.GetBytes((short)numOfEachGiz[2]));
                return ret.ToArray();
            //Blowup
            case 3: return new byte[] { };
            //Pickup
            case 4:
                byte[] helper1 = GizmosReader.reader.headerData[4];
                if (helper1.Length > 0)
                {
                    if (helper1.Length > 1||(helper1[0] > 0x0F && helper1.Length == 1))
                    {
                        ret.AddRange(BitConverter.GetBytes(7));
                        ret.AddRange(BitConverter.GetBytes(numOfEachGiz[4]));
                        ret.AddRange(BitConverter.GetBytes(1));
                        ret.AddRange(helper1);
                    }
                    else
                    {
                        ret.AddRange(helper1); ret.Add(0); ret.Add(0); ret.Add(0);
                        ret.AddRange(BitConverter.GetBytes(numOfEachGiz[4]));
                        ret.AddRange(BitConverter.GetBytes(1));
                    }
                }
                else
                {
                    ret.AddRange(BitConverter.GetBytes(7));
                    ret.AddRange(BitConverter.GetBytes(numOfEachGiz[4]));
                    ret.AddRange(BitConverter.GetBytes(1));
                    ret.Add(0); ret.Add(0); ret.Add(0x20); ret.Add(0x41);
                    ret.AddRange(BitConverter.GetBytes(1f));
                    //ret = "07 00 00 00 " + TypeConverter.Int32ToHex((uint)numOfEachGiz[4]) + "01 00 00 00 00 00 20 41 00 00 80 3F ";
                }
                return ret.ToArray();
            //Lever
            case 5: return new byte[] { };
            //Spinner
            case 6: return new byte[] { };
            //Minicut
            case 7: return new byte[] { };
            //Tube
            case 8: return new byte[] { };
            //ZipUp
            case 9: return new byte[] { };
            //Turret
            case 10: return new byte[] { };
            //BombGenerator
            case 11: return new byte[] { };
            //Panel
            case 12:
                ret.AddRange(BitConverter.GetBytes(8)); ret.AddRange(BitConverter.GetBytes(numOfEachGiz[12]));
                return ret.ToArray();
            //HatMachine
            case 13: return new byte[] { };
            //PushBlocks
            case 14: return new byte[] { };
            //TorpMachine
            case 15: return new byte[] { };
            //ShadowEditor
            case 16: return new byte[] { };
        }
        return new byte[]{};
    }
}
