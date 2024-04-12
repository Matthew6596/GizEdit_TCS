using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosWriter : MonoBehaviour
{
    GameManager gm;
    static public GizmosWriter instance;

    private void Start()
    {
        instance = this;
        gm = GameManager.gmInstance;
    }

    public int getPosAfter(string _val)
    {
        return gm.fhex.IndexOf(_val) + _val.Length;
    }
    public int getPosAfter(string _val, int startLocation)
    {
        return gm.fhex.IndexOf(_val, startLocation) + _val.Length;
    }

    static int B(int _bytes)
    {
        return (_bytes * 3);
    }
    static int B(uint _bytes)
    {
        return ((int)_bytes * 3);
    }
    string cHex = "";
    BaseGizmo[] gizmos;
    int[] numOfEachGiz;
    bool[] sectionChanged;
    public IEnumerator CompileGizmos(BaseGizmo[] gizs, int[] numberOfEachGizmo, bool[] _sectionChanged)
    {
        //loadingStuff.SetActive(true);
        //changeLoadText("COMPILING...");

        gizmos = gizs;
        numOfEachGiz = numberOfEachGizmo;
        sectionChanged = _sectionChanged;

        int gizsDone = 0;

        //start file
        cHex = "01 00 00 00 ";

        //Gizmo Sections
        for(int i=0; i<17; i++)
        {
            yield return null;
            ConvertSectionToHex(i, gizsDone, GetExtraHeaderStuff(i));
            gizsDone += numberOfEachGizmo[i];
            //progress bar
        }

        //end file
        cHex += "00 00 00 00 ";

        StartCoroutine(HexToBytes());
    }
    public IEnumerator HexToBytes()
    {
        yield return null; //<<temp
        int _l = cHex.Length / 3;
        byte[] _b = new byte[_l];
        for (int i = 0; i < _l; i++)
        {
            _b[i] = Convert.ToByte(TypeConverter.HexToInt8(cHex.Substring(i * 3, 3)));

            //if (i % 2000 == 0) { barImg.fillAmount = (float)i / _l; yield return null; } <<Progress bar
        }
        ExportGizFile(_b);
    }
    public void ExportGizFile(byte[] compiledBytes)
    {
        var extensions = new[] { new ExtensionFilter("Gizmo Files", "GIZ"), };
        string path = gm.FileBrowser.SaveFilePanel("Gizmo File Export", "", "CustomGizmos.GIZ", extensions);
        if (path.Length != 0)
        {
            System.IO.File.WriteAllBytes(path, compiledBytes);
        }
        //loadingStuff.SetActive(false);
        //EditorManager.TaskCompletePopup();
    }
    public string ConvertGizmosToHex(int sectionNum, int start)
    {
        string ret="";
        for (int i = start; i < numOfEachGiz[sectionNum]; i++)
        {
            ret += gizmos[i].ConvertToHex();
        }
        return ret;
    }
    public void ConvertSectionToHex(int compileSection, int gizsDone, string extraHeaderStuff)
    {
        string hh = TypeConverter.headerHex[compileSection];
        int afterHead = getPosAfter(hh);
        uint numBytes = gm.FSliceInt32(afterHead);
        if (sectionChanged[compileSection])
        {
            string r = ConvertGizmosToHex(compileSection, gizsDone);
            cHex += hh+TypeConverter.Int32ToHex((uint)r.Length / 3) +extraHeaderStuff+ r;
        }
        else
            cHex += gm.fhex.Substring(afterHead - hh.Length, hh.Length+B(4 + numBytes));
    }
    public string GetExtraHeaderStuff(int gizSection)
    {
        switch (gizSection)
        {
            //Obstacle
            case 0: return"";
            //Buildit
            case 1: return "09 " + TypeConverter.Int16ToHex((uint)numOfEachGiz[1]);
            //Force
            case 2: return"";
            //Blowup
            case 3: return"";
            //Pickup
            case 4: return"";
            //Lever
            case 5: return"";
            //Spinner
            case 6: return"";
            //Minicut
            case 7: return"";
            //Tube
            case 8: return"";
            //ZipUp
            case 9: return"";
            //Turret
            case 10: return"";
            //BombGenerator
            case 11: return"";
            //Panel
            case 12: return"";
            //HatMachine
            case 13: return"";
            //PushBlocks
            case 14: return"";
            //TorpMachine
            case 15: return"";
            //ShadowEditor
            case 16: return"";
        }
        return "";
    }
}
