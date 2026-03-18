using Godot;
using System;

public partial class GridBuildingSystem : Node
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private Vector2 _mouseDelta;

    private Camera3D cam;

    private Vector3 lastPosition;

    private uint mask = Layers.BuildingLayer;
	[Export]
    private VirtualGrid grid;
	
	public override void _Ready()
    {
        cam = GetViewport().GetCamera3D();
    }

	public Vector3 GetMapPosition()
    {
        // 1. Pobierz pozycję myszy na ekranie w 2D
        Vector2 mousePos = GetViewport().GetMousePosition();

        // 2. Oblicz skąd promień startuje (nearClipPlane) i w jakim kierunku leci
        Vector3 rayOrigin = cam.ProjectRayOrigin(mousePos);
        Vector3 rayDirection = cam.ProjectRayNormal(mousePos);
        
        // Oblicz punkt końcowy promienia (odpowiednik maxDistance = 200 w Unity)
        Vector3 rayEnd = rayOrigin + rayDirection * 200.0f;

        // 3. Uzyskaj dostęp do stanu fizyki świata 3D
        PhysicsDirectSpaceState3D spaceState = GetViewport().World3D.DirectSpaceState;

        // 4. Stwórz parametry zapytania (promień od - do, plus maska kolizji)
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd, mask);

        // 5. Wystrzel promień
        Godot.Collections.Dictionary hit = spaceState.IntersectRay(query);

        // Jeżeli trafiliśmy, słownik będzie zawierał dane
        if (hit.Count > 0)
        {
            // Pobieramy "position" ze słownika i rzutujemy Variant na Vector3
            lastPosition = (Vector3)hit["position"];
			GD.Print(lastPosition);
        }

        return lastPosition;
    }

}




public static class Layers
{
    public const uint None = 0;
    
    // Zwróć uwagę: w kodzie liczymy od 0, więc Warstwa 1 w edytorze to bit 0!
    // Literka 'u' oznacza, że to jest typ uint.
    public const uint Player = 1u << 0;  // Warstwa 1 w edytorze
    public const uint Enemy  = 1u << 1;  // Warstwa 2 w edytorze
    public const uint Wall   = 1u << 2;  // Warstwa 3 w edytorze
    public const uint BuildingLayer = 1u << 3;  // Warstwa 4 w edytorze
    
    public const uint All = ~0u;         // Skrót na "Wszystkie warstwy" (same jedynki)
}
