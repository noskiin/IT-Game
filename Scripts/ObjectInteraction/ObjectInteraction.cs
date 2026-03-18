using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

public partial class ObjectInteraction : Node
{
	protected float radius = 150f;
	protected IInteractable _objectInteractionScript;


	public void Interacted(Dictionary _rayResult,PackedScene interactionButton,Camera3D cam)
	{
		if(_objectInteractionScript == null)
		{
			foreach(Node child in GetChildren())
			{
				if(child is IInteractable interactable) _objectInteractionScript = interactable;
			}
			if(_objectInteractionScript == null)
			{
				GD.PrintErr("Dalej nie ma Dictionary!");
				return;
			}
		}
		else
		{
			List<Interaction> options = _objectInteractionScript.GetInteractions();
			Vector3 worldPos = (Vector3)_rayResult["position"];
    		Vector2 centerOfMenu = cam.UnprojectPosition(worldPos);
			if (RadialMenu.Instance != null)
            {
				RadialMenu.Instance.InstantiateButtons(interactionButton,options);
				RadialMenu.Instance.GenerateRadialMenuUI(options.Count,centerOfMenu,options);
            }
            else 
            {
                GD.PrintErr("BŁĄD: RadialMenu.Instance jest nullem! Czy dodałeś skrypt RadialMenu do sceny?");
            }
		}

	}



	private void CloseDownWindow()
	{
		if (RadialMenu.Instance != null)
        {
			foreach (Node child in RadialMenu.Instance.GetNode(".").GetChildren()) 
            	if (child is CanvasItem ci) ci.Visible = false;
        }

	}



}



//Po kliknięciu przycisku wyłączyć wszystkie