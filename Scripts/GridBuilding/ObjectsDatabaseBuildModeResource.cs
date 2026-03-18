using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ObjectsDatabaseBuildModeResource : Resource
{
	[Export]    
	public Godot.Collections.Array<ObjectDataResource> objectsData;


}

