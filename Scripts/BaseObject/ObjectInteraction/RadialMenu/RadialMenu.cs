using Godot;
using System;
using System.Collections.Generic;

public partial class RadialMenu : ObjectInteraction
{
    public static RadialMenu Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
		 // Singleton - Tworzę jedną instancję tego gdy powstaje scena. 
		// Singelton wykorzystywany do menu interakcji. 
		// gra będzie posiadała dużo obiektow do interakcji więc stworzenie jednego skryptu dostępnego wszędzie jest wygodne i efektywne. 
        
    }

	//Metoda Odpowiadająca jedynie za Stworzenie Buttona wtedy i tylko wtedy gdy ilość możliwych interakcji jest większa niż ilość buttonow w scenie.
	//  Gdy InstantiatedButtons<ilości opcji obiektu - dotworz kolejne przyciski
	public  void InstantiateButtons(PackedScene interactionButton,List<Interaction> options)
	{
		//Sprawdzamy pierw czy musimy wszystkie Buttony z opcji wyświetlić! 
		//Dzięki temu sprawdzeniu Tworzymy w danym momencie tylko tyle buttonow ile jest potrzebnych do wyświetlenia menu, W przeciwnym razie Kończymy 
		// Ile przycisków aktualnie mamy stworzonych w tym węźle?
		int currentButtons = GetChildCount(); 
	
		// Ile przycisków potrzebujemy dla tej listy opcji?
		int neededButtons = options.Count;
	
		// Jeśli mamy mniej przycisków niż potrzebujemy opcji:
		if (currentButtons < neededButtons)
		{
		    // Obliczamy ile dokładnie nam brakuje
		    int missingButtons = neededButtons - currentButtons;
	
		    // Tworzymy tylko te brakujące
		    for (int i = 0; i < missingButtons; i++)
		    {
		        Button btn = interactionButton.Instantiate() as Button;
		        btn.Visible = false;
		        AddChild(btn); // To samo co GetNode(".").AddChild(btn)
		    }
		}
		

	}

	//Metoda przygotowująca Przyciski do interakcji gracza. 
	// Gdy gracz Kliknie w obiekt ta metoda tworzy nową listę Buttonow z już istniejących przyciskow 
	// by przez jakiś przypadkowy błąd programu Metoda GenerateRadialMenuUI i currentOption.Action?.Invoke();
	// Nie wybrały losowego niekatywnego i bez interakcji buttona do Wizualizacji menu
	List<Button> PrepareButtons(int bCount,List<Interaction> options)
	{
		List<Button> Buttons = new List<Button>();
		for (int i = 0; i < bCount; i++)
		{
			Buttons.Add(GetNode(".").GetChild<Button>(i));

			Button btn = Buttons[i];
			
			foreach (var connection in btn.GetSignalConnectionList(Button.SignalName.Pressed))
        	{
        		btn.Disconnect(Button.SignalName.Pressed, (Callable)connection["callable"]);
        	}


			var currentOption = options[i];
			Buttons[i].Text = options[i].Label;
			Buttons[i].Pressed += () => {
            if (currentOption.SubOptions != null && currentOption.SubOptions.Count > 0) 
            {
                // Wywołujemy to samo menu, ale z nową listą (podmenu)
                GenerateRadialMenuUI(currentOption.SubOptions.Count, btn.GlobalPosition, currentOption.SubOptions);
            }
            else 
            {
                currentOption.Action?.Invoke();
                // Tu możesz dodać CloseDownWindow() jeśli chcesz zamknąć po akcji
            }
        };
			
		}
		return Buttons;
	}

	//Metoda Jedynie tworząca Wizualizację menu interakcji
	//Po przekazaniu Listy Buttonow z metody PrepareButtons Bierzemy
	//Dokładnie te same buttony i je wyświetlamy
	public void GenerateRadialMenuUI( int bCount,Vector2 centerOfMenu,List<Interaction> options)
	{
		foreach (Node child in GetNode(".").GetChildren()) 
            if (child is CanvasItem ci) ci.Visible = false;

		float angleStep = Mathf.Tau / bCount; // Dzielimy pełne koło (Tau) na równe kawałki
		List<Button> SelectedButtons = PrepareButtons(bCount,options);

		for (int i = 0; i < bCount; i++)
		{
		    // Kąt dla obecnego przycisku (0, 90, 180, 270 w radianach)
		    float currentAngle = i * angleStep; 
		    // Obliczamy przesunięcie od środka
		    Vector2 offset = Vector2.FromAngle(currentAngle) * radius;
			SelectedButtons[i].Visible = true;
		    // Ustawiamy przycisk! (odejmujemy połowę jego rozmiaru, żeby idealnie wyśrodkować na punkcie)
		    SelectedButtons[i].Position = centerOfMenu + offset - (SelectedButtons[i].Size / 2f);
		}
	}
}
