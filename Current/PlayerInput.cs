using System;
using InControl;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public static bool GetKeyJumpDown()
    {
        return PlayerInput.lockedJump
            || Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.W)
            || Tas.Action.Jump // Added
            || (InputManager.ActiveDevice != null && InputManager.ActiveDevice.Action1.WasPressed);
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

        float num = 0f;
        if (InputManager.ActiveDevice != null)
        {
            if (InputManager.ActiveDevice.DPadLeft.IsPressed)
            {
                num = -1f;
            }
            else if (InputManager.ActiveDevice.DPadRight.IsPressed)
            {
                num = 1f;
            }
            else
            {
                num = InputManager.ActiveDevice.LeftStickX.Value;
            }
        }

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

        // Modified: Extracted variables for left/right keys and added TAS
        bool isLeftPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Tas.Action.Left;
        bool isRightPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Tas.Action.Right;

        if (isLeftPressed && isRightPressed) // modified
        {
            if (PlayerInput.leftTimeStamp < PlayerInput.rightTimeStamp)
            {
                num = -1f;
            }
            else
            {
                num = 1f;
            }
        }
        else if (isLeftPressed) // modified
        {
            num = -1f;
            PlayerInput.leftTimeStamp = Time.time;
        }
        else if (isRightPressed) // modified
        {
            num = 1f;
            PlayerInput.rightTimeStamp = Time.time;
        }

        float num2 = 1f;
        if (Globals.Camera.GetComponent<CameraScript>().IsCameraMirrored())
        {
            num2 = -1f;
        }
        return num * num2;
    }
}
