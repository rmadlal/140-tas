using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

	public static bool GetKeyJumpDown()
	{
		return PlayerInput.lockedJump || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetButtonDown("Jump") || Tas.jump; // Modified line (Tas.jump)
	}

	public static float GetHorizontalStick()
	{
		if (PlayerInput.lockedLeft)
		{
			return -1f;
		}
		if (PlayerInput.lockedRight)
		{
			return 1f;
		}
		float num = Input.GetAxis("Horizontal");
		if (num > 0.4f)
		{
			num = 1f;
		}
		else if (num < -0.4f)
		{
			num = -1f;
		}
		else
		{
			num = 0f;
		}
		if (Input.GetKey("left") || Input.GetButton("Left") || Tas.left) // Modified line (Tas.left)
		{
			num = -1f;
		}
		else if (Input.GetKey("right") || Input.GetButton("Right") || Tas.right) // Modified line (Tas.right)
		{
			num = 1f;
		}
		float num2 = 1f;
		if (Globals.Camera.GetComponent<CameraScript>().IsCameraMirrored())
		{
			num2 = -1f;
		}
		return num * num2;
	}
}
