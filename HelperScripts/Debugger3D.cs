using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Debugger3D : Node3D
{
    // DATA //
    // Cached Data
    private List<DebugEffect> timedEffects = new List<DebugEffect>();

    // Singleton pattern
    public static Debugger3D instance = null;
    
    // FUNCTIONS //
    // Godot Defaults
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            QueueFree();
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        EndElapsedEffects();
        base._Process(delta);
	}


    // Internal Functions
    private void EndElapsedEffects()
    {
        // Gets effects whose time has elapsed
        List<DebugEffect> effectsToRemove = new List<DebugEffect>();

        foreach(DebugEffect effect in timedEffects)
        {
            if(Time.GetUnixTimeFromSystem() - effect.StartTime > effect.Duration)
            {
                effectsToRemove.Add(effect);
            }
        }

        // Removes all the elapsed effects
        foreach(DebugEffect effect in effectsToRemove)
        {
            effect.EffectNode.QueueFree();
            timedEffects.Remove(effect);
        }
    }

    private void CacheTimedEffect(Node3D effect, double duration)
    {
        timedEffects.Add(new DebugEffect(effect, Time.GetUnixTimeFromSystem(), duration));
    }

    private static StandardMaterial3D CreateMaterial(Color colour, float alpha = 1)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        colour.A = Mathf.Clamp(alpha, 0, 1);
        material.AlbedoColor = colour;
        material.Metallic = 0;
        material.MetallicSpecular = 0;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        return material;
    }


    // External Functions
    private void CreateMeshEffect(Mesh mesh, double duration, Vector3? pos)
    {
        // Instantiates a MeshInstance to store the mesh, creates an effect for it, and sets
        // its position
        MeshInstance3D meshInstance = new MeshInstance3D();
        AddChild(meshInstance);
        meshInstance.Mesh = mesh;

        if(pos.HasValue)
        {
            meshInstance.GlobalPosition = pos.Value;
        }
        else
        {
            meshInstance.GlobalPosition = Vector3.Zero;
        }

        CacheTimedEffect(meshInstance, duration);
    }

    public void LineEffect(Vector3 startPos, Vector3 endPos, Color colour, double durationSeconds)
    {
        ImmediateMesh line = new ImmediateMesh();

        line.SurfaceBegin(Mesh.PrimitiveType.Lines, CreateMaterial(colour));
        line.SurfaceAddVertex(startPos);
        line.SurfaceAddVertex(endPos);
        line.SurfaceEnd();

        CreateMeshEffect(line, durationSeconds, null);
    }

    public void SphereEffect(Vector3 pos, float radius, Color colour, float alpha, double durationSeconds)
    {
        SphereMesh sphere = new SphereMesh();
        sphere.Radius = radius;
        sphere.Height = radius;
        sphere.Material = CreateMaterial(colour, alpha);

        CreateMeshEffect(sphere, durationSeconds, pos);
    }

}
