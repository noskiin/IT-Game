using Godot;

public partial class VirtualGrid : Node
{
    // Wielkość pojedynczej kratki, którą możesz ustawić w Inspektorze
    [Export] 
    public Vector3 CellSize = new Vector3(0.5f, 0.5f, 0.5f);

    // Odpowiednik Unity: grid.WorldToCell()
    public Vector3I WorldToCell(Vector3 worldPosition)
    {
        return new Vector3I(
            Mathf.FloorToInt(worldPosition.X / CellSize.X),
            Mathf.FloorToInt(worldPosition.Y / CellSize.Y),
            Mathf.FloorToInt(worldPosition.Z / CellSize.Z)
        );
    }

    // Odpowiednik Unity: grid.CellToWorld()
    public Vector3 CellToWorld(Vector3I cellPosition)
    {
        return new Vector3(
            cellPosition.X * CellSize.X,
            cellPosition.Y * CellSize.Y,
            cellPosition.Z * CellSize.Z
        );
    }

    // Odpowiednik Unity: grid.GetCellCenterWorld()
    // Powoduje ,że biekt ląduje na środku kratki
    public Vector3 GetCellCenterWorld(Vector3I cellPosition)
    {
        return CellToWorld(cellPosition) + (CellSize / 2.0f);
    }
}