using Godot;
using System;

[GlobalClass]
public partial class ObjectDataResource : Resource
{
	[Export]    
	public string Name { get; set; }

    [Export]
    public int ID { get;  set; }

    [Export]
    public Godot.Collections.Array<Vector2I> occupiedCells { get; set; }

    public bool CustomShape;

    [Export]
    public Vector2I Pivot { get; set; } = Vector2I.Zero;

    [Export]
    public PackedScene Prefab { get;  set; }

    [Export]
    public string Tag { get;  set; }
}
