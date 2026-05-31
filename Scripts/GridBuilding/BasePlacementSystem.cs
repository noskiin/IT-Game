using Godot;
using System;
using System.Collections.Generic;

public abstract partial class BasePlacementSystem : Node
{
    protected abstract VirtualGrid Grid { get; }
    protected abstract GridBuildingSystem BuildingSystem { get; }
    protected abstract PreviewPlacement PreviewSystem { get; }
    protected abstract ObjectsDatabaseBuildModeResource BuildModeSO { get; }
    protected abstract Node3D GridVisualisation { get; }
    protected abstract uint RaycastMask { get; }
    [Export] protected BuildModeControls _input;
    [Export] protected NavigationRegion3D NavMeshSurface;

	public int selectedObjIndex = -1;
    public int orientation = 0;
    private int rotation = 0;
    private Vector3I lastDetectedPos = Vector3I.Zero;
    public List<GridDatabase> ObjectsInSceneData;
    public List<Node3D> placedGameObjects = new();
    public event Action OnClick, OnExit;
    private int metaID;

	public override void _Ready()
    {
        ObjectsInSceneData = new();
        ObjectsInSceneData.Add(new GridDatabase());
        ObjectsInSceneData.Add(new GridDatabase());
    }

	protected Vector2I CalculatePivot(Godot.Collections.Array<Vector2I> occupiedCells)
    {
        if (occupiedCells.Count == 0) return Vector2I.Zero;
        int sumX = 0, sumY = 0;
        foreach (var c in occupiedCells) { sumX += c.X; sumY += c.Y; }
        return new Vector2I(
            Mathf.RoundToInt(sumX / (float)occupiedCells.Count),
            Mathf.RoundToInt(sumY / (float)occupiedCells.Count));
    }

	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
            OnClick?.Invoke();
        if (Input.IsKeyPressed(keycode:Key.Escape))
            OnExit?.Invoke();


        if (selectedObjIndex < 0)
            return;

        //Konwertowanie pozycji myszki na ppozycje kafelka grida
        Vector3 MousePosi = BuildingSystem.GetMapPosition();
        Vector3I gridPos = Grid.WorldToCell(MousePosi);
        Vector2I pivot = CalculatePivot(BuildModeSO.objectsData[selectedObjIndex].occupiedCells);
        Vector3I pivotWorldCell = gridPos - new Vector3I(pivot.X, 0, pivot.Y);
        //Sprawdzanie czy mozna postawc obiekt na kafelku. Jezeli : nie zmien kolor na czerwony, tak zmien kolor na bialy
        if (lastDetectedPos != gridPos)
        {
            bool placementValidity = CheckPlacementValidity(pivotWorldCell, selectedObjIndex);
            //przemieszczanie indykatora kafelka
            PreviewSystem.UpdatePos(Grid.GetCellCenterWorld(gridPos), placementValidity);
            lastDetectedPos = gridPos;
        }

        if (Input.IsKeyPressed(keycode: Key.B))
            DestroyObject();
	}
    
	public void StartPlacement(int ID)
    {
        // wylaczenie  stawiania obiektu by moc postawic koleny obiekt
        StopPlacement();
        GD.Print(ID + "Index");
        selectedObjIndex = -1;
        for (int i = 0; i < BuildModeSO.objectsData.Count; i++)
        {
            if (BuildModeSO.objectsData[i].ID == ID)
            {
                selectedObjIndex = i;
                break;
            }
        }  

        if (selectedObjIndex < 0)
        {
            GD.Print($"No ID found {ID}");
            return;
        }
        //Wlaczenie trybu budowania
        GridVisualisation.Visible = true;
        PreviewSystem.StartShowingPlacementPreview(
            BuildModeSO.objectsData[selectedObjIndex].Prefab,
            CalculatePivot(BuildModeSO.objectsData[selectedObjIndex].occupiedCells),
            BuildModeSO.objectsData[selectedObjIndex]
            );
        OnClick += PlaceStructure;
        OnClick -= DestroyObject;
        OnExit += StopPlacement;
        _input.RotatePressed += RotateGrid;
    }

	//Metoda ktora stawia obiekt w scenie na obiekcie Grid.
    private void PlaceStructure()
    {
        metaID++;
        //sprawdzenie czy mysz znajduje sie na UI gracza
        // if (IsPointerOverUI())
        //     return;  
        // // STRAŻNIK: Jeśli indeks jest mniejszy niż 0 lub większy niż rozmiar tablicy, wyjdź z metody!
        if (selectedObjIndex < 0 || selectedObjIndex >= BuildModeSO.objectsData.Count)
        {
            GD.Print($"Nie wybrano poprawnego obiektu do postawienia! ID : {selectedObjIndex}");
            return;
        }

        var objData = BuildModeSO.objectsData[selectedObjIndex];
        Vector3 MousePosi = BuildingSystem.GetMapPosition();
        Vector3I gridPos = Grid.WorldToCell(MousePosi);
        Vector2I pivot = CalculatePivot(objData.occupiedCells);

        Vector3I pivotWorldCell = gridPos - new Vector3I(pivot.X, 0, pivot.Y);

        //Sprawdzenie czy na wybranym przez gracza kafelku z grida znajduje sie juz obiekt
        bool placementValidity = CheckPlacementValidity(pivotWorldCell, selectedObjIndex);
        if (placementValidity == false)
            return;
        //Dodanie do sceny obiektu wybranego przez gracza na danym kafelku w gridzie
        Node3D buildModeObject = objData.Prefab.Instantiate<Node3D>();
        AddChild(buildModeObject);
        buildModeObject.Position = Grid.GetCellCenterWorld(gridPos);
        buildModeObject.Rotation = PreviewSystem.previewObj.GlobalRotation;
        placedGameObjects.Add(buildModeObject);

        /*Dodanie postawionego obiektu do bazy danych postawionych juz obiektow - przechowuje :
         * Ilosc kafelkow i miejsce kafelkow ktore zajmuje obiekt
         * ID w bazie danych 
         * ?????
         * ID obiektu
         
        Sprawdzic po co jest count(Plik GridDatabase.cs) i jjak cos usunac
         */
        GridDatabase selectedData = objData.Tag == "Floor" ? ObjectsInSceneData[0] : ObjectsInSceneData[1];
        selectedData.AddObjectAt(
            pivotWorldCell,
            objData.ID,
            placedGameObjects.Count,
            selectedObjIndex,
            objData.occupiedCells,
            pivot,
            PreviewSystem.orientation);
            buildModeObject.SetMeta("ID",metaID);
        PreviewSystem.UpdatePos(Grid.GetCellCenterWorld(gridPos), false);
    }

	private bool CheckPlacementValidity(Vector3I gridPos, int selectedObjIndex)
    {
        GridDatabase selectedData = BuildModeSO.objectsData[selectedObjIndex].Tag == "Floor" ? ObjectsInSceneData[0] : ObjectsInSceneData[1];
        Godot.Collections.Array<Vector2I> occupiedCells = BuildModeSO.objectsData[selectedObjIndex].occupiedCells;
        Vector2I pivot = CalculatePivot(occupiedCells);
        return selectedData.CanPlaceObjectAt(gridPos, BuildModeSO.objectsData[selectedObjIndex].occupiedCells, pivot, PreviewSystem.orientation);
    }
	private void StopPlacement()
    {
        selectedObjIndex = -1;
        GridVisualisation.Visible = false;
        PreviewSystem.StopShowingPreview();
        OnClick -= PlaceStructure;
        OnClick -= DestroyObject;
        OnExit -= StopPlacement;
        NavMeshSurface.BakeNavigationMesh(true);
        _input.RotatePressed -= RotateGrid;
        lastDetectedPos = Vector3I.Zero;
    }

	// public bool IsPointerOverUI()
    //     => EventSystem.current.IsPointerOverGameObject();

	private void RotateGrid(int RotationValue)
    { 
        Vector3 MousePosi = BuildingSystem.GetMapPosition();
        Vector3I gridPos = Grid.WorldToCell(MousePosi);
        Vector2I pivot = CalculatePivot(BuildModeSO.objectsData[selectedObjIndex].occupiedCells);
        Vector3I pivotWorldCell = gridPos - new Vector3I(pivot.X, 0, pivot.Y);

        if (RotationValue> 0)
        {
            orientation++;
            if (orientation > 3)
                orientation = 0;
            rotation = 90 * orientation;
        }
        else
        {
            orientation--;
            if (orientation < 0)
                orientation = 3;
            rotation = -90 * orientation;
       }

        PreviewSystem.orientation = orientation;
        bool placementValidity = CheckPlacementValidity(pivotWorldCell, selectedObjIndex);
        PreviewSystem.UpdateRotation(Grid.GetCellCenterWorld(gridPos), BuildModeSO.objectsData[selectedObjIndex], pivot, placementValidity, rotation);
    }
	private void DestroyObject()
    {
       //if()
    }
	
}
