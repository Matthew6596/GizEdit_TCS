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

    List<byte> cBytes = new();
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
        cBytes.AddRange(BitConverter.GetBytes(1)); //version?

        //Gizmo Sections
        for (int i = 0; i < numberOfEachGizmo.Length; i++)
        {
            yield return null;
            ConvertSectionToBin(i, gizsDone, writer.GetExtraHeaderStuff(i,numberOfEachGizmo));
            gizsDone += numberOfEachGizmo[i];
            //progress bar
        }

        //end file
        cBytes.AddRange(BitConverter.GetBytes(0));

        ExportGizFile(cBytes.ToArray());
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
    public byte[] ConvertGizmosToBin(int sectionNum, int start)
    {
        List<byte> ret = new();
        int len = numOfEachGiz[sectionNum] + start;
        for (int i = start; i < len; i++)
        {
            ret.AddRange(gizmos[i].ToBin());
        }
        return ret.ToArray();
    }
    public void ConvertSectionToBin(int compileSection, int gizsDone, byte[] extraHeaderStuff)
    {
        byte[] hb = writer.headerBytes[compileSection];
        //int afterHead = GizmosReader.reader.ReadLocation + hb.Length;
        GizmosReader.reader.ReadLocation += hb.Length;
        int numBytes = GameManager.ReadInt32();
        if (GizmosReader.reader.sectionReady[compileSection])
        {
            byte[] b = ConvertGizmosToBin(compileSection, gizsDone);
            cBytes.AddRange(hb);
            cBytes.AddRange(BitConverter.GetBytes(b.Length + extraHeaderStuff.Length));
            cBytes.AddRange(extraHeaderStuff);
            cBytes.AddRange(b);
            //cHex += hh + TypeConverter.Int32ToHex((uint)(r.Length+extraHeaderStuff.Length) / 3) + extraHeaderStuff + r;
        }
        else
        {
            cBytes.AddRange(hb);
            cBytes.AddRange(GameManager.ReadSlice(numBytes));
        }
    }
}
