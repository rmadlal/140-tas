using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tas : MonoBehaviour
{
    private void Awake()
    {
        this.player = base.gameObject.GetComponent<MyCharacterController>();
        this.guiStyle = new GUIStyle();
        this.guiStyle.fontStyle = FontStyle.Bold;
        this.guiStyle.fontSize = 16;
        this.guiStyle.normal.textColor = Color.white;
    }

    private void Start()
    {
        this.StartTas();
    }

    private void Update()
    {
        if (Event.current.shift && Input.GetKeyDown(KeyCode.F5))
        {
            this.StartTas();
        }

        if (Event.current.shift && Input.GetKeyDown(KeyCode.F6))
        {
            this.StopTas();
        }

        if (Event.current.shift && Input.GetKeyDown(KeyCode.F7))
        {
            Tas.showGUI = !Tas.showGUI;
        }
    }

    private void StartTas()
    {
        this.ResetState();
        this.InitActionQueue();
        this.isRunning = true;
    }

    private void StopTas()
    {
        this.ResetState();
        this.isRunning = false;
    }

    private void InitActionQueue()
    {
        this.ActionQueue = new Queue<TasAction>();

        string inputFilename = Application.loadedLevelName + ".txt";
        string[] inputLines = File.ReadAllLines(inputFilename);
        Debug.Log(string.Format("Tas input file {0} loaded", inputFilename));

        foreach (string line in inputLines)
        {
            TasAction action = ParseActionLine(line);
            if (action != null)
            {
                ActionQueue.Enqueue(action);
            }
        }

        Debug.Log("Finished parsing tas script");
    }

    private TasAction ParseActionLine(string line)
    {
        string command = line.Trim().ToLowerInvariant();
        while (string.IsNullOrEmpty(command) || command.StartsWith("#"))
        {
            return null;
        }

        string[] parts = command.Split(',');

        TasAction action = new TasAction();
        if (int.TryParse(parts[0], out action.RemainingFrames))
        {
            action.IsPosition = false;
            foreach (string part in parts)
            {
                switch (part)
                {
                    case "left":
                        action.Left = true;
                        break;
                    case "right":
                        action.Right = true;
                        break;
                    case "jump":
                        action.Jump = true;
                        break;
                }
            }
        }
        else if (parts[0] == "pos" && parts.Length >= 2)
        {
            action.IsPosition = true;
            action.TargetX = float.Parse(parts[1]);
        }

        return action;
    }

    public void UpdateTas()
    {
        if (!this.isRunning || this.player == null || this.player.IsControlPaused() || this.player.visualPlayer.StateIsLocked())
        {
            return;
        }

        // Gate activated - resume from next line
        if (this.player.IsForceMoveActive())
        {
            this.ResetInputs();
            return;
        }

        if (this.IsCurrentActionFinished())
        {
            if (this.ActionQueue.Count == 0)
            {
                this.StopTas();
                return;
            }

            this.ResetInputs();
            Tas.Action = ActionQueue.Dequeue();

            if (Tas.Action.IsPosition)
            {
                if (Tas.Action.TargetX > base.transform.position.x)
                {
                    Tas.Action.Right = true;
                }
                else if (Tas.Action.TargetX < base.transform.position.x)
                {
                    Tas.Action.Left = true;
                }
            }
        }

        // TODO If there is an off-by-one error, try moving this to the top
        Tas.Action.RemainingFrames--;
    }

    public void ResetState()
    {
        this.ActionQueue.Clear();
        this.ResetInputs();
    }

    private void ResetInputs()
    {
        Tas.Action = new TasAction();
    }

    private bool IsCurrentActionFinished()
    {
        if (Tas.Action.IsPosition)
        {
            return (Tas.Action.Right && base.transform.position.x >= Tas.Action.TargetX)
                || (Tas.Action.Left && base.transform.position.x <= Tas.Action.TargetX);
        }

        return Tas.Action.RemainingFrames <= 0;
    }

    private void OnGUI()
    {
        if (!Tas.showGUI)
        {
            return;
        }

        GUI.Label(new Rect(50f, (float)(Screen.height - 70), 200f, 200f), Tas.Action.RemainingFrames.ToString(), this.guiStyle);

        string currentAction = Tas.Action.ToString();
        GUI.Label(new Rect(50f, (float)(Screen.height - 50), 200f, 200f), currentAction, this.guiStyle);

        string position = string.Format("{0} {1}", base.transform.position.x.ToString("0.00"), base.transform.position.y.ToString("0.00"));
        GUI.Label(new Rect(50f, (float)(Screen.height - 30), 200f, 200f), position, this.guiStyle);
    }

    private void Destroy()
    {
        ActionQueue = null;
        Tas.Action = null;
    }

    public static TasAction Action;

    private static bool showGUI;

    private Queue<TasAction> ActionQueue;

    private bool isRunning;

    private MyCharacterController player;

    private GUIStyle guiStyle;
}
