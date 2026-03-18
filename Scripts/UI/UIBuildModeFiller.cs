using Godot;
using System;

public partial class UIBuildModeFiller : Node
{
    [Export]
    private GridContainer buildModeContent;
    [Export]
    private ObjectsDatabaseBuildModeResource database;
    [Export]
    private PackedScene buttonPrefab;
    [Export]
    private PlacementSystem PlacementSys;
    public override void _Ready()
    {
        for (int i = 0; i < database.objectsData.Count; i++)
        {

            int index = i;
            // Instancjonuj prefab przycisk
			var instance = buttonPrefab.Instantiate();
            Button newButton = instance as Button;
			if (newButton == null)
        	{
        	    GD.PrintErr($"BŁĄD: Prefab numer {index} nie ma Buttona jako głównego węzła!");
        	    instance.QueueFree(); // Sprzątamy nieudany obiekt
        	    continue;
        	}

			buildModeContent.AddChild(newButton);
            newButton.Text = database.objectsData[index].Name.ToString();
            newButton.Name = database.objectsData[index].ID.ToString();
            newButton.Pressed += () =>
            { PlacementSys.StartPlacement(database.objectsData[index].ID); }; 
        }
    }
}
