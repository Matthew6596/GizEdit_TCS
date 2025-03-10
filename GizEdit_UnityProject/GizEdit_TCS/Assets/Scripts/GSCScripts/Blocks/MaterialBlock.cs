using System;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Diagnostics;

class MaterialBlock : DefaultFileBlock
{
    public override void readFromFile(int blockSize, int blockId)
    {
        base.readFromFile(blockSize, blockId);

        int materialCount = SceneLoader.reader.ReadInt32();
        SceneLoader.reader.ReadInt32();

        for (int i = 0; i < materialCount; i++)
        {
            int ptr = SceneLoader.ReadLocation;
            //System.out.println("material " + i + " | " + Integer.toHexString(ptr));

            //var material = new FileMaterial(ptr);
            //mapData.scene().materials().put(ptr, material);

            SceneLoader.ReadLocation = ptr;
            byte[] data = SceneLoader.reader.ReadBytes(0x2C4);

            SceneLoader.ReadLocation = ptr + 0x38;
            int materialID = SceneLoader.reader.ReadInt32(); //get ID for material
            //material.mysteryPointer = readPointer();
            int mysteryPointer = readPointer();
            int tempLocation = SceneLoader.ReadLocation;
            SceneLoader.ReadLocation = mysteryPointer;
            int valAtPtr = SceneLoader.reader.ReadInt32();
            SceneLoader.ReadLocation = tempLocation;
            //Debug.Log($"Mystery Pointer: {mysteryPointer} is {valAtPtr}");

            //material.setID(materialID);
            int alphaBlend = SceneLoader.reader.ReadInt32();

            SceneLoader.ReadLocation = ptr + 0x54;
            /*material.setColor(new Vector4f(
                    SceneLoader.reader.getFloat(),
                    SceneLoader.reader.getFloat(),
                    SceneLoader.reader.getFloat(),
                    SceneLoader.reader.getFloat())); //material color!*/
            Color color = SceneLoader.reader.ReadColorRGBA();

            SceneLoader.ReadLocation = ptr + 0x74;
            int diffuseTextureIndex = SceneLoader.reader.ReadInt16();
            //material.setDiffuseFileTexture(mapData.scene().texturesByRealIndex().get((int)SceneLoader.reader.getShort())); //material texture! <<<
            SceneLoader.ReadLocation = ptr + 0xB4;
            int textureFlags = SceneLoader.reader.ReadInt32();
            //material.setTextureFlags(SceneLoader.reader.getInt()); //texture flags (idk what those are but cool)
            int diffuseTextureIndex2 = SceneLoader.reader.ReadInt32();
            
            //material.setDiffuseFileTexture(mapData.scene().texturesByRealIndex().get(SceneLoader.reader.getInt())); //texture again??? <<<
            int layer1TexID = SceneLoader.reader.ReadInt32();
            if (layer1TexID != -1)
            {
                //material.setLayer1DiffuseTexture(mapData.scene().texturesByRealIndex().get(layer1TexID));
            }
            SceneLoader.ReadLocation = ptr + 0xB4 + 0x78;
            float reflPower = SceneLoader.reader.ReadSingle();
            float exp = SceneLoader.reader.ReadSingle();

            SceneLoader.ReadLocation = ptr + 0xB4 + 0x90;
            float fresnelMul = SceneLoader.reader.ReadSingle();
            float fresnelCoeff = SceneLoader.reader.ReadSingle();
            //material.setReflectivityColor(Util.packedIntToVector4f(0x7f7f7f7f));
            //material.setSpecular(new Vector4f(exp, reflPower, fresnelMul, fresnelCoeff));

            SceneLoader.ReadLocation = ptr + 0xB4 + 0x44;
            byte combineop1 = SceneLoader.reader.ReadByte();
            //material.setCombineOp1(combineop1);
            SceneLoader.reader.ReadByte();
            SceneLoader.reader.ReadByte();
            SceneLoader.reader.ReadByte();
            int specularTextureIndex = SceneLoader.reader.ReadInt32();
            int normalTextureIndex = SceneLoader.reader.ReadInt32();
            //material.setSpecularFileTexture(mapData.scene().texturesByRealIndex().get(SceneLoader.reader.getInt())); //specular texture?
            //material.setNormalIndex(mapData.scene().texturesByRealIndex().get(SceneLoader.reader.getInt())); //normal texture <<<
            //Debug.Log($"Mat{i} texture index?: {diffuseTextureIndex} or {diffuseTextureIndex2}, spec:{specularTextureIndex}, norm:{normalTextureIndex}, color: ({color.r},{color.g},{color.b})");
            Texture diffuse = (diffuseTextureIndex==-1)?Texture2D.whiteTexture:SceneLoader.inst.textures[diffuseTextureIndex];
            Texture normal = (normalTextureIndex == -1) ? Texture2D.normalTexture : SceneLoader.inst.textures[normalTextureIndex];
            Texture spec = (specularTextureIndex == -1) ? Texture2D.grayTexture : SceneLoader.inst.textures[specularTextureIndex];
            if (diffuse.name.Contains("DXT3") || diffuse.name.Contains("DXT5"))
            {
                SceneLoader.inst.materials.Add(MaterialExt.GetStandardTransparent(diffuse,normal,spec,color));
            }
            else
            {
                SceneLoader.inst.materials.Add(MaterialExt.GetStandard(diffuse,normal,spec,color));
            }

            SceneLoader.ReadLocation = ptr + 0xB4 + 0x13;
            //int vertexFormatBits = SceneLoader.reader.getInt();
            //int formatBits2 = SceneLoader.reader.getInt();

            SceneLoader.ReadLocation = ptr + 0xB4 + 0xA8;
            byte lightmapIdx = SceneLoader.reader.ReadByte();
            byte surfaceIdx = SceneLoader.reader.ReadByte();
            byte specularIdx = SceneLoader.reader.ReadByte();
            byte normalIdx = SceneLoader.reader.ReadByte();
            //Debug.Log($"lightmapIdx:{lightmapIdx}, surfaceIdx:{surfaceIdx}, specularIdx:{specularIdx}, normalIdx:{normalIdx}");

            /*SceneLoader.reader.position(ptr + 0xB4 + 0x1B4);
            int inputDefines = SceneLoader.reader.getInt();
            int shaderDefines = SceneLoader.reader.getInt();
            int uvsetCoords = SceneLoader.reader.getInt();

            material.setAlphaType(alphaBlend);
            material.setFormatBits(vertexFormatBits);
            material.setInputDefinesBits(inputDefines);
            material.setShaderDefinesBits(shaderDefines);
            material.setUVSetCoords(uvsetCoords);
            material.setLightmapSetIndex(lightmapIdx);
            material.setSpecularIndex(specularIdx);
            material.setSurfaceUVIndex(surfaceIdx);
            material.generateShaderSettings();

            for (int j = 0; j < 4; j++)
            {
                SceneLoader.reader.position(ptr + 0x1c0 + j * 4);
                var animEnabled = SceneLoader.reader.getInt();

                SceneLoader.reader.position(ptr + 0x1F8 + j * 20);
                var animTypeX = switch (SceneLoader.reader.get())
                {
                    case 2->UVAnimType.LINEAR;
                    case 3->UVAnimType.SINE;
                    case 4->UVAnimType.COSINE;
                    default -> UVAnimType.OFF;
                }
                ;
                var animTypeY = switch (SceneLoader.reader.get())
                {
                    case 2->UVAnimType.LINEAR;
                    case 3->UVAnimType.SINE;
                    case 4->UVAnimType.COSINE;
                    default -> UVAnimType.OFF;
                }
                ;
                SceneLoader.reader.get();
                SceneLoader.reader.get();

                var xTrigScale = SceneLoader.reader.getFloat();
                var yTrigScale = SceneLoader.reader.getFloat();
                var xScrollSpeed = SceneLoader.reader.getFloat();
                var yScrollSpeed = SceneLoader.reader.getFloat();

                material.setUVAnimationProperties(j,
                    new UVAnimationProperties(
                        j,
                        animEnabled != -1,
                        animTypeX,
                        animTypeY,
                        xScrollSpeed,
                        xTrigScale,
                        yScrollSpeed,
                        yTrigScale)
                );
            }

            if (isLayerEnabled(1, shaderDefines))
            {
                SceneLoader.reader.position(ptr + 0x74);
                var t1 = SceneLoader.reader.getShort();
                var t2 = SceneLoader.reader.getShort();
                var t3 = SceneLoader.reader.getShort();
            }
            else
            {
                SceneLoader.reader.position(ptr + 0x74);
                var t1 = SceneLoader.reader.getShort();
                var t2 = SceneLoader.reader.getShort();
                var t3 = SceneLoader.reader.getShort();
                // System.out.println("not layer 1:" + t1+","+layer1TexID + "," + combineop1);
            }*/

            SceneLoader.ReadLocation = (ptr + 0x2C4);

        }
    }
}
