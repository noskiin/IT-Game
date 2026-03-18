using Godot;
using System;
using System.Collections.Generic;


/*Musisz dodatkowo Stworzyć Skrypt Dla SetActive*/
public partial class PlacementSystem : Node
{
	public event Action OnClick, OnExit;
    
    private BuildModeControls _input;
    public GridBuildingSystem GridBuildingSys;
	[Export]
    private ObjectsDatabaseBuildModeResource m_BuildModeSO;
	[Export]
    private NavigationRegion3D m_NavMeshSurface;

    public int selectedObjIndex = -1;

	[Export]
    private Node3D gridVisualisation;

    public List<GridDatabase> ObjectsInSceneData;

    public List<Node3D> placedGameObjects = new();

	[Export]
    private PreviewPlacement previewSystem;

    private Vector3I lastDetectedPos = Vector3I.Zero;
	[Export]
	private VirtualGrid grid;
	[Export]
    //private BuildModeControlls buildModeControlls;
    public int orientation = 0;
    private int rotation = 0;

    private int metaID;
	// Called when the node enters the scene tree for the first time.   
	public override void _Ready()
	{
        _input = GetNode<BuildModeControls>("BuidlingModeInputHandler");
        GridBuildingSys =GetNode<GridBuildingSystem>("GridBuildingSystem");
		StopPlacement();
        ObjectsInSceneData = new();
        GridDatabase floorData = new();
        GridDatabase objectData = new();
        ObjectsInSceneData.Add(floorData); ObjectsInSceneData.Add(objectData);
        //Umieszczenie pierwszego elementu na scenie
        /* GameObject buildModeObject = Instantiate(m_BuildModeSO.objectsData[0].Prefab);
         buildModeObject.transform.position = GridBuildingSys.grid.GetCellCenterWorld(new Vector3Int(0, 2, 0));
         m_NavMeshSurface.BuildNavMesh();
         placedGameObjects.Add(buildModeObject);
         //dodanie pierwszego elementu na scenie do bazy danych elementow dodanych do sceny
         GridDataBase selectedData = m_BuildModeSO.objectsData[0].Prefab.tag == "Floor" ? ObjectsInSceneData[0] : ObjectsInSceneData[1];
         selectedData.AddObjectAt(
             new Vector3Int(0, 0, 0),
             m_BuildModeSO.objectsData[0].ID,
             placedGameObjects.Count,
             0,
             m_BuildModeSO.objectsData[0].occupiedCells,
             CalculatePivot(m_BuildModeSO.objectsData[0].occupiedCells),
             0
             );*/
        /*        Debug.Log(selectedData.ToString());
        */
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
            OnClick?.Invoke();
        if (Input.IsKeyPressed(keycode:Key.Escape))
            OnExit?.Invoke();


        if (selectedObjIndex < 0)
            return;

        //Konwertowanie pozycji myszki na ppozycje kafelka grida
        Vector3 MousePosi = GridBuildingSys.GetMapPosition();
        Vector3I gridPos = grid.WorldToCell(MousePosi);
        Vector2I pivot = CalculatePivot(m_BuildModeSO.objectsData[selectedObjIndex].occupiedCells);
        Vector3I pivotWorldCell = gridPos - new Vector3I(pivot.X, 0, pivot.Y);
        //Sprawdzanie czy mozna postawc obiekt na kafelku. Jezeli : nie zmien kolor na czerwony, tak zmien kolor na bialy
        if (lastDetectedPos != gridPos)
        {
            bool placementValidity = CheckPlacementValidity(pivotWorldCell, selectedObjIndex);
            //przemieszczanie indykatora kafelka
            previewSystem.UpdatePos(grid.GetCellCenterWorld(gridPos), placementValidity);
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
        for (int i = 0; i < m_BuildModeSO.objectsData.Count; i++)
        {
            if (m_BuildModeSO.objectsData[i].ID == ID)
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
        gridVisualisation.Visible = true;
        previewSystem.StartShowingPlacementPreview(
            m_BuildModeSO.objectsData[selectedObjIndex].Prefab,
            CalculatePivot(m_BuildModeSO.objectsData[selectedObjIndex].occupiedCells),
            m_BuildModeSO.objectsData[selectedObjIndex]
            );
        OnClick += PlaceStructure;
        OnClick -= DestroyObject;
        OnExit += StopPlacement;
        _input.RotatePressed += RotateGrid;
    }

	Vector2I CalculatePivot(Godot.Collections.Array<Vector2I> occupiedCells)
    {
        if (occupiedCells.Count == 0) return Vector2I.Zero;

        int sumX = 0;
        int sumY = 0;
        foreach (var c in occupiedCells)
        {
            sumX += c.X;
            sumY += c.Y;
        }
        // �rednia, zaokr�glona do najbli�szej kom�rki
        return new Vector2I(Mathf.RoundToInt(sumX / (float)occupiedCells.Count),
                          Mathf.RoundToInt(sumY / (float)occupiedCells.Count));
    }

	//Metoda ktora stawia obiekt w scenie na obiekcie Grid.
    private void PlaceStructure()
    {
        metaID++;
        //sprawdzenie czy mysz znajduje sie na UI gracza
        // if (IsPointerOverUI())
        //     return;  
        // // STRAŻNIK: Jeśli indeks jest mniejszy niż 0 lub większy niż rozmiar tablicy, wyjdź z metody!
        if (selectedObjIndex < 0 || selectedObjIndex >= m_BuildModeSO.objectsData.Count)
        {
            GD.Print($"Nie wybrano poprawnego obiektu do postawienia! ID : {selectedObjIndex}");
            return;
        }

        var objData = m_BuildModeSO.objectsData[selectedObjIndex];
        Vector3 MousePosi = GridBuildingSys.GetMapPosition();
        Vector3I gridPos = grid.WorldToCell(MousePosi);
        Vector2I pivot = CalculatePivot(objData.occupiedCells);

        Vector3I pivotWorldCell = gridPos - new Vector3I(pivot.X, 0, pivot.Y);

        //Sprawdzenie czy na wybranym przez gracza kafelku z grida znajduje sie juz obiekt
        bool placementValidity = CheckPlacementValidity(pivotWorldCell, selectedObjIndex);
        if (placementValidity == false)
            return;
        //Dodanie do sceny obiektu wybranego przez gracza na danym kafelku w gridzie
        Node3D buildModeObject = objData.Prefab.Instantiate<Node3D>();
        AddChild(buildModeObject);
        buildModeObject.Position = grid.GetCellCenterWorld(gridPos);
        buildModeObject.Rotation = previewSystem.previewObj.GlobalRotation;
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
            previewSystem.orientation);
            buildModeObject.SetMeta("ID",metaID);
        previewSystem.UpdatePos(grid.GetCellCenterWorld(gridPos), false);
    }

	private bool CheckPlacementValidity(Vector3I gridPos, int selectedObjIndex)
    {
        GridDatabase selectedData = m_BuildModeSO.objectsData[selectedObjIndex].Tag == "Floor" ? ObjectsInSceneData[0] : ObjectsInSceneData[1];
        Godot.Collections.Array<Vector2I> occupiedCells = m_BuildModeSO.objectsData[selectedObjIndex].occupiedCells;
        Vector2I pivot = CalculatePivot(occupiedCells);
        return selectedData.CanPlaceObjectAt(gridPos, m_BuildModeSO.objectsData[selectedObjIndex].occupiedCells, pivot, previewSystem.orientation);
    }
	private void StopPlacement()
    {
        selectedObjIndex = -1;
        gridVisualisation.Visible = false;
        previewSystem.StopShowingPreview();
        OnClick -= PlaceStructure;
        OnClick -= DestroyObject;
        OnExit -= StopPlacement;
        m_NavMeshSurface.BakeNavigationMesh(true);
        _input.RotatePressed -= RotateGrid;
        lastDetectedPos = Vector3I.Zero;
    }

	// public bool IsPointerOverUI()
    //     => EventSystem.current.IsPointerOverGameObject();

	private void RotateGrid(int RotationValue)
    { 
        Vector3 MousePosi = GridBuildingSys.GetMapPosition();
        Vector3I gridPos = grid.WorldToCell(MousePosi);
        Vector2I pivot = CalculatePivot(m_BuildModeSO.objectsData[selectedObjIndex].occupiedCells);
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

        previewSystem.orientation = orientation;
        bool placementValidity = CheckPlacementValidity(pivotWorldCell, selectedObjIndex);
        previewSystem.UpdateRotation(grid.GetCellCenterWorld(gridPos), m_BuildModeSO.objectsData[selectedObjIndex], pivot, placementValidity, rotation);
    }
	private void DestroyObject()
    {
       //if()
    }

}
