using Godot;
using System;

public partial class GameMaster : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}



public static class Layers
{
    public const uint None = 0;
    
    public const uint Player = 1u << 0;  // Warstwa 1 w edytorze
    public const uint Enemy  = 1u << 1;  // Warstwa 2 w edytorze
    public const uint Wall   = 1u << 2;  // Warstwa 3 w edytorze
    public const uint BuildingLayer = 1u << 3;  // Warstwa 4 w edytorze
    public const uint ObjectLayer = 1u << 4;  // Warstwa 5 w edytorze

    
    public const uint All = ~0u;         // Skrót na "Wszystkie warstwy" (same jedynki)
}
