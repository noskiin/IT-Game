using Godot;
using System;
using System.Collections.Generic;


public partial class GridDatabase
{
	public Dictionary<Vector3I, PlacementData> placedObjects = new();

    public Dictionary<Vector3I, List<SnapPointData>> SnapPointCells = new();
    public void AddObjectAt(
        Vector3I gridPosition,
        int ID,
        int placedObjectIndeX,
        int mainObjectIndeX,
        Godot.Collections.Array<Vector2I> occupiedCells,
        Vector2I pivot, // pivot w przestrzeni lokalnej ksztaltu (2D)
        int orientation) // 0..3 (North,East,South,West)

    {
        Godot.Collections.Array<Vector3I> positionToOccupy = CalculatePositions_CustomShapes(gridPosition,occupiedCells,pivot,orientation);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndeX, mainObjectIndeX);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary Already contains{pos}");
            placedObjects[pos] = data;

        }

        //Przyklad wyciaganbia ddanych z tej bazy danych zostawiony w razie w
        /*        foreach (var pos in placedObjects) { 
            Debug.Log( "IndeX objektu w tej bazie danych to : " + pos.Value.placedObjectIndeX +", a IndeX obiektu to :" + pos.Value.mainObjectIndeX);
            Debug.Log(pos.Key);
        }*/




    }



    private Vector3I CalculateSinglePosition(Vector3I gridPosition, Vector2I localPos, Vector2I pivot, int orientation)
    {
        Vector2I relative = localPos - pivot;
        Vector2I rotated = RotateVector(orientation, relative);
        return gridPosition + new Vector3I(rotated.X, 0, rotated.Y);
    }

    private Godot.Collections.Array<Vector3I> CalculatePositions_CustomShapes(Vector3I gridPosition, Godot.Collections.Array<Vector2I> occupiedCells, Vector2I pivot, int orientation)
    {
        
        Godot.Collections.Array<Vector3I> returnVal = new();
        foreach (var pos in occupiedCells)
        {
            // 1) Przesuniecie punktu wzgledem pivotu (ustawiamy pivot jako punkt odniesienia)
         //    Dzieki temu obracamy punkt wokol pivotu, a nie wokol (0,0).
            Vector2I relative = pos - pivot;
            // 2) Obrot punktu RELATYWNEGO (teraz obracamy punkt wokol (0,0),
            //    ale poniewaz wczesniej odjelismy pivot, to w rzeczywistosci
            //    obracamy wokol pivotu).
            Vector2I rotated = RotateVector(orientation,relative);
             // 3) Finalna pozycja w gridzie:
        //    pivotWorldPosition + rotated -> da nam wlasciowe miejsce kafelka po obrocie.
            Vector3I finalPos = gridPosition + new Vector3I(rotated.X,0,rotated.Y);
            returnVal.Add(finalPos);
        }
        return  returnVal;
    }


    private Vector2I RotateVector(int orientation, Vector2I offset)
    {
        switch (orientation)
        {
            case 0: // 0�
                return new Vector2I(offset.X, offset.Y); ;
            case 1: // 90�: (X,z) -> (z, -X)
                return new Vector2I(offset.Y, -offset.X);
            case 2: // 180�: (X,z) -> (-X, -z)
                return new Vector2I(-offset.X, -offset.Y);
            case 3: // 270�: (X,z) -> (-z, X)
                return new Vector2I(-offset.Y, offset.X);
            default:
                return offset;
        }

    }

    public bool CanPlaceObjectAt(Vector3I gridPosition, Godot.Collections.Array<Vector2I> occupiedCells, Vector2I pivot, int orientation)
    {
        Godot.Collections.Array<Vector3I> positionToOccupy = CalculatePositions_CustomShapes(gridPosition,occupiedCells,pivot,orientation);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    public bool TryFindValidSnapPoint(Vector3I checkPosition, SnapType typeLookingFor, out SnapPointData validSnapPoint)
    {
        validSnapPoint = null;

        // 1. Sprawdź, czy w ogóle istnieją jakieś snap pointy w tej komórce
        if (SnapPointCells.TryGetValue(checkPosition, out List<SnapPointData> pointsInCell))
        {
            // 2. Przeszukaj listę w poszukiwaniu dopasowania
            foreach (var point in pointsInCell)
            {
                // Czy ten punkt udostępnia to, czego szuka nasz obiekt?
                // (np. Wazon szuka TableSurface, a punkt na stole oferuje TableSurface)
                if (point.ProvidesType == typeLookingFor)
                {
                    validSnapPoint = point;
                    return true;
                }
            }
        }
        return false;
    }
}


public class PlacementData
{
    public Godot.Collections.Array<Vector3I> occupiedPositions;
    public int ID { get; set; }
    public int placedObjectIndeX { get; set; }

    public int mainObjectIndeX { get; set; }
    public PlacementData(Godot.Collections.Array<Vector3I> occupiedPositions, int iD, int placedObjectIndeX, int mainObjectIndeX)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        this.placedObjectIndeX = placedObjectIndeX;
        this.mainObjectIndeX = mainObjectIndeX;
    }
}


public class SnapPointData
{
    public Vector3I position;          
    public int ID { get; set; }
    public int placedObjectIndex { get; set; }
    public int mainObjectIndex { get; set; }
    public SnapType ProvidesType { get; set; }
    public SnapType AcceptsType { get; set; }

    public SnapPointData(Vector3I position, int iD, int placedObjectIndex, int mainObjectIndex, SnapType provides, SnapType accepts)
    {
        this.position = position;      
        ID = iD;
        this.placedObjectIndex = placedObjectIndex;
        this.mainObjectIndex = mainObjectIndex;
        ProvidesType = provides;
        AcceptsType = accepts;
    }
}

public enum SnapType
{
    None = 0,
    TableSurface = 1,   // Stół to udostępnia, wazon tego szuka
    ChairSlot = 2       // Stół to udostępnia przy krawędzi, krzesło tego szuka
}