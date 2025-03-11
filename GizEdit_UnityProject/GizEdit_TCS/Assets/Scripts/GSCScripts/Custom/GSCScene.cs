using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GSCScene
{
    public List<Model> sceneModels;
    public List<StaticObject> staticObjs;
    public List<SpecialObject> specialObjs;

    public static GSCScene LoadFromFile()
    {
        SFB.ExtensionFilter[] filters = new SFB.ExtensionFilter[] { 
            new("Game Scene", "gsc", "GSC", "Gsc", "tcscene", "TCScene", "TCSCENE") 
        };

        string[] paths = GameManager.gm.FileBrowser.OpenFilePanel("LSW:TCS Scene File Dialog", GameManager.gm.LastDirectory, filters, false);
        
        if (paths.Length <= 0 || !File.Exists(paths[0]))
        {
            throw new FileNotFoundException("No valid file was selected");
        }

        string dirPath = Path.GetDirectoryName(paths[0]);

        byte[] bytes = File.ReadAllBytes(paths[0]);
        GSCScene scene = (Path.GetExtension(paths[0]).ToLower()) switch
        {
            ".gsc"=>LoadFromGSC(bytes),
            ".tcscene"=>LoadFromTCScene(bytes),
            _ => throw new FileLoadException("File extension must be .gsc or .tcscene")
        };

        //Load giz and other also? prompt?
        List<string>gizs = Directory.EnumerateFiles(dirPath).ToList();
        string gizPath = "";
        foreach (string giz in gizs)
        {
            if (Path.GetExtension(giz).ToLower() == ".giz")
            {
                gizPath = giz;
                break;
            }
        }
        Debug.Log(gizPath);
        GizmosReader.instance.OpenGizFile(gizPath);

        return scene;
    }
    private static GSCScene LoadFromGSC(byte[] bytes)
    {
        GSCScene newScene = new();
        SceneLoader.LoadGSC(bytes,newScene);
        return newScene;
    }
    private static GSCScene LoadFromTCScene(byte[] bytes)
    {
        throw new NotImplementedException(".TCScene loading not yet implemented");
    }
}
