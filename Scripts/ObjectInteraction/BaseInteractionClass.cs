using Godot;
using System.Collections.Generic;

public abstract partial class BaseInteractionClass : Node , IInteractable
{
	public abstract List<Interaction> GetInteractions();

}
