using System;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : GroundbasedController, IBulletReceiver, IBulletOwner
{

	private new void Start()
	{
		base.Start();
		Globals.levelsManager != null;
		this.facingGfxDist = Mathf.Abs(this.facingGfx.transform.localPosition.x);
		this.bulletGfxDist1 = Mathf.Abs(this.facingGfx.transform.GetChild(0).transform.localPosition.x);
		this.bulletGfxDist2 = Mathf.Abs(this.facingGfx.transform.GetChild(1).transform.localPosition.x);
		this.facingGfxStartColor = this.facingGfx.renderer.material.color;
		this.tas = base.gameObject.AddComponent<Tas>(); // Added line
		this.tas.enabled = true; // Added line
		this.UpdateFacing();
	}

	private new void FixedUpdate()
	{
		if (!this.logicPaused)
		{
			this.tas.UpdateTas(); // Added line
			this.UpdateHorizontalStick();
			this.UpdateMove();
			if (!this.controlPaused)
			{
				this.UpdateFacing();
			}
			this.UpdateJump();
			if (this.hasSlowMotionAbility)
			{
				this.UpdateSlowMoState();
			}
		}
		if (this.hasSlowMotionAbility)
		{
			this.UpdateSlowMoManipulation();
		}
		this.jumpButtonPressed = false;
		this.shootButtonDown = false;
		this.altShootButtonDown = false;
		base.FixedUpdate();
	}

	// New method
	public bool IsControlPaused()
	{
		return this.controlPaused;
	}

	private Tas tas; // Added line
}
