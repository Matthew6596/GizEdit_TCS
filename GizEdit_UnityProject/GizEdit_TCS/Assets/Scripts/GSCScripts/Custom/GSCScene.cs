using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        byte[] bytes = File.ReadAllBytes(paths[0]);
        return (Path.GetExtension(paths[0]).ToLower()) switch
        {
            ".gsc"=>LoadFromGSC(bytes),
            ".tcscene"=>LoadFromTCScene(bytes),
            _ => throw new FileLoadException("File extension must be .gsc or .tcscene")
        };

        //Load giz and other also? prompt?

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
