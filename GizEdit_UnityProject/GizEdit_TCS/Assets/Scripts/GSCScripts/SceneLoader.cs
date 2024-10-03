using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static int pntrLocation=-1;

    public static int ReadLocation { get { return (int)reader.BaseStream.Position; } set { reader.BaseStream.Position = value; } }
    public static BinaryReader reader;

    public static List<Texture2D> textures = new();
    public static Dictionary<int,Texture2D> texturesDict = new();

    public void LoadGSC()
    {
        byte[] gscBytes = GameManager.gmInstance.gscBytes;
        MemoryStream ms = new (gscBytes);

        reader = new BinaryReader(ms);

        //preparing int vars
        int headerLocation;
        int nu20Start;

        //cache clearing
        //

        nu20Start = reader.ReadInt32() + 4;

        reader.BaseStream.Position = nu20Start + 0x18;

        pntrLocation = ReadLocation + reader.ReadInt32(); //pntrLocation = ReadLocation + file.ReadInt();
        headerLocation = ReadLocation + reader.ReadInt32(); //headerLocation = ReadLocation + file.ReadInt();

        ReadLocation = pntrLocation;
        parsePNTRValues(ms);

        ReadLocation = headerLocation-8;
        loadBlock(true);
        ReadLocation = nu20Start + 0x20; //ReadLocation = nu20Start+0x20;

        while (ReadLocation<gscBytes.Length) //Keep reading entire file till at end
            if (!loadBlock(false)) break; //Read Blocks (break if error)

        reader.Dispose();
        Debug.Log("[Done Reading .gsc]");
    }
    private void parsePNTRValues(MemoryStream ms)
    {
        int numPntr = reader.ReadInt32();
        for (int i = 0; i < numPntr; i++)
        {
            int pos = ReadLocation;

            int ptrOffset = reader.ReadInt32();

            int ptr = ptrOffset + pos;
            ReadLocation = ptr;

            int offset = reader.ReadInt32();
            if (offset != 0)
            {
                ms.Position = ptr;
                byte[] writeBytes = BitConverter.GetBytes(ptr + offset);
                ms.WriteByte(writeBytes[0]);
                ms.WriteByte(writeBytes[1]);
                ms.WriteByte(writeBytes[2]);
                ms.WriteByte(writeBytes[3]);
            }
            //fileBuffer.putInt(ptr, ptr + offset); //huh?!

            ReadLocation = pos;
        }
    }
    private bool loadBlock(bool loadGameScene)
    {
        int savePtr = ReadLocation; //savePtr = ReadLocation;
        int blockId = reader.ReadInt32(); //blockId = file.ReadInt();
        int blockSize = reader.ReadInt32(); //blockSize = file.ReadInt();

        //string blockName = Util.blockIDToString(Integer.reverseBytes(blockId)); //blockName = blockIDToString(blockID)
        int tmp=0;
        string blockName = TypeConverter.ReadFixedString(BitConverter.GetBytes(blockId), ref tmp, 4);

        if (blockSize > 1e8) return false; //If block big, BREAK!
        if (blockSize == 0) throw new Exception("Attempted to read block w/ length of 0");
        if (blockId == 0x4E55533A) return false; //blockID = 0x4E55533A, BREAK!

        //Create blockObj based on name
        DefaultFileBlock block;
        switch (blockName)
        {
            ///case "PORT": new PortalSetFileBlock(); break; //idk
            case "DISP": block = new DisplaySetBlock(); break;//want!
            //case "TST0": new TextureSetBlock(blockSize,blockId); break;//want! |--<<--|
            ///case "NTBL": new NameTableFileBlock(); break;//already kinda got
            ///case "BNDS": new BoundingBoxBlock(); break;//want!
            ///case "SST0": new SplineBlock(); break;//maybe
            ///case "BINH": new BINHBlock(); break;//idk
            case "GSNH":
                if (loadGameScene) block = new GameSceneHeaderBlock(); //want!
                else block = new DefaultFileBlock();
                break;
            ///case "INID": new DINIBlock(); break;//idk
            ///case "MS00": new MaterialBlock(); break;//want?
            ///case IABL_HEX_ID -> new IABLBlock();
            ///case ANIMATED_TEX_HEX_ID -> new AnimatedTextureBlock();
            default: block = new DefaultFileBlock(); break;
        };
        //Read block from file
        block.readFromFile(blockSize,blockId);

        ReadLocation = savePtr + blockSize;

        //Add blockObj to mapData
        //mapData.scene().blocks().put(blockName, new NU2MapData.SceneData.Block(savePtr, blockSize));
        return true;
    }

    //Public Method called after Open Gsc btn pressed
    public void LoadGscMesh()
    {
        LoadGSC();
    }
}
