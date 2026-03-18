using Godot;
using System;
using System.Collections.Generic;
using System.Threading;


public partial class PreviewPlacement : Node
{

	[Export]
    private float previewYOffset = 0.06f;
    int rotation;
    [Export]
    private PackedScene cellIndicator;
    [Export]
    public Vector3 CellSize = new Vector3(0.5f, 0.5f, 0.5f);
    public Node3D previewObj;

    [Export]
    private StandardMaterial3D previewMaterialPrefab;
    private StandardMaterial3D previewMaterialInstance;

    private MeshInstance3D cellIndicatorRenderer;

    [Export]
    //private BuildModeControlls buildModeControlls;

    public int orientation = 0;

    [Export]
    private Godot.Collections.Array<Vector2I> rotatedOffsets = new Godot.Collections.Array<Vector2I>();

    [Export]
    private Godot.Collections.Array<Node3D> indicatorPool = new Godot.Collections.Array<Node3D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		previewMaterialInstance = (StandardMaterial3D)previewMaterialPrefab.Duplicate();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartShowingPlacementPreview(PackedScene prefab, Vector2I pivot,ObjectDataResource data )//ObjectsDatabaseBuildModeSO
    {
        previewObj = prefab.Instantiate<Node3D>();
        AddChild(previewObj);
        previewObj.GlobalRotationDegrees = new Vector3(0, rotation, 0);
        DisableCollision();
        // napisać skrypt Disable Specjalnie Dla CollisonShape3D jest w edytorze ta funkcja enable disable collision
        PreparePreview(previewObj);
        PrepareCursor(data.occupiedCells,pivot);

    }
	private void PreparePreview(Node3D previewObj)
    {
		var renderers = previewObj.FindChildren("*", "MeshInstance3D",recursive: true);
        foreach(MeshInstance3D mat in renderers)
        {
            var material = previewMaterialInstance;
            mat.MaterialOverride = material;
        }

    }

    private void DisableCollision()
    {
        var ColliderNode = previewObj.FindChildren("*","CollisionShape3D",recursive: true);
        if(ColliderNode != null ) GD.Print($"HALO COS NIE TAK {previewObj.Name}");
		foreach (CollisionShape3D co in ColliderNode)
		{   
            if(co is CollisionShape3D collider)
            {
                co.Disabled = true;
            }
		}
    }

    private void PrepareCursor(Godot.Collections.Array<Vector2I> occupiedCells, Vector2I pivot)
    {
        rotatedOffsets.Clear();
        if (occupiedCells.Count <= 0 || occupiedCells == null)
        {
            rotatedOffsets.Add(Vector2I.Zero);
        }
        else
        {
            foreach (var cell in occupiedCells)
            {
                Vector2I relative = cell - pivot;
                Vector2I rot = RotateOffset(orientation, relative);
                rotatedOffsets.Add(rot);

            }

            while (indicatorPool.Count < rotatedOffsets.Count)
            {
                var go =  cellIndicator.Instantiate<Node3D>();
                AddChild(go);
                go.Scale = CellSize;
                go.Visible = true;
                MeshInstance3D rdr = go.FindChild("MeshInstance3D", true) as MeshInstance3D;   
                if(rdr != null) rdr.MaterialOverride = previewMaterialInstance;
                indicatorPool.Add(go);
            }

            for (int i = 0; i < indicatorPool.Count; i++)
            {
                indicatorPool[i].Visible = true;
            }
        }
    }

    public void StopShowingPreview()
    {
        if(IsInstanceValid(previewObj))
        {
            previewObj.QueueFree();
        }

        for (int i = 0; i < indicatorPool.Count; i++)
        {
            indicatorPool[i].Visible =false ;
        }

    }

    public void UpdatePos(Vector3 pos,bool validity)
    {
        MovePreview(pos);
        MoveCursor(pos);
        ApplyFeedback(validity);
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Colors.Green : Colors.Red;
        c.A = .5f;
        var material = (StandardMaterial3D)previewMaterialInstance.Duplicate();
        material.AlbedoColor = c;
        foreach (MeshInstance3D cell in indicatorPool)
        {   
            cell.MaterialOverride = material;
        }

        var renderers = previewObj.FindChildren("*", "MeshInstance3D",recursive: true);
        foreach(MeshInstance3D mat in renderers)
        {
            mat.MaterialOverride = material;
        }
    }
    private void MoveCursor(Vector3 pos)
    {
        for (int i = 0; i < rotatedOffsets.Count; i++)
        {
            var off = rotatedOffsets[i];
            var go = indicatorPool[i];
            float offsetX = off.X * CellSize.X;
            float offsetZ = off.Y * CellSize.Z;
            go.GlobalPosition = new Vector3(pos.X + offsetX, pos.Y, pos.Z + offsetZ);
            go.Visible = true;
        }
    }

    private void MovePreview(Vector3 pos)
    {
        if (previewObj == null) return;
            previewObj.GlobalPosition = new Vector3(pos.X, pos.Y + previewYOffset, pos.Z);
    }

    public void UpdateRotation(Vector3 pos,ObjectDataResource data, Vector2I pivot, bool validity,int rotation2)
    {
        ApplyFeedback(validity);
        /*Debug.Log("XD" + pos + data + pivot);*/
        PrepareCursor(data.occupiedCells, pivot);
        ApplyFeedback(validity);
        MoveCursor(pos);
        MovePreview(pos);
        ApplyFeedback(validity);
        rotation = rotation2;
        previewObj.GlobalRotationDegrees = new Vector3(0, rotation2, 0);
    }
    public Vector2I RotateOffset(int orientation, Vector2I offset)
    {
        switch (orientation)
        {
            case 0: // 0�
                return new Vector2I(offset.X, offset.Y); ;
            case 1: // 90�: (x,z) -> (z, -x)
                return new Vector2I(offset.Y, -offset.X);
            case 2: // 180�: (x,z) -> (-x, -z)
                return new Vector2I(-offset.X, -offset.Y);
            case 3: // 270�: (x,z) -> (-z, x)
                return new Vector2I(-offset.Y, offset.X);
            default:
                return offset;

        }

    }

}