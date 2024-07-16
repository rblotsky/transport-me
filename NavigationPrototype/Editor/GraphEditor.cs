using Godot;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// This is intended to allow loading an arbitrary Scene and editing the NavGraph within it.
/// MAY BE DEPRECATED SOON.
/// </summary>
public partial class GraphEditor : Node
{
    // DATA //
    // Cached Data
    private string editedScenePath;
    private Node editedSceneRoot;


    // FUNCTIONS //
    // Getting available scenes
    public List<string> GetLoadableScenes(string scenesDirectory, bool recursive = false)
    {
        List<string> loadableScenes = new List<string>();

        // Opens the given directory
        using var dir = DirAccess.Open(scenesDirectory);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    GD.Print($"Found directory: {fileName}");

                    // If we are searching recursively, add all subdirectories too
                    if (recursive) 
                    {
                        loadableScenes.AddRange(GetLoadableScenes(Path.Combine(scenesDirectory, fileName), true));
                    }
                }
                else if(fileName.EndsWith(".tscn"))
                {
                    GD.Print($"Found scene file: {fileName}");

                    // Adds the filename b/c it's a scene
                    loadableScenes.Add(fileName);
                }
                else
                {
                    GD.Print($"Found non-scene file: {fileName}");
                }
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print($"An error occurred when trying to access: {scenesDirectory}");
        }

        return loadableScenes;
    }


    // Serializing Scenes
    private void ClearEditedScene()
    {
        editedScenePath = null;
        RemoveChild(editedSceneRoot);
        editedSceneRoot.QueueFree();
        editedSceneRoot = null;
    }

    private void LoadSceneAsEditable(string scenePath)
    {
        PackedScene newScene = ResourceLoader.Load<PackedScene>(scenePath);
        Node instantiatedScene = newScene.Instantiate();
        AddChild(instantiatedScene);

        // Caches the edited scene's path and its root in this scene
        editedScenePath = scenePath;
        editedSceneRoot = instantiatedScene;
    }

    private void SaveEditedScene()
    {
        PackedScene savedScene = new PackedScene();
        savedScene.Pack(editedSceneRoot);
        ResourceSaver.Save(savedScene, editedScenePath);
    }
}
