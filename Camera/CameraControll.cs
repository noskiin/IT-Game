using Godot;
using System;

public partial class CameraControll : GameMaster
{
	// Called when the node enters the scene tree for the first time.
	private Camera3D cam;

    [Export] 
    private Node3D Target;

    private Vector2 _mouseDelta;
    float rotationSpeed = 1f/10f;

    private bool OrtographicCam = true;
    private float OrtoSize = 90f, TppSize = 45f, newOrtoSize;

    float orthographicSize = 20f;
    float distance = 20f;
    float minVerticalAngle = -90f, maxVerticalAngle = -10f;

    Vector3 orbitAngles = new Vector3(38f, -29f,0f);
	Vector3 currentPOV;

    private Vector3 currentOrtographicCameraPosition;
    private Vector3 currentTPPCameraPosition;

	public override void _Ready()
	{       
		cam = GetViewport().GetCamera3D();
		GD.Print(cam.Position);
        GD.Print(Target.Position);

        newOrtoSize = OrtoSize;
        cam.Quaternion = Quaternion.FromEuler(orbitAngles);
        currentOrtographicCameraPosition = new Vector3(0f, 0f,0f);
        currentTPPCameraPosition = new Vector3 (Target.GlobalPosition.X , -2f, Target.GlobalPosition.Z);
        cam.Position = currentOrtographicCameraPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Toggle_Orto") && OrtographicCam)
        {
            OrtographicCam = false;
            currentPOV = currentTPPCameraPosition; 
            newOrtoSize = TppSize;
            FocusPointUpdate(currentPOV,delta);       
        }else if (Input.IsActionJustPressed("Toggle_Orto") && !OrtographicCam)
        {
            OrtographicCam = true;
            currentPOV = currentOrtographicCameraPosition;
            newOrtoSize = OrtoSize;
            FocusPointUpdate(currentPOV,delta);

        }

        Quaternion lookRotation;
        if (ManualRotation())
        {
            ConstrainAngles();
            Vector3 radiansAngles = new Vector3(
                Mathf.DegToRad(orbitAngles.X), 
                Mathf.DegToRad(orbitAngles.Y), 
                0f
            );
            lookRotation = Quaternion.FromEuler(radiansAngles);
        }
        else {
            lookRotation = cam.Quaternion;
        }
        Vector3 lookPosition = FocusPointUpdate(currentPOV,delta);
        cam.Position = lookPosition;
        cam.Quaternion = lookRotation;

    }

    Vector3 FocusPointUpdate(Vector3 focusPoint,double Delta)
    {
        

        Vector3 lookDirection = cam.GlobalTransform.Basis * Vector3.Forward;

        float ZoomInTIme = 3f;
        float journeyTime = 5f;
        double cameraSmoothnes = Delta * journeyTime;
        double ZoomInSmoothnes = Delta * ZoomInTIme;


        cam.Fov = (float)Mathf.Lerp(cam.Fov, newOrtoSize, ZoomInSmoothnes);


        
        return cam.Position.Slerp(focusPoint - lookDirection * distance , (float)cameraSmoothnes);;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Sprawdzamy, czy zdarzenie to ruch myszką
        if (@event is InputEventMouseMotion mouseMotion)
        {
            _mouseDelta = mouseMotion.Relative;

        }
    
    }


    bool ManualRotation()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            // Pobieramy nasz ruch i zamieniamy osie Y i X (tak jak chciałeś)
            Vector3 input = new Vector3(_mouseDelta.Y, _mouseDelta.X,0f);

            if (!input.IsZeroApprox())
            {
                orbitAngles += (rotationSpeed) * input;
            }

            _mouseDelta = Vector2.Zero; 

            return true;
        }

        _mouseDelta = Vector2.Zero;
        return false;
    } 


    void ConstrainAngles()
    {
        orbitAngles.X =
            Mathf.Clamp(orbitAngles.X, minVerticalAngle, maxVerticalAngle); 
        orbitAngles.Y = Mathf.Wrap(orbitAngles.Y, 0f, 360f);
    }  
    
}



