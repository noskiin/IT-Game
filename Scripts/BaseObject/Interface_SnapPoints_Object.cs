using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public interface Interface_SnapPoints_Object
{
	protected List<Group> snapPoints_Tags {get; }
	protected List<Node3D> snapPoints_Pos {get; }
}
