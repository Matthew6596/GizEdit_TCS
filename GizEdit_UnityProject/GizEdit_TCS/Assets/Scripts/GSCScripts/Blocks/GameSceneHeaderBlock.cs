using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSceneHeaderBlock : DefaultFileBlock
{
    GameBuffer vertBuffer, indexBuffer;
    Dictionary<int,Mesh> meshes = new();
    int txtrCount;

    public override void readFromFile(int blockSize, int blockId)
    {
        base.readFromFile(blockSize,blockId);

        int texIndexList = readPointer(); //1240

        int texCount = SceneLoader.reader.ReadInt32(); //106
        int texMetaPtr = readPointer(); //1656
        int materialPtrList = readPointer(); //512

        //mapData.scene().materialListAddressFromGSNH().set(materialPtrList - blockOffset);
        //mapData.scene().texMetaListAddressFromGSNH().set(texMetaPtr - blockOffset);
        //mapData.scene().texIndexListAddressFromGSNH().set(texIndexList - blockOffset);

        txtrCount = texCount;

        // fileBuffer.position(0x28);
        // int meshMetaPtr = readFilePositionPlusOffset();

        Debug.Log(blockOffset + 0x1d0);
        SceneLoader.ReadLocation = blockOffset + 0x1d0; //14561932
        SceneLoader.ReadLocation = readPointer(); //27928

        int gscRenderableList = SceneLoader.ReadLocation; //27928
        SceneLoader.ReadLocation = gscRenderableList + 0x10; //27944

        int listStartAddr = readPointer(); //0 ??
        Debug.Log(SceneLoader.ReadLocation+" List start?: " + listStartAddr);
        int gscRenderableAmount = SceneLoader.reader.ReadInt32();
        Debug.Log("RenderableAmt: " + gscRenderableAmount);
        int gscListEndAddr = SceneLoader.reader.ReadInt32();
        int listEndAddr = listStartAddr + gscRenderableAmount * 4;


        for (int i = 0; i < gscRenderableAmount; i++)
        {
            SceneLoader.ReadLocation = listStartAddr + i * 4;
            int meshAddr = readPointer();
            Mesh mesh = readMesh(meshAddr);
            meshes.Add(meshAddr, mesh);
        }

        if (listEndAddr != gscListEndAddr)
        {
            Debug.LogWarning("Game scene header has unequal list ends for parts!");
        }

        Debug.Log("[MESHES READ] "+gscRenderableAmount);
        return;
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
            vertBuffer = new GameBuffer(data, bufAddr, bufSize);
        }

        int indexBufferCount = SceneLoader.reader.ReadInt16();
        for (int i = 0; i < indexBufferCount; i++)
        {
            int bufAddr = SceneLoader.ReadLocation;
            int bufSize = SceneLoader.reader.ReadInt32();
            byte[] data = SceneLoader.reader.ReadBytes(bufSize);
            indexBuffer = new GameBuffer(data, bufAddr, bufSize);
        }
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
        }

        SceneLoader.ReadLocation = 0x6;

        foreach (TextureDescriptor descriptor in realDescriptors)
        {
            int width = SceneLoader.reader.ReadInt32();
            int height = SceneLoader.reader.ReadInt32();
            SceneLoader.reader.ReadInt32();
            SceneLoader.reader.ReadInt32();
            SceneLoader.reader.ReadInt32();
            int size = SceneLoader.reader.ReadInt32();

            readTextureContents(new TextureDescriptor(descriptor.address, width, height, size, descriptor.trueIndex));
        }
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
        byte[] img = TypeConverter.ReadSlice(GameManager.gmInstance.gscBytes, ref tmp,descriptor.size);
        //SceneLoader.ReadLocation = tmp;
        /*ByteBuffer image = 
            Allocator.alloc(descriptor.size)
            .put(fileBuffer.slice(fileBuffer.position(), descriptor.size))
            .flip();(*/

        SceneLoader.ReadLocation += descriptor.size;

        Texture2D texture = Texture2D.grayTexture;
        //var texture = new FileTexture(name, image, textureStart, fileBuffer.position(), descriptor); //Creating texture

        SceneLoader.textures.Add(texture);
        SceneLoader.texturesDict.Add(descriptor.trueIndex,texture);
    }

    private TextureDescriptor readTextureDescriptor()
    {
        int pos = SceneLoader.ReadLocation;

        int width = SceneLoader.reader.ReadInt32();
        int height = SceneLoader.reader.ReadInt32();
        return new TextureDescriptor(pos, width, height, 0, 0);
    }

    private Mesh readMesh(int ptr)
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

        //----Make a unity Mesh with the above data (please)----
        Debug.Log("Mesh vert/index/size offsets & counts: " + "[vert: " + vertexOffset + ", " + vertexCount + ", " + vertexSize + "] [index: " + indexOffset + "]");

        return new Mesh();

        //computeIfAbsent(key, mappingFunc<? super k, ? extends v>)
        //Adding vertexListID to vertexBuffersBySize
        //mapData.scene().vertexBuffersBySize().computeIfAbsent(vertexSize, k-> new ArrayList<>()).add(vertexListID);

        //return new GSCMesh(ptr, vertexCount, vertexSize, vertexOffset, vertexListID, triangleCount, indexOffset, indexListID, useDynamicBuffer, dynamicBuffer);
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