using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCSGizmosReader : IGizmosReader
{

    public bool[] sectionReady { get; set; }
    public byte[][] headerData { get; set; }
    public int[] headerLengths { get; set; }
    public int ReadLocation { get; set; }

    public GameManager gm;

    public static byte[][] versions { get; private set; }

    public TCSGizmosReader()
    {
        gm = GameManager.gmInstance;
        sectionReady = new bool[]{false,true,true, false, true, false, false, false, false, false, false, false, true, false, false, false, false};
        headerData = new byte[17][];
        versions = new byte[17][];
    }

    public IEnumerator ReadGizmos()
    {
        ReadLocation = 4; //skip 01 00 00 00

        for (int i = 0; i < 17; i++)
        {
            yield return null;
            int headLen = GameManager.ReadInt32();
            ReadLocation += headLen; //skip header name
            int sectionLength = GameManager.ReadInt32();
            if (sectionReady[i])
            {
                ReadGizmoSection(i);
            }
            else
                ReadLocation += sectionLength;

            //progress bar
        }
    }
    void ReadGizmoSection(int readSection)
    {
        List<int> numGizs = ReadExtraHeaderStuff(readSection);
        for (int j = 0; j < numGizs.Count; j++)
        {
            for (int i = 0; i < numGizs[j]; i++)
            {
                GameObject obj = new();
                BaseGizmo giz = CreateGizmo(readSection, obj,j);
                giz.FromBin();
            }
        }
    }
    List<int> ReadExtraHeaderStuff(int gizSection)
    {
        List<int> n = new();
        switch (gizSection)
        {
            //Obstacle
            case 0: return n;
            //Buildit
            case 1:
                ReadLocation++;
                n.Add(GameManager.ReadInt16());
                return n;
            //Force
            case 2:
                byte forceH = GameManager.ReadInt8();
                versions[2] = new byte[] { forceH };
                n.Add(GameManager.ReadInt16());
                if (forceH != 16) Debug.LogWarning("Update to force header reader needed: " + forceH);
                return n;
            //Blowup
            case 3: 
                int blowupVersion = GameManager.ReadInt32();
                n.Add(GameManager.ReadInt32());
                n.Add(GameManager.ReadInt32());
                return n;
            //Pickup
            case 4:
                int pickupVersion = GameManager.ReadInt32();
                n.Add(GameManager.ReadInt32()); ReadLocation += 4;
                if (pickupVersion == 7) { headerData[4] = GameManager.ReadSlice(8); }
                else headerData[4] = BitConverter.GetBytes(pickupVersion);
                return n;
            //Lever
            case 5: return n;
            //Spinner
            case 6: return n;
            //Minicut
            case 7: return n;
            //Tube
            case 8: return n;
            //ZipUp
            case 9: return n;
            //Turret
            case 10: return n;
            //BombGenerator
            case 11: return n;
            //Panel
            case 12:
                int panelVersion = GameManager.ReadInt32();
                n.Add(GameManager.ReadInt32());
                return n;
            //HatMachine
            case 13: return n;
            //PushBlocks
            case 14: return n;
            //TorpMachine
            case 15: return n;
            //ShadowEditor
            case 16: return n;
        }
        return n;
    }
    public BaseGizmo CreateGizmo(int section, GameObject obj, int subsection)
    {
        BaseGizmo g = null; //<<temp!!!
        switch (section)
        {
            //Obstacle
            case 0: return g;
            //Buildit
            case 1: return obj.AddComponent<GizBuildit>();
            //Force
            case 2: return obj.AddComponent<GizForce>();
            //Blowup
            case 3: 
                if(subsection==0)return obj.AddComponent<blowupFx>();
                else return obj.AddComponent<blowupGiz>();
            //Pickup
            case 4: return obj.AddComponent<GizmoPickup>();
            //Lever
            case 5: return g;
            //Spinner
            case 6: return g;
            //Minicut
            case 7: return g;
            //Tube
            case 8: return g;
            //ZipUp
            case 9: return g;
            //Turret
            case 10: return g;
            //BombGenerator
            case 11: return g;
            //Panel
            case 12: return obj.AddComponent<Panel>();
            //HatMachine
            case 13: return g;
            //PushBlocks
            case 14: return g;
            //TorpMachine
            case 15: return g;
            //ShadowEditor
            case 16: return g;
            //ObstacleChild
            case 17: return g;
            //BuilditChild
            case 18: return obj.AddComponent<GizBuilditChild>();
            //ForceChild
            case 19: return obj.AddComponent<GizForceChild>();
        }
        GameObject.Destroy(obj);
        return null;
    }
    public byte[] getHeader(string titleName) => titleName switch
    {
        "GizObstacle" => new byte[] { 0x0B, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x4F, 0x62, 0x73, 0x74, 0x61, 0x63, 0x6C, 0x65 },
        "GizBuildit" => new byte[] { 0x0A, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x42, 0x75, 0x69, 0x6C, 0x64, 0x69, 0x74 },
        "GizForce" => new byte[] { 0x08, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x46, 0x6F, 0x72, 0x63, 0x65 },
        "blowup" => new byte[] { 0x06, 0x00, 0x00, 0x00, 0x62, 0x6C, 0x6F, 0x77, 0x75, 0x70 },
        "GizmoPickup" => new byte[] { 0x0B, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x6D, 0x6F, 0x50, 0x69, 0x63, 0x6B, 0x75, 0x70 },
        "Lever" => new byte[] { 0x05, 0x00, 0x00, 0x00, 0x4C, 0x65, 0x76, 0x65, 0x72 },
        "Spinner" => new byte[] { 0x07, 0x00, 0x00, 0x00, 0x53, 0x70, 0x69, 0x6E, 0x6E, 0x65, 0x72 },
        "MiniCut" => new byte[] { 0x07, 0x00, 0x00, 0x00, 0x4D, 0x69, 0x6E, 0x69, 0x43, 0x75, 0x74 },
        "Tube" => new byte[] { 0x04, 0x00, 0x00, 0x00, 0x54, 0x75, 0x62, 0x65 },
        "ZipUp" => new byte[] { 0x05, 0x00, 0x00, 0x00, 0x5A, 0x69, 0x70, 0x55, 0x70 },
        "GizTurret" => new byte[] { 0x09, 0x00, 0x00, 0x00, 0x47, 0x69, 0x7A, 0x54, 0x75, 0x72, 0x72, 0x65, 0x74 },
        "BombGenerator" => new byte[] { 0x0D, 0x00, 0x00, 0x00, 0x42, 0x6F, 0x6D, 0x62, 0x47, 0x65, 0x6E, 0x65, 0x72, 0x61, 0x74, 0x6F, 0x72 },
        "Panel" => new byte[] { 0x05, 0x00, 0x00, 0x00, 0x50, 0x61, 0x6E, 0x65, 0x6C },
        "HatMachine" => new byte[] { 0x0A, 0x00, 0x00, 0x00, 0x48, 0x61, 0x74, 0x4D, 0x61, 0x63, 0x68, 0x69, 0x6E, 0x65 },
        "PushBlocks" => new byte[] { 0x0A, 0x00, 0x00, 0x00, 0x50, 0x75, 0x73, 0x68, 0x42, 0x6C, 0x6F, 0x63, 0x6B, 0x73 },
        "Torp Machine" => new byte[] { 0x0C, 0x00, 0x00, 0x00, 0x54, 0x6F, 0x72, 0x70, 0x20, 0x4D, 0x61, 0x63, 0x68, 0x69, 0x6E, 0x65 },
        "ShadowEditor" => new byte[] { 0x0C, 0x00, 0x00, 0x00, 0x53, 0x68, 0x61, 0x64, 0x6F, 0x77, 0x45, 0x64, 0x69, 0x74, 0x6F, 0x72 },
        "Grapple" => new byte[] { 0x07, 0x00, 0x00, 0x00, 0x47, 0x72, 0x61, 0x70, 0x70, 0x6C, 0x65 },
        "Plug" => new byte[] { 0x04, 0x00, 0x00, 0x00, 0x50, 0x6C, 0x75, 0x67 },
        "Techno" => new byte[] { 0x06, 0x00, 0x00, 0x00, 0x54, 0x65, 0x63, 0x68, 0x6E, 0x6F },
        _ => new byte[] { }
    };
}
