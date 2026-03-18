using Godot;
using System;
using System.Xml.XPath;

public partial class Player_Movement : Node
{
	[Export] PlayerInputHandling _playerInputHandling;
    [Export]  private NavigationAgent3D _navigationAgent3D;
    [Export] private Node3D Player ;
    [Export] private  float Speed {get; set;} = 5f;
	private float _movementDelta;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_navigationAgent3D = GetParent<NavigationAgent3D>();
		_navigationAgent3D.VelocityComputed += OnVelocityComputed;
	}
	public override void _PhysicsProcess(double delta)
	{		

		if (Input.IsMouseButtonPressed(MouseButton.Right))
		{
			SetMovementTarget();
		}

		if (_navigationAgent3D.IsNavigationFinished())
            return;

		Vector3 nextPathPosition = _navigationAgent3D.GetNextPathPosition();
		Vector3 newVelocity = Player.GlobalPosition.DirectionTo(nextPathPosition) * Speed;
		if (_navigationAgent3D.AvoidanceEnabled)
        {
            _navigationAgent3D.Velocity = newVelocity;
        }
        else
        {
            // Jeśli avoidance wyłączony, sami wywołujemy logikę ruchu
            OnVelocityComputed(newVelocity);
        }



	}

	private void SetMovementTarget()
    {
		var result = _playerInputHandling.CalculateRayFromCamera();
        if (result.Count > 0)
        {	
			Vector3 hitPoint = (Vector3)result["position"];
            _navigationAgent3D.TargetPosition = hitPoint;
        }
    }

	private void OnVelocityComputed(Vector3 safeVelocity)
    {
        // Tutaj nakładamy deltę, bo zmieniamy pozycję co klatkę
        float delta = (float)GetPhysicsProcessDeltaTime();
        Player.GlobalPosition = Player.GlobalPosition.MoveToward(Player.GlobalPosition + safeVelocity, Speed * delta);
        
        // Opcjonalnie: Obracanie gracza w stronę ruchu
        if (safeVelocity.Length() > 0.1f)
        {
            Vector3 lookAtTarget = Player.GlobalPosition + safeVelocity;
            lookAtTarget.Y = Player.GlobalPosition.Y; // Blokada osi Y, żeby nie "nurkował"
            Player.LookAt(lookAtTarget, Vector3.Up);
        }
    }
}


