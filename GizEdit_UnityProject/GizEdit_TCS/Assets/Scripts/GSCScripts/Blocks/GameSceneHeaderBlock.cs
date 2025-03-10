using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

public class GameSceneHeaderBlock : DefaultFileBlock
{
    List<GameBuffer> vertBuffers=new(), indexBuffers=new();
    public Dictionary<int, SceneMesh> meshes = new();
    int txtrCount;

    public override void readFromFile(int blockSize, int blockId)
    {
        base.readFromFile(blockSize,blockId);

        int texIndexList = readPointer();

        int texCount = SceneLoader.reader.ReadInt32();
        int texMetaPtr = readPointer();
        int materialPtrList = readPointer();

        //mapData.scene().materialListAddressFromGSNH().set(materialPtrList - blockOffset);
        //mapData.scene().texMetaListAddressFromGSNH().set(texMetaPtr - blockOffset);
        //mapData.scene().texIndexListAddressFromGSNH().set(texIndexList - blockOffset);

        txtrCount = texCount;

        // fileBuffer.position(0x28);
        // int meshMetaPtr = readFilePositionPlusOffset();

        SceneLoader.ReadLocation = blockOffset + 0x1d0;
        SceneLoader.ReadLocation = readPointer();

        int gscRenderableList = SceneLoader.ReadLocation;
        SceneLoader.ReadLocation = gscRenderableList + 0x10;

        int listStartAddr = readPointer();
        //Debug.Log(SceneLoader.ReadLocation+" List start?: " + listStartAddr);
        int gscRenderableAmount = SceneLoader.reader.ReadInt32();
        //Debug.Log("RenderableAmt: " + gscRenderableAmount);
        int gscListEndAddr = SceneLoader.reader.ReadInt32();
        int listEndAddr = listStartAddr + gscRenderableAmount * 4;

        for (int i = 0; i < gscRenderableAmount; i++)
        {
            SceneLoader.ReadLocation = listStartAddr + i * 4;
            int meshAddr = readPointer();
            SceneMesh mesh = readMesh(meshAddr);
            meshes.Add(meshAddr, mesh);
            SceneLoader.meshes.Add(mesh);
        }

        if (listEndAddr != gscListEndAddr)
        {
            Debug.LogWarning("Game scene header has unequal list ends for parts!");
        }

        Debug.Log("[MESHES READ] "+SceneLoader.meshes.Count+"/"+gscRenderableAmount);
        //return;
        //mapData.scene().gscRenderableEndFromGSNH().set(listEndAddr - blockOffset);
        //mapData.scene().gscRenderableListFromGSNH().set(gscRenderableList - blockOffset);
        //mapData.scene().gscRenderableListLen().set(gscRenderableAmount);

        loadTCSTextures(texCount, texMetaPtr); //erroring, outofbounds

        int vertexBufferCount = SceneLoader.reader.ReadInt16();
        for (int i = 0; i < vertexBufferCount; i++)
        {
            int bufAddr = SceneLoader.ReadLocation;
            int bufSize = SceneLoader.reader.ReadInt32();
            byte[] data = SceneLoader.reader.ReadBytes(bufSize);
            vertBuffers.Add(new GameBuffer(data, bufAddr, bufSize));
            //add to vert buffers list
        }
        Debug.Log("[VERTEX BUFFERS READ]");

        int indexBufferCount = SceneLoader.reader.ReadInt16();
        for (int i = 0; i < indexBufferCount; i++)
        {
            int bufAddr = SceneLoader.ReadLocation;
            int bufSize = SceneLoader.reader.ReadInt32();
            byte[] data = SceneLoader.reader.ReadBytes(bufSize);
            indexBuffers.Add(new GameBuffer(data, bufAddr, bufSize));
            //add to index buffers list
        }
        Debug.Log("[INDEX BUFFERS READ]");
    }
    public void loadTCSTextures(int texCount, int texDescriptorPtr)
    {
        SceneLoader.ReadLocation = texDescriptorPtr;

        List<TextureDescriptor> realDescriptors = new();
        for (int i = 0; i < texCount; i++)
        {
            SceneLoader.ReadLocation=texDescriptorPtr + i * 4;
            int targetPtr = readPointer();
            SceneLoader.ReadLocation=targetPtr;

            TextureDescriptor descriptor = readTextureDescriptor();
            if (descriptor.w != 0 && descriptor.h != 0)
            {
                realDescriptors.Add(new TextureDescriptor(descriptor.address, descriptor.w, descriptor.h, 0, i));
            }
            else
            {
                realDescriptors.Add(null);
            }
        }

        SceneLoader.ReadLocation = 0x6;

        Debug.Log("NUM TEXTURES: " + realDescriptors.Count + "/" + texCount);
        foreach (TextureDescriptor descriptor in realDescriptors)
        {
            if (descriptor == null)
            {
                SceneLoader.inst.textures.Add(Texture2D.blackTexture);
                continue;
            }
            int width = SceneLoader.reader.ReadInt32();
            int height = SceneLoader.reader.ReadInt32();
            SceneLoader.reader.ReadInt32();
            SceneLoader.reader.ReadInt32();
            SceneLoader.reader.ReadInt32();
            int size = SceneLoader.reader.ReadInt32();

            readTextureContents(new TextureDescriptor(descriptor.address, width, height, size, descriptor.trueIndex));
        }

        Debug.Log("[TEXTURES READ?]");
    }

    private void readTextureContents(TextureDescriptor descriptor)
    {
        int textureStart = SceneLoader.ReadLocation;
        int type = SceneLoader.reader.ReadInt32();
        SceneLoader.ReadLocation=textureStart;

        string extension = type switch
        {
            0x474e5089 => ".png",
            _ => ".dxt"
        };

        //string name = mapData.xmlData().loadedTextures().getOrDefault("Texture_" + descriptor.trueIndex, "Texture_" + descriptor.trueIndex);

        //Image shenanigans
        int tmp = SceneLoader.ReadLocation;
        byte[] img = TypeConverter.ReadSlice(SceneLoader.bytes, ref tmp,descriptor.size);
        //SceneLoader.ReadLocation = tmp;
        /*ByteBuffer image = 
            Allocator.alloc(descriptor.size)
            .put(fileBuffer.slice(fileBuffer.position(), descriptor.size))
            .flip();(*/

        int temp = 0;
        Bitmap ddsTexture = KUtility.DDSImage.ReadDDSFromGSC(img, ref temp, out string ext);
        MemoryStream ddsMs = new();
        ddsTexture.Save(ddsMs, ddsTexture.RawFormat);
        Texture2D newTxtr = new(ddsTexture.Width, ddsTexture.Height);
        newTxtr.LoadImage(ddsMs.ToArray());
        newTxtr.name = "texture_" + ext;

        SceneLoader.ReadLocation += descriptor.size;

        //Texture2D texture = Texture2D.grayTexture;
        //var texture = new FileTexture(name, image, textureStart, fileBuffer.position(), descriptor); //Creating texture

        SceneLoader.inst.textures.Add(newTxtr);
        SceneLoader.texturesDict.Add(descriptor.trueIndex, newTxtr);
    }

    private TextureDescriptor readTextureDescriptor()
    {
        int pos = SceneLoader.ReadLocation;

        int width = SceneLoader.reader.ReadInt32();
        int height = SceneLoader.reader.ReadInt32();
        return new TextureDescriptor(pos, width, height, 0, 0);
    }

    private SceneMesh readMesh(int ptr)
    {
        SceneLoader.ReadLocation=ptr;

        int type = SceneLoader.reader.ReadInt32();

        int triangleCount = SceneLoader.reader.ReadInt32();
        int vertexSize = SceneLoader.reader.ReadInt16();

        SceneLoader.ReadLocation = ptr + 0x14;

        int vertexOffset = SceneLoader.reader.ReadInt32();
        int vertexCount = SceneLoader.reader.ReadInt32();

        int indexOffset = SceneLoader.reader.ReadInt32();
        int indexListID = SceneLoader.reader.ReadInt32();
        int vertexListID = SceneLoader.reader.ReadInt32();
        int useDynamicBuffer = SceneLoader.reader.ReadInt32();

        SceneLoader.ReadLocation = ptr + 0x34;
        int dynamicBuffer = SceneLoader.reader.ReadInt32();

        if (type != 6) Debug.LogWarning("Attempted to load a non-triangle strip mesh at " + ptr);

        //Debug.Log("Mesh vert/index/size offsets & counts: " + "[vert: " + vertexOffset + ", " + vertexCount + ", " + vertexSize + "] [index: " + indexOffset + "]");

        GameObject meshObj = new("msh" + SceneLoader.ReadLocation);
        meshObj.transform.SetParent(GameManager.gm.allMeshesParent);
        SceneMesh mesh = meshObj.AddComponent<SceneMesh>();
        mesh.fileIndex = ptr;
        mesh.PrepareLoad(new(triangleCount,vertexSize,vertexOffset,vertexCount,indexOffset,indexListID,vertexListID,useDynamicBuffer,dynamicBuffer));
        return mesh;

        //computeIfAbsent(key, mappingFunc<? super k, ? extends v>)
        //Adding vertexListID to vertexBuffersBySize
        //mapData.scene().vertexBuffersBySize().computeIfAbsent(vertexSize, k-> new ArrayList<>()).add(vertexListID);

        //return new GSCMesh(ptr, vertexCount, vertexSize, vertexOffset, vertexListID, triangleCount, indexOffset, indexListID, useDynamicBuffer, dynamicBuffer);
        
    }
    public void FinishLoading()
    {
        //Loop through all meshes and load using vertex buffers and index buffers
        foreach(int key in meshes.Keys)
        {
            SceneMesh mesh = meshes[key];
            MeshInfo info = mesh.preloadInfo;
            //Debug.Log($"LOAD MESH: verts:{info.vertexCount},tri:{info.triangleCount},indOff:{info.indexOffset},vertsize:{info.vertexSize}");

            byte[] vertexBytes = vertBuffers[info.vertexListID].bytes;
            (Vector3[] verts, Vector2[] uvs) = vertBytesToVerts(vertexBytes, info.vertexCount, info.vertexOffset, info.vertexSize);
            int[] indices = indexBytesToIndices(indexBuffers[info.indexListID].bytes, info.triangleCount + 2, info.indexOffset);

            //LogVerts(verts);
            //LogIndices(indices);
            indices = convertFromStripToTriangles(indices);
            //LogIndices(indices);

            //meshes[key].Load(Texture2D.grayTexture,verts,indices,null);
            meshes[key].Load(null,verts,indices,uvs);
        }
    }
    private (Vector3[], Vector2[]) vertBytesToVerts(byte[] bytes,int count,int offset,int vertSize)
    {
        Vector3[] verts = new Vector3[count];
        Vector2[] uvs = new Vector2[count];
        //if (vertSize != 16 && vertSize!=24) Debug.LogWarning("Vertex size is not 16 nor 24, therefore they may be read incorrectly. Vertex Size:"+vertSize);
        for(int i=0; i<count; i++)
        {
            int baseOffset = offset * vertSize + i * vertSize;
            float x = BitConverter.ToSingle(bytes, baseOffset);
            float y = BitConverter.ToSingle(bytes, baseOffset+4);
            float z = BitConverter.ToSingle(bytes, baseOffset+8);
            verts[i] = new Vector3(x, y, z);
            uvs[i] = Vector2.zero;

            Vector3 normal = new(bytes[baseOffset + 12]/byte.MaxValue, bytes[baseOffset + 13]/byte.MaxValue, bytes[baseOffset + 14]/byte.MaxValue);

            if (vertSize == 16) continue;
            Vector3 biTangent = new(bytes[baseOffset + 16] / byte.MaxValue, bytes[baseOffset + 17] / byte.MaxValue, bytes[baseOffset + 18] / byte.MaxValue);

            //if (vertSize <= 20) continue;
            UnityEngine.Color color = new(bytes[baseOffset + 20] / sbyte.MaxValue, bytes[baseOffset + 21] / sbyte.MaxValue, bytes[baseOffset + 22] / sbyte.MaxValue, bytes[baseOffset + 23] / sbyte.MaxValue);

            if (vertSize <= 28) continue;
            Vector2 uv = new Vector2(BitConverter.ToSingle(bytes, baseOffset + 24), BitConverter.ToSingle(bytes, baseOffset + 28));

            uvs[i] = uv;
        }
        return (verts,uvs);
    }
    private int[] indexBytesToIndices(byte[] bytes, int count, int offset)
    {
        int[] indices = new int[count];
        for(int i=0; i<count; i++)
        {
            indices[i] = BitConverter.ToInt16(bytes, offset*2 + i * 2);
        }
        return indices;
    }
    private int[] convertFromStripToTriangles(int[] stripIndices)
    {
        List<int> indices = new();

        bool alt = true;
        for (int i=2; i<stripIndices.Length; i++)
        {
            if (alt)
            {
                indices.Add(stripIndices[i - 2]);
                indices.Add(stripIndices[i - 1]);
                indices.Add(stripIndices[i]);
            }
            else
            {
                indices.Add(stripIndices[i]);
                indices.Add(stripIndices[i - 1]);
                indices.Add(stripIndices[i - 2]);
            }
            alt = !alt;
        }
        //Count goes from triangle+2 --> triange*3
        //1,2,3,4,5 --> 1,2,3,2,3,4,3,4,5
        return indices.ToArray();
    }
    private void LogVerts(Vector3[] verts)
    {
        string debug = "[ ";
        foreach(Vector3 vert in verts) { debug += vert + ", "; }
        Debug.Log(debug + " ]");
    }
    private void LogIndices(int[] inds)
    {
        string debug = "[ ";
        foreach (int ind in inds) { debug += ind + ", "; }
        Debug.Log(debug + " ]");
    }

}

class TextureDescriptor
{
    public int address;
    public int size;
    public int w, h;
    public int trueIndex;
    public TextureDescriptor(){}
    public TextureDescriptor(int pos, int w, int h, int size, int trueIndex)
    {
        address = pos; this.w = w; this.h = h; this.size = size; this.trueIndex=trueIndex;
    }
}

struct GameBuffer
{
    public byte[] bytes;
    public int fileLocation;
    public int size;
    public GameBuffer(byte[] bytes, int filePos, int size) { this.bytes=bytes; fileLocation=filePos; this.size=size; }
}

public struct MeshInfo
{
    public int triangleCount,vertexSize,vertexOffset,vertexCount,indexOffset,indexListID,vertexListID,useDynamicBuffer,dynamicBuffer;
    public MeshInfo(int triangleCount,int vertexSize,int vertexOffset,int vertexCount,int indexOffset,int indexListID,int vertexListID,int useDyBuf,int dyBuf)
    {
        this.triangleCount = triangleCount;
        this.vertexSize = vertexSize;
        this.vertexOffset = vertexOffset;
        this.vertexCount = vertexCount;
        this.indexOffset = indexOffset;
        this.indexListID = indexListID;
        this.vertexListID = vertexListID;
        useDynamicBuffer = useDyBuf;
        dynamicBuffer = dyBuf;
    }
}