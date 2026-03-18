using Godot;
using System;
using System.Collections.Generic;


public partial class GridDatabase
{
	public Dictionary<Vector3I, PlacementData> placedObjects = new();
    public void AddObjectAt(
        Vector3I gridPosition, // POZYCJA PIVOTU W �WIECIE (grid cell)
        int ID,
        int placedObjectIndeX,
        int mainObjectIndeX,
        Godot.Collections.Array<Vector2I> occupiedCells,
        Vector2I pivot, // pivot w przestrzeni lokalnej kszta�tu (2D)
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

        //Przyklad wyciaganbia ddanych z tej bazy danych
/*        foreach (var pos in placedObjects) { 
            Debug.Log( "IndeX objektu w tej bazie danych to : " + pos.Value.placedObjectIndeX +", a IndeX obiektu to :" + pos.Value.mainObjectIndeX);
            Debug.Log(pos.Key);
        }*/
    }


    private Godot.Collections.Array<Vector3I> CalculatePositions_CustomShapes(Vector3I gridPosition, Godot.Collections.Array<Vector2I> occupiedCells, Vector2I pivot, int orientation)
    {
        
        Godot.Collections.Array<Vector3I> returnVal = new();
        foreach (var pos in occupiedCells)
        {
            // 1) Przesuni�cie punktu wzgl�dem pivotu (ustawiamy pivot jako punkt odniesienia)
         //    Dzi�ki temu obracamy punkt wok� pivotu, a nie wok� (0,0).
            Vector2I relative = pos - pivot;
            // 2) Obr�t punktu RELATYWNEGO (teraz obracamy punkt wok� (0,0),
            //    ale poniewa� wcze�niej odj�li�my pivot, to w rzeczywisto�ci
            //    obracamy wok� pivotu).
            Vector2I rotated = RotateVector(orientation,relative);
             // 3) Finalna pozycja w gridzie:
        //    pivotWorldPosition + rotated -> da nam w�a�ciwe miejsce kafelka po obrocie.
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