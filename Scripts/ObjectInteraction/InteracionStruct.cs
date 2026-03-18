using Godot;
using System;
using System.Collections.Generic;

public class Interaction
{
    public string Label;      // Napis na przycisku (np. "Usiądź")
    public Action Action;     // Co się ma stać (metoda do wywołania)
    public List<Interaction> SubOptions;
    public Interaction(string label, Action action = null,List<Interaction> subOptions = null)
    {
        Label = label;
        Action = action;
        SubOptions = subOptions;
    }
}