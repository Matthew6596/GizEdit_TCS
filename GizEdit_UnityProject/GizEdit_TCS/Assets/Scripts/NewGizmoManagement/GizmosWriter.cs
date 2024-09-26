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
    int gizIndex = 0;
    public IEnumerator CompileGizmos(BaseGizmo[] gizs, int[] numberOfEachGizmo)
    {
        //loadingStuff.SetActive(true);
        //changeLoadText("COMPILING...");

        gizIndex = 0;
        gizmos = gizs;
        numOfEachGiz = numberOfEachGizmo;

        //int gizsDone = 0;
        GizmosReader.reader.ReadLocation = 4;
        //start file
        cBytes.AddRange(BitConverter.GetBytes(1)); //version?

        //Gizmo Sections
        for (int i = 0; i < numberOfEachGizmo.Length; i++)
        {
            yield return null;
            //Debug.Log(i + ": " + cBytes.Count);
            if (GizmosReader.reader.sectionReady[i])
            {
                ConvertSectionToBin(i, writer.GetExtraHeaderStuff(i, numberOfEachGizmo));
            }
            else
            {
                ConvertSectionToBin2(i);
            }
            
            //gizsDone += numberOfEachGizmo[i];
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
    public byte[] ConvertGizmosToBin(int sectionNum)
    {
        List<byte> ret = new();
        int len = numOfEachGiz[sectionNum];
        for (int i = 0; i < len; i++, gizIndex++)
        {
            ret.AddRange(gizmos[gizIndex].ToBin());
        }
        return ret.ToArray();
    }
    public void ConvertSectionToBin2(int compileSection)
    {
        byte[] hb = writer.headerBytes[compileSection];
        GizmosReader.reader.ReadLocation += hb.Length;
        int numBytes = GameManager.ReadInt32();
        cBytes.AddRange(hb);
        cBytes.AddRange(BitConverter.GetBytes(numBytes));
        cBytes.AddRange(GameManager.ReadSlice(numBytes));
    }
    public void ConvertSectionToBin(int compileSection, byte[] extraHeaderStuff)
    {
        byte[] hb = writer.headerBytes[compileSection];
        //int afterHead = GizmosReader.reader.ReadLocation + hb.Length;
        GizmosReader.reader.ReadLocation += hb.Length;
        int numBytes = GameManager.ReadInt32();
        byte[] b = ConvertGizmosToBin(compileSection);
        cBytes.AddRange(hb);
        cBytes.AddRange(BitConverter.GetBytes(b.Length + extraHeaderStuff.Length));
        cBytes.AddRange(extraHeaderStuff);
        cBytes.AddRange(b);
        GizmosReader.reader.ReadLocation += numBytes;
        //cHex += hh + TypeConverter.Int32ToHex((uint)(r.Length+extraHeaderStuff.Length) / 3) + extraHeaderStuff + r;
    }
}
