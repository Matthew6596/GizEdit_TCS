using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCSGizmosReader : IGizmosReader
{

    public bool[] sectionReady { get; set; }
    public string[] headerData { get; set; }
    public int[] headerLengths { get; set; }
    public uint ReadLocation { get; set; }

    public GameManager gm;

    public TCSGizmosReader()
    {
        gm = GameManager.gmInstance;
        sectionReady = new bool[]{false,false,true, false, true, false, false, false, false, false, false, false, false, false, false, false, false};
        headerData = new string[17];
    }

    public IEnumerator ReadGizmos()
    {
        ReadLocation = 4; //skip 01 00 00 00

        for (int i = 0; i < 17; i++)
        {
            yield return null;
            ReadLocation += gm.FSliceInt32(ReadLocation) + 4; //skip header name
            uint sectionLength = gm.FSliceInt32(ReadLocation); ReadLocation += 4;
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
        uint numGizs = ReadExtraHeaderStuff(readSection);
        for (int i = 0; i < numGizs; i++)
        {
            GameObject obj = new();
            BaseGizmo giz = CreateGizmo(readSection, obj);
            giz.ReadFromHex();
        }
    }
    uint ReadExtraHeaderStuff(int gizSection)
    {
        uint n = 0;
        switch (gizSection)
        {
            //Obstacle
            case 0: return n;
            //Buildit
            case 1:
                n = gm.FSliceInt16(ReadLocation + 1);
                ReadLocation += 3;
                return n;
            //Force
            case 2:
                uint forceH = gm.FSliceInt8(ReadLocation); ReadLocation += 1;
                n = gm.FSliceInt16(ReadLocation); ReadLocation += 2;
                if (forceH != 16) Debug.LogWarning("Update to force header reader needed: " + forceH);
                return n;
            //Blowup
            case 3: return n;
            //Pickup
            case 4:
                uint pickupH = gm.FSliceInt32(ReadLocation); ReadLocation += 4;
                n = gm.FSliceInt32(ReadLocation); ReadLocation += 8;
                if (pickupH == 7) { headerData[4] = gm.fhex.Substring((int)ReadLocation * 3, 24); ReadLocation += 8; }
                else headerData[4] = pickupH.ToString();
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
            case 12: return n;
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
    public BaseGizmo CreateGizmo(int section, GameObject obj)
    {
        BaseGizmo g = null; //<<temp!!!
        switch (section)
        {
            //Obstacle
            case 0: return g;
            //Buildit
            case 1: return g;
            //Force
            case 2: return obj.AddComponent<GizForce>();
            //Blowup
            case 3: return g;
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
            case 12: return g;
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
            case 18: return g;
            //ForceChild
            case 19: return obj.AddComponent<GizForceChild>();
            //BlowupChild
            case 20: return g;
        }
        GameObject.Destroy(obj);
        return null;
    }
    public string getHeader(string titleName)
    {
        if (titleName == "GizObstacle") { return "0B 00 00 00 47 69 7A 4F 62 73 74 61 63 6C 65 "; }
        else if (titleName == "GizBuildit") { return "0A 00 00 00 47 69 7A 42 75 69 6C 64 69 74 "; }
        else if (titleName == "GizForce") { return "08 00 00 00 47 69 7A 46 6F 72 63 65 "; }
        else if (titleName == "blowup") { return "06 00 00 00 62 6C 6F 77 75 70 "; }
        else if (titleName == "GizmoPickup") { return "0B 00 00 00 47 69 7A 6D 6F 50 69 63 6B 75 70 "; }
        else if (titleName == "Lever") { return "05 00 00 00 4C 65 76 65 72 "; }
        else if (titleName == "Spinner") { return "07 00 00 00 53 70 69 6E 6E 65 72 "; }
        else if (titleName == "MiniCut") { return "07 00 00 00 4D 69 6E 69 43 75 74 "; }
        else if (titleName == "Tube") { return "04 00 00 00 54 75 62 65 "; }
        else if (titleName == "ZipUp") { return "05 00 00 00 5A 69 70 55 70 "; }
        else if (titleName == "GizTurret") { return "09 00 00 00 47 69 7A 54 75 72 72 65 74 "; }
        else if (titleName == "BombGenerator") { return "0D 00 00 00 42 6F 6D 62 47 65 6E 65 72 61 74 6F 72 "; }
        else if (titleName == "Panel") { return "05 00 00 00 50 61 6E 65 6C "; }
        else if (titleName == "HatMachine") { return "0A 00 00 00 48 61 74 4D 61 63 68 69 6E 65 "; }
        else if (titleName == "PushBlocks") { return "0A 00 00 00 50 75 73 68 42 6C 6F 63 6B 73 "; }
        else if (titleName == "Torp Machine") { return "0C 00 00 00 54 6F 72 70 20 4D 61 63 68 69 6E 65 "; }
        else if (titleName == "ShadowEditor") { return "0C 00 00 00 53 68 61 64 6F 77 45 64 69 74 6F 72 "; }
        else if (titleName == "Grapple") { return "07 00 00 00 47 72 61 70 70 6C 65 "; }
        else if (titleName == "Plug") { return "04 00 00 00 50 6C 75 67 "; }
        else if (titleName == "Techno") { return "06 00 00 00 54 65 63 68 6E 6F "; }
        else { return ""; }
    }
}
