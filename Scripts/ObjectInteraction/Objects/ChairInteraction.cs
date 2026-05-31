using Godot;
using System;
using System.Collections.Generic;


public partial class Chair : BaseInteractionClass
{
	public override List<Interaction> GetInteractions()
	{
	    return new List<Interaction>
	    {

				new Interaction("Przesuń", () => MoveChair()),
				new Interaction("Usiądź i Jedz", () => SitDownAndEat()),
				new Interaction("Wyrzuć przez okno", () => ThrowOutWindow()),
								new Interaction("Usiądź", subOptions: new List<Interaction>
				{
					new Interaction("Przesuń", () => SitDown()),
					new Interaction("Usiądź i Jedz", subOptions: new List<Interaction>
					{
						new Interaction("Przesuń", () => SitDown()),
						new Interaction("Usiądź i Jedz", () => SitDownAndEat()),
					})
				}),
	    };
	}


	private void SitDown() { GD.Print("Siadam..."); }
	private void MoveChair() { GD.Print("A"); }
	private void SitDownAndEat() { GD.Print("B..."); }
	private void ThrowOutWindow() { GD.Print("C..."); }


}
