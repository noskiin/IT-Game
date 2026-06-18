using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract partial class BaseObject : Node
{
	protected String displayName,objectName;
	protected Int32 cost;
	protected AnimationPlayer animationPlayer;
	protected ObjectInteraction objectInteraction { get; }

	protected List<Group> snapPoints_Tags {get; }
	protected List<Node3D> snapPoints_Pos {get; }

}
