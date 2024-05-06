using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosWriter : MonoBehaviour
{
    GameManager gm;
    static public GizmosWriter instance;
    static public IGizmosWriter writer;

    private void Start()
    {
        instance = this;
        gm = GameManager.gmInstance;

        writer = new TCSGizmosWriter();
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
    public IEnumerator CompileGizmos(BaseGizmo[] gizs, int[] numberOfEachGizmo)
    {
        //loadingStuff.SetActive(true);
        //changeLoadText("COMPILING...");

        gizmos = gizs;
        numOfEachGiz = numberOfEachGizmo;

        int gizsDone = 0;

        //start file
        cHex = "01 00 00 00 ";

        //Gizmo Sections
        for (int i = 0; i < numberOfEachGizmo.Length; i++)
        {
            yield return null;
            ConvertSectionToHex(i, gizsDone, writer.GetExtraHeaderStuff(i,numberOfEachGizmo));
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
        int len = numOfEachGiz[sectionNum] + start;
        for (int i = start; i < len; i++)
        {
            ret += gizmos[i].ConvertToHex();
        }
        return ret;
    }
    public void ConvertSectionToHex(int compileSection, int gizsDone, string extraHeaderStuff)
    {
        string hh = writer.headerHex[compileSection];
        int afterHead = getPosAfter(hh);
        uint numBytes = gm.FSliceInt32(afterHead/3);
        if (GizmosReader.reader.sectionReady[compileSection])
        {
            string r = ConvertGizmosToHex(compileSection, gizsDone);
            cHex += hh + TypeConverter.Int32ToHex((uint)(r.Length+extraHeaderStuff.Length) / 3) + extraHeaderStuff + r;
        }
        else
        {
            cHex += gm.fhex.Substring(afterHead - hh.Length, hh.Length + B(4 + numBytes));
        }
    }
}
