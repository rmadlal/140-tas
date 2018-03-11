using System;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : Savepoint
{

	private void StartState()
	{
		if (this.started)
		{
			this.startPressed = true;
			this.colorChangesTerminated = true;
			this.logoColorChangesTerminated = true;
		}
	}
}
