using System;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : Savepoint
{

	private void StartState()
	{
		if (this.started) // Modified line - starts the game on the first possible frame (as if by holding space)
		{
			this.startPressed = true;
			this.colorChangesTerminated = true;
			this.logoColorChangesTerminated = true;
		}
	}
}
