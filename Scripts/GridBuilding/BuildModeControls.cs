using Godot;
using System;

public partial class BuildModeControls : Node
{
	
    [Signal]
    public delegate void RotatePressedEventHandler(int direction);

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("RotateObjRight"))
        {
            // "Krzyczymy" do systemu: Obróć w prawo!
            EmitSignal(SignalName.RotatePressed, 1);
        }
        else if (@event.IsActionPressed("RotateObjLeft"))
        {
            EmitSignal(SignalName.RotatePressed, -1);
        }
    }
}
