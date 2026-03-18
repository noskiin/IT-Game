using Godot;
using Godot.Collections;
using System;
using System.Text.RegularExpressions;

public partial class PlayerInputHandling : Node
{
	[Export] Camera3D cam ;
    [Export] private Node3D Player ;
	[Export] private PackedScene interactionButton;

	public Dictionary CalculateRayFromCamera()
	{
		Vector2 mousePos = cam.GetViewport().GetMousePosition();
		Vector3 from = cam.ProjectRayOrigin(mousePos);
		var to = from + cam.ProjectRayNormal(mousePos) * 1000;
        var spaceState = Player.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = spaceState.IntersectRay(query);
		return result;
	}

	public void ClickedOn(Dictionary _rayResult)
	{
		if (_rayResult == null || _rayResult.Count == 0)
        	return;

		var hitNode = _rayResult["collider"].As<Node>();

		if(hitNode == null) return;

		switch(hitNode)
		{	case ObjectInteraction interaction:
				interaction.Interacted(_rayResult,interactionButton,cam);
				break;
			default:
				GD.Print("Trafiłem w: " + hitNode.Name + ", ale nie ma skryptu ObjectInteraction");
				break;
		}

	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("Interact"))
		{	
			Dictionary _dictionary = CalculateRayFromCamera();

			ClickedOn(_dictionary);
		}
		else
		{
			
		}

	}
}
