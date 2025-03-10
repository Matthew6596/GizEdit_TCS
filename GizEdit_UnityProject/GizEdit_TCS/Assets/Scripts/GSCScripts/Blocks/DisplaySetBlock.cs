using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class DisplaySetBlock : DefaultFileBlock
{
    enum DisplayCommandType
    {
        MTL = 0x80,
        GEOMCALL = 0x82,
        MTXLOAD = 0x83,
        TERMINATE = 0x84,
        MTL_CLIP = 0x85,
        DUMMY = 0x87,
        DYNAMIC_GEOMETRY = 0x8b,
        END = 0x8e,
        NEXT = 0x8d,
        FACEON = 0x8f,
        LIGHTMAP = 0xb0,
        OTHER = 0x0
    }

    public override void readFromFile(int blockSize, int blockId) //MOVE ALL THIS TO DISPLAY SET BLOCK ===================
    {
        base.readFromFile(blockSize, blockId);

        SceneLoader.ReadLocation = blockOffset + 0x4;
        int displayListLength = SceneLoader.reader.ReadInt32(); //get length
        int displayListPos = readPointer(); //get pos
        List<SceneMesh> displayCommandList = parseDisplayCommands(displayListPos); //parse commands

        SceneLoader.ReadLocation = blockOffset + 0x60;
        int staticDisplayObjectCount = SceneLoader.reader.ReadInt32();
        int staticDisplayObjectPos = readPointer();

        //parseDisplayLists(staticDisplayObjectPos, staticDisplayObjectCount);

        //Can skip to here
        SceneLoader.ReadLocation = blockOffset + 0x10; //16 bytes after DISP?
        int gameModelCount = SceneLoader.reader.ReadInt32(); //get num models
        int gameModelPointer = readPointer();  //get pointer
        parseGameModels(gameModelPointer, gameModelCount, displayCommandList); //parse game models

        SceneLoader.ReadLocation = blockOffset + 0x18;
        var modelSizeList = readPointer();

        /*
        mapData.scene().modelSizeListStart().set(modelSizeList);

        for (int i = 0; i < mapData.scene().gameModels().size(); i++)
        {
            SceneLoader.reader.position(modelSizeList + i * 2);

            var sizeListEntry = SceneLoader.reader.getShort();
            var modelSize = mapData.scene().gameModels().get(i).modelParts().size();

            if (modelSize != sizeListEntry)
            {
                GGConsole.warning("Non-matching size entries for model " + mapData.scene().gameModels().get(i).name() + ": " + sizeListEntry + " vs " + modelSize);
            }
        }
        */

        SceneLoader.ReadLocation = blockOffset + 0x4c;
        int dispObject4Size = SceneLoader.reader.ReadInt32();

        SceneLoader.ReadLocation = blockOffset + 0x54;
        int dispObject4Pos = readPointer();
        //parseDisplayObject4s(dispObject4Pos, dispObject4Size);

        SceneLoader.ReadLocation = blockOffset + 0x6c; //can skip to here
        int specialObjectSize = SceneLoader.reader.ReadInt32();
        int specialObjectLocation = readPointer();

        List<SpecialObject> specialObjects = parseSpecialObjects(specialObjectLocation, specialObjectSize);
        //mapData.scene().specialObjects().addAll(parseSpecialObjects(specialObjectLocation, specialObjectSize)); //parse special objects
        //mapData.scene().modelInstances().addAll(analyzeCustomListForInstances()); //call function for model instances
    }

    private List<SceneMesh> parseDisplayCommands(int commandListStart)
    {
        List<SceneMesh> displayCommands = new();
        SceneLoader.ReadLocation = commandListStart;
        int commandSize = 16; //size

        int commandPtr = commandListStart;
        int commandIndex = 0;
        string ptrList = "";
        while (true)
        {
            SceneLoader.ReadLocation = commandPtr;
            byte commandType = SceneLoader.reader.ReadByte();
            uint flags = SceneLoader.reader.ReadByte();

            SceneLoader.ReadLocation = commandPtr + 4;
            int resourcePtr = readPointer();
            ptrList += resourcePtr + ", ";

            if (commandType == (byte)DisplayCommandType.MTL_CLIP) flags = 0;
            if (commandType == (byte)DisplayCommandType.DYNAMIC_GEOMETRY) flags = 0;
            if (commandType == (byte)DisplayCommandType.END)
            {
                //mapData.scene().renderCommandList().add(new DisplayCommand(commandIndex, commandPtr, flags, DisplayCommandType.END, new UntypedCommandResource(resourcePtr, DisplayCommand.CommandType.END)));
                displayCommands.Add(SceneLoader.GetSceneMeshByPtr(resourcePtr));
                break;
            }


            //if (!mapData.scene().uniqueRenderCommands().containsKey(resourcePtr))
            {
                /*DisplayCommandResource <?> nextRenderCommand = switch (commandType)
                {
                    case MTXLOAD-> {
                            fileBuffer.position(resourcePtr);
                            var matrix = BufferUtil.readMatrix4f(fileBuffer);
                            yield new MatrixCommandResource(resourcePtr, matrix);
                        }
                    case GEOMCALL->mapData.scene().meshes().get(resourcePtr);
                    case MTL->mapData.scene().materials().get(resourcePtr);
                    case LIGHTMAP-> {
                            fileBuffer.position(resourcePtr);
                            int type = fileBuffer.getInt();
                            int lm = fileBuffer.getInt();
                            int lm2 = fileBuffer.getInt();
                            int lm3 = fileBuffer.getInt();
                            int lm4 = fileBuffer.getInt();
                            float xOffset = fileBuffer.getFloat();
                            float yOffset = fileBuffer.getFloat();
                            float zOffset = fileBuffer.getFloat();
                            float wOffset = fileBuffer.getFloat();

                            yield new LightmapCommandResource(resourcePtr, type, lm, lm2, lm3, lm4, xOffset, yOffset, zOffset, wOffset, mapData);

                        }
                    default -> {
                            yield new UntypedCommandResource(resourcePtr, commandType);
                        }
                };
                mapData.scene().uniqueRenderCommands().put(resourcePtr, nextRenderCommand);*/
            }
            displayCommands.Add(SceneLoader.GetSceneMeshByPtr(resourcePtr));
            //mapData.scene().renderCommandList().add(new DisplayCommand(commandIndex, commandPtr, flags, commandType, mapData.scene().uniqueRenderCommands().get(resourcePtr)));
            commandPtr += commandSize;
            commandIndex++;
        }
        Debug.Log(ptrList);
        return displayCommands;
    }

    private List<SpecialObject> parseSpecialObjects(int specialObjectPos, int specialObjectSize)
    {
        List<SpecialObject> objs = new();
        for (int i = 0; i < specialObjectSize; i++)
        {
            int thisSpecObj = specialObjectPos + i * 0xd0;
            SceneLoader.ReadLocation = thisSpecObj;
            Matrix4x4 initialMatrix = SceneLoader.reader.ReadMatrix4();
            //var localIABL = getIABLObject(fileBuffer);

            Vector4 unknownVec43 = SceneLoader.reader.ReadVector4();

            int modelAddress = readPointer();
            int stringAddr = readPointer();
            int visibilityFn = SceneLoader.reader.ReadInt32();
            int lodPtr = readPointer();
            int boundingBoxIndex = SceneLoader.reader.ReadInt32();
            int remoteIablAddress = SceneLoader.reader.ReadInt32();
            short windShearFactor = SceneLoader.reader.ReadInt16();
            short windSpeedFactor = SceneLoader.reader.ReadInt16();

            //Debug.Log($"Special Object {stringAddr} model at {modelAddress}");

            /*int index = get3ALA(fileBuffer,remoteIablAddress);

            SceneMesh displayObject = mapData.scene().gameModels().stream().filter(m->m.modelAddress() == modelAddress).findFirst().get();
            string name = NameTableFileBlock.CURRENT.getByOffsetFromStart(stringAddr); //get name from NameTableBlock by address
            var lods = getLODLevels(lodPtr);

            SpecialObject specObj = new(displayObject, initialMatrix, localIABL, remoteIablAddress,
                    name, boundingBoxIndex, lods, windSpeedFactor / 65535.0f, windShearFactor / 65535.0f, thisSpecObj);
            objs.Add(specObj);*/
        }
        return objs;
    }

    /*public IABLObject getIABLObject(ByteBuffer source)
    {
        int addr = SceneLoader.ReadLocation;
        Matrix4x4 localIablMatrix = SceneLoader.reader.ReadMatrix4();
        Vector3 boundsPos = SceneLoader.reader.ReadVector3();
        SceneLoader.reader.ReadSingle();
        Vector3 boundsSize = SceneLoader.reader.ReadVector3();
        SceneLoader.reader.ReadSingle();

        return new IABLObject(localIablMatrix, new IABLObject.IABLBoundingBox(boundsPos, boundsSize, addr + 16 * 4), addr);
    }*/

    private void parseGameModels(int gameModelPos, int gameModelCount, List<SceneMesh> displayCommands)
    {
        //var allCommands = mapData.scene().renderCommandList();
        //var materialsCopy = List.copyOf(mapData.scene().materials().values());

        for (int i = 0; i < gameModelCount; i++)
        {
            int address = gameModelPos + (i * 0xc); //addr = (pntr ~16 bytes after DISP?) + (i*0xc)

            //set name
            //string name = mapData.xmlData().loadedModels().getOrDefault("GameModel_" + i, "GameModel_" + i);
            string name = "GameModel_" + i;

            SceneLoader.ReadLocation = address; //ReadLocation = addr;
            int commandCount = SceneLoader.reader.ReadInt32(); //commandCount = SceneLoader.reader.ReadInt();

            //Special case for gungan command count
            //if (mapData.name().toLowerCase(Locale.ROOT).contains("gungan") && commandCount == 0) commandCount = 3;

            int materialOffset = readPointer(); //matOffset = readPointer();
            int meshOffset = readPointer(); //meshOffset = readPointer();
            //Debug.Log($"{name} has {commandCount} cmds, mats at {materialOffset}, meshs at {meshOffset}");
            //Initialize list for materials and meshes
            //List<FileMaterial> materials = new ArrayList<>();
            List<int> materials = new();
            List<SceneMesh> renderables = new();
            List<int> renderableIndices = new();

            SceneLoader.ReadLocation = materialOffset; //ReadLocation = matoffset;
            string debugStr = $"{name} mats:[";
            for (int j = 0; j < commandCount; j++) //for commandCount
            {
                //might have to read materials block cuz normal maps and such
                //materials.Add(materialsCopy.get(fileBuffer.getInt())); //get material, index at ReadInt()?
                int matIndex = SceneLoader.reader.ReadInt32();
                debugStr += matIndex + ",";
                materials.Add(matIndex);
            }
            debugStr += "], meshes:[";
            SceneLoader.ReadLocation = meshOffset; //ReadLocation = meshOffset;
            for (int j = 0; j < commandCount; j++) //for commandCount
            {
                int renderableIndex = SceneLoader.reader.ReadInt32(); //meshIndex = ReadInt();
                //var commandResource = allCommands.get(renderableIndex).command(); //idk, get command for mesh from meshIndex
                debugStr += renderableIndex+",";
                renderableIndices.Add(renderableIndex);
                //NOTE, NOT MESH, COMMAND LIST --> try getting the renderCommandList[renderableIndex].command();
                if (displayCommands[renderableIndex]!=null)
                    displayCommands[renderableIndex].SetMaterial(SceneLoader.inst.materials[materials[j]]);
                /*if (commandResource instanceof GSCMesh rc) {
                if(commandResource is SceneMesh rc)
                    renderables.Add(rc); //add mesh to renderables list, get mesh from command?
                }
                else
                {
                    renderables.Add(null); // is FACEON, investigate
                }*/
            }
            Debug.Log(debugStr+"]");
            /*var commands = IntStream.range(0, commandCount) //foreach command, make new model part with material[i] renderable[i]
                        .mapToObj(idx-> new GameModel.GameModelPart(materials.get(idx), renderables.get(idx), renderableIndices.get(idx)))
                        .collect(Collectors.toList());
            mapData.scene().gameModels().add(new GameModel(name, commands, address, materialOffset, meshOffset, renderableIndices));*/
        }
    }

    /*private List<ModelInstance> analyzeCustomListForInstances()
    {
        //check for duplicate materials
        var existingMeshes = new HashMap<DisplayCommandResource<?>, GameModel>();

        foreach (var model : mapData.scene().gameModels())
        {
            foreach (var part : model.modelParts())
            {
                existingMeshes.put(mapData.scene().renderCommandList().get(part.sourceCommandIndex()).command(), model);
            }
        }

        var lastDisplayList = mapData.scene().displayLists().get(mapData.scene().displayLists().size() - 1);

        if (lastDisplayList.isCustomList())
        {
            record InstanceFinder(MatrixCommandResource matrix,
                                   List<DisplayCommand> foundMeshes,
                                   GameModel model)
            { }
            ;

            var completeInstances = new ArrayList<ModelInstance>();
            var matrixToInstanceMap = new HashMap<Integer, InstanceFinder>();
            MatrixCommandResource lastResource = null;

            for (var command : lastDisplayList.commands())
            {
                if (command.command() instanceof MatrixCommandResource mcr)
                {
                    lastResource = mcr;
                    continue;
                }

                var associatedModel = existingMeshes.get(command.command());
                if (associatedModel != null)
                {
                    MatrixCommandResource finalLastResource = lastResource;
                    var existingInstance = matrixToInstanceMap.computeIfAbsent(lastResource.address(),
                            l-> new InstanceFinder(finalLastResource, new ArrayList<>(), associatedModel));
                    existingInstance.foundMeshes.add(command);
                }
            }

            for (var instanceFinder : matrixToInstanceMap.values())
            {
                if (instanceFinder.foundMeshes().size() == instanceFinder.model().modelParts().size())
                {
                    var previousInstancesOfModel = completeInstances.stream().filter(c->c.model() == instanceFinder.model()).count();
                    completeInstances.add(new ModelInstance(instanceFinder.model(), instanceFinder.matrix, instanceFinder.foundMeshes(), (int)previousInstancesOfModel));
                }
            }

            return completeInstances;
        } 
        else
        {
            return List.of();
        }
    }*/
}
