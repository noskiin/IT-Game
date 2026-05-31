using Godot;
using System;
using System.Collections.Generic;


/*Musisz dodatkowo Stworzyć Skrypt Dla SetActive*/
public partial class PlacementSystem : BasePlacementSystem
{
    [Export] private VirtualGrid _grid;
    [Export] private GridBuildingSystem _buildingSystem;
    [Export] private PreviewPlacement _previewSystem;
    [Export] private ObjectsDatabaseBuildModeResource _buildModeSO;
    [Export] private Node3D _gridVisualisation;

    protected override VirtualGrid Grid => _grid;
    protected override GridBuildingSystem BuildingSystem => _buildingSystem;
    protected override PreviewPlacement PreviewSystem => _previewSystem;
    protected override ObjectsDatabaseBuildModeResource BuildModeSO => _buildModeSO;
    protected override Node3D GridVisualisation => _gridVisualisation;
    protected override uint RaycastMask => Layers.BuildingLayer;

    public void OnStartPlacement(int ID) => StartPlacement(ID);

}
