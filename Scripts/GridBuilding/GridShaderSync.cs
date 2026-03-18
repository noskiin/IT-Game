using Godot;
using System;

public partial class GridShaderSync : Node
{
	[Export] public VirtualGrid LogicGrid;
    [Export] public MeshInstance3D GridVisualizer; 
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (LogicGrid == null || GridVisualizer == null) return;
		var mat = GridVisualizer.GetActiveMaterial(0) as ShaderMaterial;
        if (mat == null) return;

		Vector3 shaderScale = new Vector3(
            1.0f / LogicGrid.CellSize.X,
            1.0f / LogicGrid.CellSize.Y,
            1.0f / LogicGrid.CellSize.Z
        );

		mat.SetShaderParameter("Scale", shaderScale);
	}
}
