using Godot;
using System;

public partial class BuildModeControls : Node
{
	
// Definiujemy sygnał (odpowiednik event Action w Unity)
    [Signal]
    public delegate void RotatePressedEventHandler(int direction);

    public override void _UnhandledInput(InputEvent @event)
    {
        // Sprawdzamy akcję z Input Map (ustaw w Project Settings)
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
