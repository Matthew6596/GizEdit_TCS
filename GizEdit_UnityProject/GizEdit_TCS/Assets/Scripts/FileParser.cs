using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices.ComTypes;

public class FileParser : MonoBehaviour
{
    //PUBLIC VARS
    [Header("General")]
    public Transform[] gizmoParents;
    public GameObject labelPrefab;
    public GameObject reticleTriggerPrefab;
    [Header("Pickup Prefabs")]
    public GameObject minikitPrefab;
    public GameObject bluekitPrefab;
    public GameObject heartPrefab;
    [Header("Panel Prefabs")]
    public GameObject astromechPrefab;
    public GameObject protocolPrefab;
    public GameObject bountyPrefab;
    public GameObject trooperPrefab;

    //PRIVATE VARS
    GameManager gm;
    SelectorScript ss;
    List<GameObject> gizObjList = new();

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ss = GameObject.Find("EditorManager").GetComponent<SelectorScript>();
    }
    void handleError()
    {
        /*"There was an error parsing your file, here is a quick trouble shoot:
         * 1. Your file not from TCS --> .GIZ files from games other than TCS may not be suitable for parsing.
         * 2. Your file is unmodified from TCS --> See [link] for a list .GIZ files, if you see your file on the list, download the version listed. Otherwise, fill out the missing file form on the site.
         * 3. Your file is modified from TCS --> You may have to redo some of your modifications. Follow the steps from option 2 or fill out this form: [link]
         * 4. Your file is a custom .GIZ --> You can either attempt to adjust your file's formatting, or fill out the form linked in option 3.
         * -NOTE-
         * At the moment, your file is unable to be read by this software. You will not be able to open this file until either the file is adjusted/reformatted or until I update the parser to address these issues."
        */
    }
    public void startLoadGizmos()
    {
        //readObstacles();
        readBuildits();
        readForces();
        //Read blowups
        readPickups();
        //Read lever
        //Read spinner
        //Read minicut
        //Read tube
        //Read zipup
        //Read turret
        //Read Bomb Generator
        //readPanels();
        //Read Hat machine
        //Read push blocks
        //Read torp machine
        //Read ShadowEditor
        //Read Grapple
        //Read plug
        //Read techno
    }
    /* NOTE ON OBJECT CREATION ORDER IN CODE
     * 1. SET TEMP LOOP OBJ TO PREFAB
     * 2. APPLY / ADD ATTRIBUTES
     * 3. INSTANTIATE
     */

    public int getPosAfter(string _val)
    {
        return gm.fhex.IndexOf(_val) + _val.Length;
    }
    public int getPosAfter(string _val, int startLocation)
    {
        return gm.fhex.IndexOf(_val,startLocation) + _val.Length;
    }

    static int B(int _bytes)
    {
        return (_bytes * 3);
    }
    static int B(uint _bytes)
    {
        return ((int)_bytes * 3);
    }

    Vector3 fixNegativeScale(Vector3 scl)
    {
        return new Vector3(Mathf.Abs(scl.x),Mathf.Abs(scl.y),Mathf.Abs(scl.z));
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //===============================================READING===============================================
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //==============================================OBSTACLES==============================================
    public void readObstacles()
    {
        int afterHead = getPosAfter(TypeConverter.getHeader("GizObstacle"));
        uint numBytes = gm.FSliceInt32(afterHead);
        //Header extra
        uint unknownNum = gm.FSliceInt8(afterHead+B(4));
        uint numObstacles = gm.FSliceInt16(afterHead+B(5));
        //
        int startData;
        if (unknownNum == 10 || unknownNum == 14) { startData = afterHead + B(7); }
        else { Debug.Log("ERROR: UNKNOWN OBSTACLE CASE: " + unknownNum); return; }

        int skipBytes=0;
        int closeBytes = 0;
        if (unknownNum==10){ skipBytes = B(28); closeBytes = B(23+8); }
        else if (unknownNum==14){ skipBytes = B(32); closeBytes = B(35); }

        int readLocation = startData;
        for (int rc=0; rc<numObstacles; rc++)
        {
            //Parsing obstacle data
            string obstacleName = gm.FSliceString(readLocation, 16);
            Vector3 trigTLPos;
            trigTLPos.x = gm.FSliceFloat32(readLocation + B(16));
            trigTLPos.y = gm.FSliceFloat32(readLocation + B(20));
            trigTLPos.z = gm.FSliceFloat32(readLocation + B(24));
            Vector3 trigBRPos;
            trigBRPos.x = gm.FSliceFloat32(readLocation + B(28));
            trigBRPos.y = gm.FSliceFloat32(readLocation + B(32));
            trigBRPos.z = gm.FSliceFloat32(readLocation + B(36));

            readLocation += skipBytes+B(40);
            //CHILDREN
            List<GameObject> childList = new List<GameObject>();
            uint numChildren = gm.FSliceInt8(readLocation + B(2));

            readLocation += B(3);
            for(int rc2=0; rc2<numChildren; rc2++)
            {
                //Parsing child data
                uint nameLength = gm.FSliceInt8(readLocation);
                string cName = gm.FSliceString(readLocation + B(1), nameLength);

                //preparing to instantiate child
                GameObject child = new GameObject(cName);
                child.name = rc2+"_"+cName;
                ChildScript _cs1 = child.AddComponent<ChildScript>();
                _cs1.parentName = rc + "_" + obstacleName;
                childList.Add(child);

                readLocation += B(1 + nameLength + 14);
            }

            //Instantiating obstacle data in GizView
            //Trigger
            Color trigCol = Color.green;
            trigCol.a = 0.25f;
            Vector3 trigScale = new Vector3(trigTLPos.x - trigBRPos.x, trigTLPos.y - trigBRPos.y, trigTLPos.z - trigBRPos.z);
            trigScale = fixNegativeScale(trigScale);
            Vector3 trigPos = new Vector3((trigBRPos.x+trigTLPos.x)/2,(trigBRPos.y+trigTLPos.y)/2,(trigBRPos.z+trigTLPos.z)/2);
            GameObject cPrefab;
            if (trigScale.x > 0.01f || trigScale.y > 0.01f || trigScale.z > 0.01f)
            {
                cPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cPrefab.transform.localScale = trigScale;
                cPrefab.transform.position = trigPos;
                cPrefab.AddComponent<BoxCollider>();
                cPrefab.GetComponent<Renderer>().material.color = trigCol;
                cPrefab.name = rc + "_" + obstacleName;
                ChildScript _cs2 = cPrefab.AddComponent<ChildScript>();
                _cs2.parentName = gizmoParents[0].gameObject.name;
            }
            else
            {
                cPrefab = labelPrefab;
                cPrefab.name = rc + "_" + obstacleName;
                cPrefab.transform.position = trigPos;
                ChildScript _cs2 = cPrefab.AddComponent<ChildScript>();
                _cs2.parentName = gizmoParents[0].gameObject.name;
                Instantiate(cPrefab);
            }

            //Instantiate children
            for (int rc2=0; rc2<numChildren; rc2++) { Instantiate(childList[rc2]); Destroy(childList[rc2]); }
            childList.Clear();

            readLocation += closeBytes;
            if (gm.FSliceInt8(readLocation) == 0) { readLocation += B(2); }
            else if (gm.FSliceInt8(readLocation) == 7) { readLocation += B(9); }
            else { Debug.Log("ERROR: UNKNOWN OBSTACLE CASE: " + gm.FSliceInt8(readLocation)); return; };
        }

    }
    //==============================================BUILDITS==============================================
    public void readBuildits()
    {
        int afterHead = getPosAfter(TypeConverter.getHeader("GizBuildit"));
        uint numBytes = gm.FSliceInt32(afterHead);
        uint unknown1 = gm.FSliceInt8(afterHead + B(4));
        uint numObjs = gm.FSliceInt16(afterHead + B(5));
        //Header extra
        //int numObjs = 0;
        //
        int startData = afterHead + B(7);
        int endLocation = B(numBytes) + afterHead;
        //

        int readLocation = startData;
        int rc = 0;
        while (readLocation < endLocation)
        { //numBytes driven loop

            //Create forceObj
            GameObject _f = new();
            GizBuildit _g = _f.AddComponent<GizBuildit>();
            _f.name = "buildit_" + rc;

            //Parse data
            _g.referenceName = gm.FSliceString(readLocation, 16);
            _f.transform.position = new Vector3(gm.FSliceFloat32(readLocation + B(16)), gm.FSliceFloat32(readLocation + B(20)), gm.FSliceFloat32(readLocation + B(24)));

            readLocation += B(29); //prev 28 bytes + 03 byte

            //Child data
            uint numBuildChildren = gm.FSliceInt8(readLocation);
            readLocation += B(1);
            for (int rc2 = 0; rc2 < numBuildChildren; rc2++)
            {
                //Create child
                GameObject _child = new();
                GizForceChild _gfc = _child.AddComponent<GizForceChild>();
                _child.name = "builditchild_" + rc2;

                //Parse child info
                uint childNameLength = gm.FSliceInt8(readLocation);
                _gfc.gizName = gm.FSliceString(readLocation + B(1), childNameLength);
                readLocation += B(childNameLength + 1);
                _gfc.unknown1 = gm.fhex.Substring(readLocation, B(4));
                _gfc.animateLength = gm.FSliceFloat32(readLocation + B(4));
                readLocation += B(8); //Unknown 4 bytes
                _gfc.isSelected = gm.FSliceInt8(readLocation) != 0;
                _gfc.unknown2 = gm.fhex.Substring(readLocation + B(1), B(3));
                readLocation += B(4); //Unknown 3 bytes

                //Assign child
                _gfc.gizParent = _g.transform;
                _g.childrenList.Add(_child);
            }

            //Parse closer data
            _g.jumpPow = gm.FSliceFloat32(readLocation);
            _g.minStudValue = gm.FSliceInt16(readLocation+B(4));
            _g.maxStudValue = gm.FSliceInt16(readLocation + B(6));
            _g.unknown1 = gm.fhex.Substring(readLocation + B(8), B(2));
            _g.unknown2 = gm.fhex.Substring(readLocation + B(10), B(5));
            _g.studPitch = TypeConverter.Int16AngleToFloat(gm.FSliceInt16(readLocation+B(15)));
            _g.studYaw = TypeConverter.Int16AngleToFloat(gm.FSliceInt16(readLocation+B(17)));
            readLocation += B(19);
            _g.studSpawnPosition = new Vector3(gm.FSliceFloat32(readLocation), gm.FSliceFloat32(readLocation + B(4)), gm.FSliceFloat32(readLocation + B(8)));
            _g.studSpeed = gm.FSliceFloat32(readLocation + B(12));
            _g.unknown3 = gm.fhex.Substring(readLocation + B(16), B(5));
            readLocation += B(21);
            rc++;
        }

    }
    //==============================================FORCES==============================================
    public void readForces()
    {
        int afterHead = getPosAfter(TypeConverter.getHeader("GizForce"));
        uint numBytes = gm.FSliceInt32(afterHead);
        uint unknown1 = gm.FSliceInt8(afterHead+B(4));
        uint unknown2 = gm.FSliceInt8(afterHead+B(5));
        //Header extra
        //int numObjs = 0;
        //
        int startData = afterHead + B(7);
        int endLocation = B(numBytes) + afterHead;
        //

        int readLocation = startData;
        int rc = 0;
        while (readLocation < endLocation) { //numBytes driven loop

            //Create forceObj
            GameObject _f = new();
            GizForce _g = _f.AddComponent<GizForce>();
            _f.name = "force_" + rc;

            //Parse data
            _g.referenceName = gm.FSliceString(readLocation, 16);
            _f.transform.position = new Vector3(gm.FSliceFloat32(readLocation + B(16)), gm.FSliceFloat32(readLocation + B(20)), gm.FSliceFloat32(readLocation + B(24)));
            _g.resetTime = gm.FSliceFloat32(readLocation + B(28));
            _g.shakeTime = gm.FSliceFloat32(readLocation + B(32));
            _g.range = gm.FSliceFloat32(readLocation + B(36));
            readLocation += B(40);
            _g.darkSide = gm.fhex[readLocation]=='1';
            _g.endState = uint.Parse(gm.fhex[readLocation + 1].ToString());
            _g.unknown1 = gm.fhex.Substring(readLocation + B(1), B(3));
            //toggle force
            _g.unknown2 = gm.fhex.Substring(readLocation + B(5), B(3));

            readLocation += B(8);

            //Child data
            uint numForceChildren = gm.FSliceInt8(readLocation);
            readLocation += B(1);
            for(int rc2=0; rc2<numForceChildren; rc2++)
            {
                //Create child
                GameObject _child = new();
                GizForceChild _gfc = _child.AddComponent<GizForceChild>();
                _child.name = "forcechild_" + rc2;

                //Parse child info
                uint childNameLength = gm.FSliceInt8(readLocation);
                _gfc.gizName = gm.FSliceString(readLocation + B(1), childNameLength);
                readLocation += B(childNameLength + 1);
                _gfc.unknown1 = gm.fhex.Substring(readLocation, B(4));
                _gfc.animateLength = gm.FSliceFloat32(readLocation + B(4));
                readLocation += B(8); //Unknown 4 bytes
                _gfc.isSelected = gm.FSliceInt8(readLocation) != 0;
                _gfc.unknown2 = gm.fhex.Substring(readLocation+B(1), B(5));
                readLocation += B(6); //Unknown 5 bytes

                //Assign child
                _gfc.gizParent = _g.transform;
                _g.childrenList.Add( _child );
            }

            //Parse closer data
            _g.forceSpeed = gm.FSliceFloat32(readLocation);
            _g.resetSpeed = gm.FSliceFloat32(readLocation+B(4));
            _g.unknown3 = gm.fhex.Substring(readLocation + B(8), B(4));
            readLocation += B(12); //Unknown 4 bytes
            _g.effectScale = gm.FSliceFloat32(readLocation);
            _g.unknown4 = gm.fhex.Substring(readLocation + B(4), B(4));
            readLocation += B(8); //Unknown 4 bytes

            uint unknownStringLength = gm.FSliceInt8(readLocation);
            string unknownString = gm.FSliceString(readLocation + B(1), unknownStringLength);
            _g.unknown5 = unknownString;
            readLocation += B(unknownStringLength + 1);

            _g.minStudValue = gm.FSliceInt16(readLocation);
            _g.maxStudValue = gm.FSliceInt16(readLocation+B(2));
            _g.studAngle = TypeConverter.Int16AngleToFloat(gm.FSliceInt16(readLocation+B(4)));
            readLocation += B(6);
            _g.studSpawnPosition = new Vector3(gm.FSliceFloat32(readLocation),gm.FSliceFloat32(readLocation+B(4)),gm.FSliceFloat32(readLocation+B(8)));
            _g.studSpeed = gm.FSliceFloat32(readLocation+B(12));
            readLocation += B(16);

            uint sfxNameLength = gm.FSliceInt8(readLocation);
            readLocation += B(1);
            _g.duringSfx = gm.FSliceString(readLocation, sfxNameLength);
            readLocation += B(sfxNameLength);
            sfxNameLength = gm.FSliceInt8(readLocation);
            readLocation += B(1);
            _g.endSfx = gm.FSliceString(readLocation, sfxNameLength);
            readLocation += B(sfxNameLength);
            sfxNameLength = gm.FSliceInt8(readLocation);
            readLocation += B(1);
            _g.unknown6 = gm.FSliceString(readLocation, sfxNameLength);
            readLocation += B(sfxNameLength);

            rc++;
        }

    }
    //==============================================PICKUPS==============================================
    public void readPickups()
    {
        int afterHead = getPosAfter(TypeConverter.getHeader("GizmoPickup"));
        uint numBytes = gm.FSliceInt32(afterHead);
        //Header extra
        uint unknownNum = gm.FSliceInt32(afterHead + B(4));
        uint numPickups = gm.FSliceInt32(afterHead + B(8));
        //
        int startData;
        if (unknownNum == 7) { startData = afterHead + B(24); }
        else if (unknownNum == 4) { startData = afterHead + B(16); }
        else { Debug.Log("ERROR: UNKNOWN PICKUP CASE: "+unknownNum); return; }

        int readLocation = startData;
        for (int rp = 0; rp < numPickups; rp++)
        {
            //Parsing pickup data
            string pickupName = gm.FSliceString(readLocation, 8);
            float pickupX = gm.FSliceFloat32(readLocation + B(8));
            float pickupY = gm.FSliceFloat32(readLocation + B(12));
            float pickupZ = gm.FSliceFloat32(readLocation + B(16));
            string pickupType = gm.FSliceString(readLocation+B(20), 1);
            uint spawnType = gm.FSliceInt8(readLocation+B(21));
            uint spawnGroup = gm.FSliceInt8(readLocation+B(22));
            char ptype = pickupType[0];
            if(ptype!='s'&&ptype != 'g' && ptype != 'b' && ptype != 'p' && ptype != 'm' && ptype != 'r' && ptype != 'u' && ptype != 'h' && ptype != 'c' && ptype != 't') { Debug.Log("UNKNOWN PICKUP TYPE: " + ptype); return; }
            if (spawnType != 0 && spawnType != 2) { Debug.Log("UNKNOWN PICKUP SPAWN TYPE: " + spawnType); return; }

            //Creating Pickup Object
            GameObject _pickupObj = new();
            GizmoPickup _props = _pickupObj.AddComponent<GizmoPickup>();
            _props.pickupType = ptype.ToString();
            _pickupObj.transform.position = new Vector3(pickupX, pickupY, pickupZ);
            _props.pickupName = pickupName;
            _pickupObj.name = "pickup_"+rp;
            _props.spawnType = spawnType;
            _props.spawnGroup = spawnGroup;

            readLocation += B(8 + 12 + 3);
        }
    }
    //==============================================PANELS==============================================
    void readPanels()
    {
        int afterHead = getPosAfter(TypeConverter.getHeader("Panel"));
        uint numBytes = gm.FSliceInt32(afterHead);
        //Header extra
        uint unknownNum = gm.FSliceInt32(afterHead+B(4));
        uint numPanels = gm.FSliceInt32(afterHead+B(8));
        //
        int startData;
        if (unknownNum == 8) { startData = afterHead + B(12); }
        else { Debug.Log("ERROR: UNKNOWN PANEL CASE: " + unknownNum); return; }

        int readLocation = startData;
        for (int rp = 0; rp < numPanels; rp++)
        {
            //Parsing panel data
            uint nameLength = gm.FSliceInt32(readLocation);
            string panelName = gm.FSliceString(readLocation+B(4),nameLength);
            readLocation += B(4 + nameLength);
            Vector3 panelPos = new(gm.FSliceFloat32(readLocation),gm.FSliceFloat32(readLocation+B(4)), gm.FSliceFloat32(readLocation + B(8)));
            readLocation += B(12);
            float rotation = TypeConverter.Int16AngleToFloat(gm.FSliceInt16(readLocation));
            uint ptype = gm.FSliceInt8(readLocation+B(2));
            readLocation += B(4);
            Vector3 trigPos = new(gm.FSliceFloat32(readLocation), gm.FSliceFloat32(readLocation + B(4)), gm.FSliceFloat32(readLocation + B(8)));
            float trigScale = gm.FSliceFloat32(readLocation + B(12));
            readLocation += B(16+5);

            //Preparing for Instantiation

            GameObject pPrefab;

            switch (ptype)
            {
                case (0): pPrefab = astromechPrefab; break;
                case (1): pPrefab = protocolPrefab; break;
                case (2): pPrefab = bountyPrefab; break;
                case (3): pPrefab = trooperPrefab; break;
                default: Debug.Log("UNKNOWN PANEL TYPE: " + ptype); return;
            }

            pPrefab.name = rp+"_"+panelName;
            pPrefab.transform.position = panelPos;
            pPrefab.transform.rotation = Quaternion.Euler(0,rotation,0);
            ChildScript _cs1 = pPrefab.AddComponent<ChildScript>();
            _cs1.parentName = gizmoParents[12].gameObject.name;

            Instantiate(pPrefab);

        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //==============================================COMPILING==============================================
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    string cHex = "";
    public string CompileGizmos()
    {
        cHex = "01 00 00 00 ";
        if (gm.fhex != "") //TEMPORARY, NEED TO BE ABLE TO MAKE BLANK .GIZ BEFORE REMOVE
        {
            compileObstacles();
            compileBuildit();
            compileForce();
            compileBlowup();
            compilePickups();
            compileLever();
            compileSpinner();
            compileMinicut();
            compileTube();
            compileZipup();
            compileTurret();
            compileBombGenerator();
            compilePanel();
            compileHatMachine();
            compilePushBlocks();
            compileTorpMachine();
            compileShadowEditor();
            compileGrapple();
            compilePlug();
            compileTechno();
        }
        cHex += "00 00 00 00 ";

        return cHex;
    }

    //==============================================OBSTACLE==============================================
    void compileObstacles()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[0]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(15), B(19 + numBytes));
        cHex += section;
    }
    //==============================================BUILDIT==============================================
    void compileBuildit()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[1]);
        uint numBytes = gm.FSliceInt32(afterHead);
        if (!ss.changedGizmoSections[2])
        {
            string section = gm.fhex.Substring(afterHead - B(14), B(18 + numBytes));
            cHex += section;
        }
        else
        {
            int readLocation = afterHead + B(7);
            string section = gm.fhex.Substring(afterHead - B(14), B(21));

            //Get all buildits
            GizBuildit[] _f = new GizBuildit[ss.gizParents.GetChild(2).childCount];
            for (int j = 0; j < _f.Length; j++) { _f[j] = ss.gizParents.GetChild(1).GetChild(j).GetComponent<GizBuildit>(); }
            //Read all buildits data
            foreach (GizBuildit f in _f)
            {
                section += TypeConverter.StringToHex(f.referenceName, 16);
                section+= TypeConverter.Float32ToHex(f.transform.position.x) + TypeConverter.Float32ToHex(f.transform.position.y) + TypeConverter.Float32ToHex(f.transform.position.z);
                //Children
                string childrenHex = TypeConverter.Int8ToHex((uint)f.childrenList.Count) + " ";
                foreach (GameObject _c in f.childrenList)
                {
                    GizForceChild fc = _c.GetComponent<GizForceChild>();
                    childrenHex += TypeConverter.VarStringToHex(fc.gizName);
                    childrenHex += fc.unknown1;
                    childrenHex += TypeConverter.Float32ToHex(fc.animateLength);
                    childrenHex += (fc.isSelected) ? "01 " : "00 ";
                    childrenHex += fc.unknown2;
                }
                //
                section += TypeConverter.Float32ToHex(f.jumpPow);
                section += TypeConverter.Int16ToHex(f.minStudValue);
                section += TypeConverter.Int16ToHex(f.maxStudValue);
                section += f.unknown1 + f.unknown2;
                section += TypeConverter.Int16ToHex(TypeConverter.FloatToInt16Angle(f.studPitch));
                section += TypeConverter.Int16ToHex(TypeConverter.FloatToInt16Angle(f.studYaw));
                section += TypeConverter.Float32ToHex(f.studSpawnPosition.x) + TypeConverter.Float32ToHex(f.studSpawnPosition.y) + TypeConverter.Float32ToHex(f.studSpawnPosition.z);
                section += TypeConverter.Float32ToHex(f.studSpeed);
                section += f.unknown3;

            }
            uint _nbytes = (uint)((section.Length / 3) - 16);
            uint _nobjs = (uint)_f.Length;
            section = TypeConverter.SetStringSlice(section, TypeConverter.Int32ToHex(_nbytes), B(14), B(4));
            section = TypeConverter.SetStringSlice(section, TypeConverter.Int16ToHex(_nobjs), B(19), B(2));
            cHex += section;
        }
    }
    //==============================================FORCE==============================================
    void compileForce()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[2]);
        uint numBytes = gm.FSliceInt32(afterHead);
        if (!ss.changedGizmoSections[2])
        {
            string section = gm.fhex.Substring(afterHead - B(12), B(16 + numBytes));
            cHex += section;
        }
        else
        {
            int readLocation = afterHead + B(7);
            string section = gm.fhex.Substring(afterHead - B(12), B(19));

            GizForce[] _f = new GizForce[ss.gizParents.GetChild(2).childCount];
            for(int j=0; j<_f.Length; j++){ _f[j] = ss.gizParents.GetChild(2).GetChild(j).GetComponent<GizForce>(); }
            foreach (GizForce f in _f)
            {
                string nameHex = TypeConverter.StringToHex(f.referenceName,16);
                string posHex = TypeConverter.Float32ToHex(f.transform.position.x) + TypeConverter.Float32ToHex(f.transform.position.y) + TypeConverter.Float32ToHex(f.transform.position.z);
                string resetTimeHex = TypeConverter.Float32ToHex(f.resetTime);
                string shakeTimeHex = TypeConverter.Float32ToHex(f.shakeTime);
                string rangeHex = TypeConverter.Float32ToHex(f.range);
                string darkPlusEndStateHex = ((f.darkSide) ? "1" : "0") + (TypeConverter.Int8ToHex(f.endState)[1]) +" ";
                string unknown1Hex = f.unknown1;
                string toggleForceHex = f.toggleForceUnknown;
                string unknown2Hex = f.unknown2;
                //Children
                string childrenHex=TypeConverter.Int8ToHex((uint)f.childrenList.Count)+" ";
                foreach (GameObject _c in f.childrenList)
                {
                    GizForceChild fc = _c.GetComponent<GizForceChild>();
                    childrenHex += TypeConverter.VarStringToHex(fc.gizName);
                    childrenHex += fc.unknown1;
                    childrenHex += TypeConverter.Float32ToHex(fc.animateLength);
                    childrenHex += (fc.isSelected)?"01 ":"00 ";
                    childrenHex += fc.unknown2;
                }
                //
                string forceSpdHex = TypeConverter.Float32ToHex(f.forceSpeed);
                string resetSpdHex = TypeConverter.Float32ToHex(f.resetSpeed);
                string unknown3Hex = f.unknown3;
                string forceScaleHex = TypeConverter.Float32ToHex(f.effectScale);
                string unknown4Hex = f.unknown4;
                string unknown5Hex = TypeConverter.VarStringToHex(f.unknown5);
                string studValueRangeHex = TypeConverter.Int16ToHex(f.minStudValue)+TypeConverter.Int16ToHex(f.maxStudValue);
                string studAngleHex = TypeConverter.Int16ToHex(TypeConverter.FloatToInt16Angle(f.studAngle));
                string studPosHex = TypeConverter.Float32ToHex(f.studSpawnPosition.x) + TypeConverter.Float32ToHex(f.studSpawnPosition.y) + TypeConverter.Float32ToHex(f.studSpawnPosition.z);
                string studSpdHex = TypeConverter.Float32ToHex(f.studSpeed);
                string sfxDuringHex = TypeConverter.VarStringToHex(f.duringSfx);
                string sfxEndHex = TypeConverter.VarStringToHex(f.endSfx);
                string unknown6Hex = TypeConverter.VarStringToHex(f.unknown6);

                section += nameHex+posHex+resetTimeHex+shakeTimeHex+rangeHex+darkPlusEndStateHex+unknown1Hex+toggleForceHex+
                    unknown2Hex+childrenHex+forceSpdHex+resetSpdHex+unknown3Hex+forceScaleHex+unknown4Hex+unknown5Hex+studValueRangeHex+
                    studAngleHex+studPosHex+studSpdHex+sfxDuringHex+sfxEndHex+unknown6Hex;
            }
            uint _nbytes = (uint)((section.Length / 3) - 16);
            uint _nobjs = (uint)_f.Length;
            section = TypeConverter.SetStringSlice(section, TypeConverter.Int32ToHex(_nbytes), B(12), B(4));
            section = TypeConverter.SetStringSlice(section, TypeConverter.Int16ToHex(_nobjs), B(17), B(2));
            cHex += section;
        }
        
    }
    //==============================================BLOWUP==============================================
    void compileBlowup()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[3]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(10), B(14 + numBytes));
        cHex += section;
    }
    //==============================================PICKUPS==============================================
    void compilePickups()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[4]);
        uint numBytes = gm.FSliceInt32(afterHead);
        uint unknownNum = gm.FSliceInt32(afterHead + B(4));
        uint numObjs = gm.FSliceInt32(afterHead + B(8));
        if (!ss.changedGizmoSections[4])
        {
            string pickupSection = gm.fhex.Substring(afterHead - B(15), B(19 + numBytes));
            cHex += pickupSection;
        }
        else
        {
            int readLocation=afterHead+B(4);
            int addAmt = 0;
            if (unknownNum == 7) { addAmt = B(20); }
            else if (unknownNum == 4) { addAmt = B(12); }
            readLocation += addAmt;
            string pickupSection = gm.fhex.Substring(afterHead-B(15),B(19)+addAmt);

            GizmoPickup[] _p = new GizmoPickup[ss.gizParents.GetChild(4).childCount];
            for (int j=0; j<_p.Length; j++) { _p[j] = ss.gizParents.GetChild(4).GetChild(j).GetComponent<GizmoPickup>(); }
            foreach (GizmoPickup p in _p)
            {
                string nameHex = TypeConverter.StringToHex(p.pickupName,8);
                string posHex = TypeConverter.Float32ToHex(p.transform.position.x);
                posHex += TypeConverter.Float32ToHex(p.transform.position.y) + TypeConverter.Float32ToHex(p.transform.position.z);
                string typeHex = TypeConverter.StringToHex(p.pickupType, 1);
                string spawnHex = TypeConverter.Int8ToHex(p.spawnType)+" ";
                string groupHex = TypeConverter.Int8ToHex(p.spawnGroup)+" ";
                pickupSection += nameHex + posHex + typeHex +spawnHex+groupHex;
            }
            uint _nbytes = (uint)((pickupSection.Length / 3) -19);
            uint _nobjs = (uint)_p.Length;
            pickupSection = TypeConverter.SetStringSlice(pickupSection, TypeConverter.Int32ToHex(_nbytes), B(15), B(4));
            pickupSection = TypeConverter.SetStringSlice(pickupSection, TypeConverter.Int32ToHex(_nobjs), B(23), B(4));
            cHex += pickupSection;
        }
    }
    //==============================================LEVER==============================================
    void compileLever()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[5]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(9), B(13 + numBytes));
        cHex += section;
    }
    //==============================================SPINNER==============================================
    void compileSpinner()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[6]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(11), B(15 + numBytes));
        cHex += section;
    }
    //==============================================MINICUT==============================================
    void compileMinicut()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[7]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(11), B(15 + numBytes));
        cHex += section;
    }
    //==============================================TUBE==============================================
    void compileTube()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[8]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(8), B(12 + numBytes));
        cHex += section;
    }
    //==============================================ZIPUP==============================================
    void compileZipup()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[9]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(9), B(13 + numBytes));
        cHex += section;
    }
    //==============================================TURRET==============================================
    void compileTurret()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[10]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(13), B(17 + numBytes));
        cHex += section;
    }
    //==============================================BOMBGENERATOR==============================================
    void compileBombGenerator()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[11]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(17), B(21 + numBytes));
        cHex += section;
    }
    //==============================================PANEL==============================================
    void compilePanel()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[12]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(9), B(13 + numBytes));
        cHex += section;
    }
    //==============================================HATMACHINE==============================================
    void compileHatMachine()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[13]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(14), B(18 + numBytes));
        cHex += section;
    }
    //==============================================PUSHBLOCKS==============================================
    void compilePushBlocks()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[14]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(14), B(18 + numBytes));
        cHex += section;
    }
    //==============================================TORPMACHINE==============================================
    void compileTorpMachine()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[15]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(16), B(20 + numBytes));
        cHex += section;
    }
    //==============================================SHADOWEDITOR==============================================
    void compileShadowEditor()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[16]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(16), B(20 + numBytes));
        cHex += section;
    }
    //==============================================GRAPPLE==============================================
    void compileGrapple()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[17]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(11), B(15 + numBytes));
        cHex += section;
    }
    //==============================================PLUG==============================================
    void compilePlug()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[18]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(8), B(12 + numBytes));
        cHex += section;
    }
    //==============================================TECHNO==============================================
    void compileTechno()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[19]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(10), B(14 + numBytes));
        cHex += section;
    }
}