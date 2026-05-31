using Godot;
using System;
using System.Collections.Generic;

public abstract partial class BaseObject : Node , IInteractable
{
	protected BaseInteractionClass InteractionClass { get; } 
	public abstract List<Interaction> GetInteractions();
	[Export] protected AnimationPlayer animationPlayer;
	protected abstract ObjectInteraction objectInteraction { get; }

}
