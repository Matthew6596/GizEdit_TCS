using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using UnityEngine;
using System.Drawing.Imaging;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader inst;

    public static int pntrLocation=-1;

    public static int ReadLocation { get { return (int)reader.BaseStream.Position; } set { reader.BaseStream.Position = value; } }
    public static BinaryReader reader;
    public static byte[] bytes;

    public List<Texture2D> textures = new();
    public static Dictionary<int,Texture2D> texturesDict = new();

    public static GSCScene sceneData;
    public static List<DefaultFileBlock> blocks = new();

    public static List<SceneMesh> meshes = new();
    public List<Material> materials = new();

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        GSCScene.LoadFromFile();
    }

    public static void LoadGSC(byte[] bytes, GSCScene sceneData)
    {
        SceneLoader.sceneData = sceneData;
        SceneLoader.bytes = bytes;
        MemoryStream ms = new (bytes);
        reader = new BinaryReader(ms);

        //preparing int vars
        int headerLocation;
        int nu20Start;

        //cache clearing
        //

        ReadLocation = 0;
        nu20Start = reader.ReadInt32() + 4;

        ReadLocation = nu20Start + 0x18;

        pntrLocation = ReadLocation + reader.ReadInt32();
        headerLocation = ReadLocation + reader.ReadInt32();

        ReadLocation = pntrLocation;
        parsePNTRValues(ms);

        ReadLocation = headerLocation-8;
        loadBlock(true);
        ReadLocation = nu20Start + 0x20;

        while (ReadLocation<bytes.Length) //Keep reading entire file till at end
            if (!loadBlock(false)) break; //Read Blocks (break if error)

        reader.Dispose();
        Debug.Log("[Done Reading .gsc]");

        foreach (DefaultFileBlock block in blocks)
        {
            if (block is GameSceneHeaderBlock gscBlock)
            {
                GameManager.gm.DelayAction(() =>
                {
                    gscBlock.FinishLoading();
                    Debug.Log("[Done Loading Meshes]");
                });
            }
        }
    }
    private static void parsePNTRValues(MemoryStream ms)
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

            ReadLocation = pos+4; //gonna cry i forgot "+4" like months ago and gave up cause it wansnt loading right
        }
    }
    private static bool loadBlock(bool loadGameScene)
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
            case "MS00": block = new MaterialBlock(); break;//want?
            ///case IABL_HEX_ID -> new IABLBlock();
            ///case ANIMATED_TEX_HEX_ID -> new AnimatedTextureBlock();
            default: block = new DefaultFileBlock(); break;
        };
        //Read block from file
        block.readFromFile(blockSize,blockId);

        ReadLocation = savePtr + blockSize;

        //Add blockObj to mapData
        //mapData.scene().blocks().put(blockName, new NU2MapData.SceneData.Block(savePtr, blockSize));
        blocks.Add(block);
        return true;
    }

    //Public Method called after Open Gsc btn pressed
    public static void LoadGscMesh()
    {
        //LoadGSC();
    }

    public static SceneMesh GetSceneMeshByPtr(int address)
    {
        foreach (SceneMesh mesh in meshes) if (mesh.fileIndex == address) return mesh;
        return null;
    }
}


public static class BinaryReaderExt
{
    public static Matrix4x4 ReadMatrix4(this BinaryReader reader)
    {
        return new(reader.ReadVector4(), reader.ReadVector4(), reader.ReadVector4(), reader.ReadVector4());
    }
    public static Vector4 ReadVector4(this BinaryReader reader)
    {
        return new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
    public static UnityEngine.Color ReadColorRGBA(this BinaryReader reader)
    {
        return new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        return new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
}