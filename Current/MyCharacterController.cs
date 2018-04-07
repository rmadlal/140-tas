using System;
using System.Collections.Generic;
using SpeedrunTimerMod;
using UnityEngine;

public class MyCharacterController : GroundbasedController
{

    private new void Start()
    {
        base.Start();
        this.activeFloatZones = new IFloatPlayer[10];
        this.rigidBody2D = base.GetComponent<Rigidbody2D>();
        this.extraBoxColl = this.colliderObject.GetComponent<BoxCollider2D>();
        Globals.levelsManager != null;
        if (this.facingGfx)
        {
            this.facingGfxDist = Mathf.Abs(this.facingGfx.transform.localPosition.x);
        }
        this.tas = base.gameObject.AddComponent<Tas>(); // Added line
        this.tas.enabled = true; // Added line
        this.UpdateFacing();
    }

    private void upDATE_FixedUpdate()
    {
        this.timeSinceKeyCollected += Time.deltaTime;
        this.timeSinceOnMover += Time.deltaTime;
        if (this.physicsPaused)
        {
            return;
        }
        if (!this.logicPaused || (this.isDead && !MirrorModeManager.mirrorModeActive))
        {
            Vector3 vector = new Vector3(0f, 0f, -10f);
            vector.y = this.visualPlayer.transform.localPosition.y;
            if (!this.FloatZoneIsActive())
            {
                vector.y = Mathf.MoveTowards(vector.y, -0.088f, Time.deltaTime);
            }
            else
            {
                vector.y = Mathf.MoveTowards(vector.y, -0.0625f, Time.deltaTime);
            }
            this.visualPlayer.transform.localPosition = vector;
        }
        this.noOfGroundCollisions = 0;
        this.bestDot = 0f;
        this.bestGroundNormal = Vector3.zero;
        this.groundBody = null;
        this.noOfCeilingCollisions = 0;
        this.verticalResolveCollIndex = -1;
        this.swapperTouched = null;
        if (!this.isDead)
        {
            base.CollectCollisions(1);
        }
        this.noOfLeftWallCollisions = 0;
        this.noOfRightWallCollisions = 0;
        if (!this.isDead)
        {
            base.CollectCollisions(2);
        }
        Vector3 position = base.transform.position;
        if (this.noOfGroundCollisions > 0)
        {
            this.groundTimer = this.groundThreshold;
        }
        else
        {
            this.groundTimer -= Time.deltaTime;
        }
        if (!this.logicPaused)
        {
            this.tas.UpdateTas(); // Added line
            this.UpdateHorizontalStick();
            this.UpdateJump();
            this.UpdateMove();
            if (!this.controlPaused)
            {
                this.UpdateFacing();
            }
            if (this.hasSlowMotionAbility)
            {
                this.UpdateSlowMoState();
            }
        }
        this.preMovePos = base.transform.position;
        if (this.hasSlowMotionAbility)
        {
            this.UpdateSlowMoManipulation();
        }
        this.jumpButtonPressed = false;
        this.shootButtonDown = false;
        this.altShootButtonDown = false;
        if (this.moveDirection.x > 0f && this.noOfRightWallCollisions > 0)
        {
            this.moveDirection.x = 0f;
        }
        if (this.moveDirection.x < 0f && this.noOfLeftWallCollisions > 0)
        {
            this.moveDirection.x = 0f;
        }
        this.wallResolvedInIndex3Phase = false;
        if (!this.isDead)
        {
            base.CollectCollisionsSafeFloor(position);
            base.CollectCollisions(3);
        }
        if (this.groundBody && this.jumpSetOffTimer <= 0.1f)
        {
            this.groundBody.pub_playerOnMover = true;
            this.timeSinceOnMover = 0f;
        }
        if (this.noOfGroundCollisions > 0 && !this.wallResolvedInIndex3Phase)
        {
            this.groundTimer = this.groundThreshold;
        }
        if (this.groundBody == null && this.jumpSetOffTimer <= 0.1f)
        {
            if (this.pub_GroundVel.y < -0.0001f)
            {
                this.moveDirection.y = this.pub_GroundVel.y;
            }
            else if (this.pub_GroundVel.y > 0.0001f)
            {
                this.moveDirection.y = this.pub_GroundVel.y * 0.5f;
            }
        }
        if (this.groundBody != null)
        {
            this.lastGroundBody = this.groundBody;
        }
        this.pub_GroundVel = Vector3.zero;
    }

    // New method
    public bool IsControlPaused()
    {
        return this.controlPaused;
    }

    private Tas tas; // Added line
}
