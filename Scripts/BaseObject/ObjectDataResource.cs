using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ObjectDataResource : Resource
{
	[Export]    
	public string Name { get; set; }

    [Export]
    public int ID { get;  set; }

    [Export]
    public int Cost {get; set;}

    [Export]
    public Godot.Collections.Array<Vector2I> occupiedCells { get; set; }

    [Export]
    public Godot.Collections.Array<Vector2I> snapPoint {get;set;} = new();

    public bool CustomShape;

    [Export]
    public Vector2I Pivot { get; set; } = Vector2I.Zero;

    [Export]
    public PackedScene Prefab { get;  set; }

    [Export]
    public Godot.Collections.Array<string> Tags { get;  set; }


    
}
