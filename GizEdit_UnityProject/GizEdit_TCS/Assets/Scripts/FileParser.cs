using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;

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
        //readObstacles(); <<very lag for some reason
        //Read build its
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
        readPanels();
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
        int startData = afterHead + B(6);
        int endLocation = B(numBytes) + afterHead;
        //

        int readLocation = startData-B(1);
        int rc = 0;
        while (readLocation < endLocation) { //numBytes driven loop
            readLocation = getPosAfter("66 6F 72 63 65 ", readLocation) - B(5);

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
            //Unknown 4 bytes
            //Unknown 3 bytes
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
                readLocation += B(8); //Unknown 8 bytes
                _gfc.isSelected = gm.FSliceInt8(readLocation) == 0;
                readLocation += B(6); //Unknown 5 bytes

                //Assign child
                _gfc.gizParent = _g.transform;
                _g.childrenList.Add( _child );
            }
            //Parse closer data
            _g.forceSpeed = gm.FSliceFloat32(readLocation);
            _g.resetSpeed = gm.FSliceFloat32(readLocation+B(4));
            readLocation += B(12); //Unknown 4 bytes
            _g.effectScale = gm.FSliceFloat32(readLocation);
            readLocation += B(8); //Unknown 4 bytes

            uint unknownStringLength = gm.FSliceInt8(readLocation);
            string unknownString = gm.FSliceString(readLocation + B(1), unknownStringLength);
            readLocation += B(unknownStringLength + 1);

            _g.minStudValue = gm.FSliceInt16(readLocation);
            _g.maxStudValue = gm.FSliceInt16(readLocation+B(2));
            readLocation += B(6); //Unknown 2 bytes
            _g.studSpawnPosition = new Vector3(gm.FSliceFloat32(readLocation),gm.FSliceFloat32(readLocation+B(4)),gm.FSliceFloat32(readLocation+B(8)));
            _g.studSpeed = gm.FSliceFloat32(readLocation+B(12));
            readLocation += B(16);

            List<string> _sfx = new();
            uint sfxNameLength = gm.FSliceInt8(readLocation);
            readLocation += B(1);
            while (sfxNameLength != 0)
            {
                _sfx.Add(gm.FSliceString(readLocation, sfxNameLength));
                readLocation += B(sfxNameLength);
                sfxNameLength = gm.FSliceInt8(readLocation);
                readLocation += B(1);
            }
            _g.sfxs = _sfx.ToArray();

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
            string pickupType = gm.FSliceString(readLocation+B(20), 3);
            char ptype = pickupType[0];
            if(ptype!='s'&&ptype != 'g' && ptype != 'b' && ptype != 'p' && ptype != 'm' && ptype != 'r' && ptype != 'u' && ptype != 'h' && ptype != 'c' && ptype != 't') { Debug.Log("UNKNOWN PICKUP: " + ptype); return; }

            //Creating Pickup Object
            GameObject _pickupObj = new();
            GizmoPickup _props = _pickupObj.AddComponent<GizmoPickup>();
            _props.pickupType = ptype.ToString();
            _pickupObj.transform.position = new Vector3(pickupX, pickupY, pickupZ);
            _props.pickupName = pickupName;
            _pickupObj.name = "pickup_"+rp;

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
            float rotation = TypeConverter.Int8AngleToFloat(gm.FSliceInt8(readLocation + B(1)));
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
        cHex = "";
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

        return cHex;
    }

    //==============================================PICKUPS==============================================
    void compileObstacles()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[0]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(15), B(15 + numBytes));
        cHex += section;
    }
    void compileBuildit()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[1]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(14), B(14 + numBytes));
        cHex += section;
    }
    void compileForce()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[2]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(12), B(12 + numBytes));
        cHex += section;
    }
    void compileBlowup()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[3]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(10), B(10 + numBytes));
        cHex += section;
    }
    void compilePickups()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[4]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string pickupSection = gm.fhex.Substring(afterHead-B(15),B(15+numBytes));
        cHex += pickupSection;
    }
    void compileLever()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[5]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(9), B(9 + numBytes));
        cHex += section;
    }
    void compileSpinner()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[6]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(11), B(11 + numBytes));
        cHex += section;
    }
    void compileMinicut()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[7]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(11), B(11 + numBytes));
        cHex += section;
    }
    void compileTube()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[8]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(8), B(8 + numBytes));
        cHex += section;
    }
    void compileZipup()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[9]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(9), B(9 + numBytes));
        cHex += section;
    }
    void compileTurret()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[10]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(13), B(13 + numBytes));
        cHex += section;
    }
    void compileBombGenerator()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[11]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(18), B(18 + numBytes));
        cHex += section;
    }
    void compilePanel()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[12]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(9), B(9 + numBytes));
        cHex += section;
    }
    void compileHatMachine()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[13]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(14), B(14 + numBytes));
        cHex += section;
    }
    void compilePushBlocks()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[14]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(14), B(14 + numBytes));
        cHex += section;
    }
    void compileTorpMachine()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[15]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(16), B(16 + numBytes));
        cHex += section;
    }
    void compileShadowEditor()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[16]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(16), B(16 + numBytes));
        cHex += section;
    }
    void compileGrapple()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[17]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(11), B(11 + numBytes));
        cHex += section;
    }
    void compilePlug()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[18]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(8), B(8 + numBytes));
        cHex += section;
    }
    void compileTechno()
    {
        int afterHead = getPosAfter(TypeConverter.headerHex[19]);
        uint numBytes = gm.FSliceInt32(afterHead);
        string section = gm.fhex.Substring(afterHead - B(10), B(10 + numBytes));
        cHex += section;
    }
}