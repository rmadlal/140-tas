using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tas : MonoBehaviour
{
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
        string script = Tas.scripts[SceneManager.GetActiveScene().name];
        this.inputLines = File.ReadAllLines(script);
        Debug.Log(string.Format("Tas input file {0} loaded", script));
        this.ResetState(true);
        this.isRunning = true;
    }

    private void StopTas()
    {
        this.ResetState(true);
        this.isRunning = false;
    }

    /*
    Set frame count and movement flags or position destination according to the next input line.
    Empty lines or line comments are skipped until a valid line is reached.
    Tas execution stops if end of file is reached.
    */
    private void ProcessNextLine()
    {
        string text = this.inputLines[this.currentLineIdx].Trim();
        while (string.IsNullOrEmpty(text) || text.StartsWith("#"))
        {
            this.currentLineIdx++;
            if (this.currentLineIdx == this.inputLines.Length)
            {
                this.StopTas();
                return;
            }
            text = this.inputLines[this.currentLineIdx].Trim();
        }
        string[] array = text.Split(',');

        // Command type?
        if (int.TryParse(array[0], out this.currentTotalFrames))
        {
            this.cmdType = Tas.CmdType.RegularInput;
        }
        else if (array[0].ToLower() == "pos" && array.Length >= 2)
        {
            this.cmdType = Tas.CmdType.MoveTo;
        }
        else if (array[0].ToLower() == "setpos" && array.Length >= 3)
        {
            this.cmdType = Tas.CmdType.SetPos;
        }
        else
            return;

        // Execute command
        switch (this.cmdType)
        {
        case Tas.CmdType.RegularInput:
            this.RegisterInputs(array, 1);
            return;
        case Tas.CmdType.MoveTo:
            this.toX = float.Parse(array[1]);
            if (this.toX > base.transform.position.x)
            {
                Tas.right = true;
                return;
            }
            if (this.toX < base.transform.position.x)
            {
                Tas.left = true;
                return;
            }
            return;
        case Tas.CmdType.SetPos:
            float x = float.Parse(array[1]);
            float y = float.Parse(array[2]);
            base.transform.position = new Vector3(x, y, base.transform.position.z);
            this.RegisterInputs(array, 3);
            return;
        default:
            return;
        }
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
            this.ResetState(false);
            return;
        }
        if (this.IsCurrentLineDone())
        {
            this.currentLineIdx++;
            if (this.currentLineIdx == this.inputLines.Length)
            {
                this.StopTas();
                return;
            }
            this.ResetInputs();
            this.ProcessNextLine();
        }
        this.currentFrameCount++;
    }

    private void ResetInputs()
    {
        Tas.left = false;
        Tas.right = false;
        Tas.jump = false;
        this.currentTotalFrames = 0;
        this.currentFrameCount = 0;
    }

    private void OnGUI()
    {
        if (!Tas.showGUI)
        {
            return;
        }
        GUI.Label(new Rect(50f, (float)(Screen.height - 90), 200f, 200f), this.currentFrameCount.ToString(), this.guiStyle);
        GUI.Label(new Rect(100f, (float)(Screen.height - 90), 200f, 200f), this.currentTotalFrames.ToString(), this.guiStyle);
        if (this.isPos)
        {
            GUI.Label(new Rect(50f, (float)(Screen.height - 70), 200f, 200f), string.Format("To {0}", this.toX), this.guiStyle);
        }
        else
        {
            GUI.Label(new Rect(50f, (float)(Screen.height - 70), 200f, 200f), string.Format("{0} {1} {2}", Tas.left ? "Left " : "", Tas.right ? "Right " : "", Tas.jump ? "Jump " : ""), this.guiStyle);
        }
        GUI.Label(new Rect(50f, (float)(Screen.height - 50), 200f, 200f),
            string.Format("x: {0}", base.transform.position.x), this.guiStyle);
        GUI.Label(new Rect(50f, (float)(Screen.height - 30), 200f, 200f),
            string.Format("y: {0}", base.transform.position.y), this.guiStyle);
    }

    private void Destroy()
    {
        this.ResetInputs();
    }

    public void ResetState(bool restartScript)
    {
        if (restartScript)
        {
            this.currentLineIdx = -1;
        }
        this.ResetInputs();
    }

    private void Awake()
    {
        this.player = base.gameObject.GetComponent<MyCharacterController>();
        this.guiStyle = new GUIStyle();
        this.guiStyle.fontStyle = FontStyle.Bold;
        this.guiStyle.fontSize = 16;
        this.guiStyle.normal.textColor = Color.white;
    }

    private bool IsCurrentLineDone()
    {
        return this.cmdType == Tas.CmdType.SetPos
            || (this.cmdType == Tas.CmdType.MoveTo && ((Tas.right && base.transform.position.x >= this.toX) || (Tas.left && base.transform.position.x <= this.toX)))
            || this.currentFrameCount == this.currentTotalFrames;
    }

    private void RegisterInputs(string[] line, int from)
    {
        for (int i = from; i < line.Length; i++)
        {
            switch (line[i].ToLower())
            {
            case "left":
                Tas.left = true;
                break;
            case "right":
                Tas.right = true;
                break;
            case "jump":
                Tas.jump = true;
                break;
            default:
                return;
            }
        }
    }

    private bool isRunning;

    private string[] inputLines;

    private int currentLineIdx;

    public static bool left;

    public static bool right;

    public static bool jump;

    private int currentFrameCount;

    private static bool showGUI;

    private int currentTotalFrames;

    private MyCharacterController player;

    private float toX;

    private GUIStyle guiStyle;

    private static Dictionary<string, string> scripts = new Dictionary<string, string>
    {
        {
            "Level_Menu",
            "tas_script_0.txt"
        },
        {
            "Level1_BeatsAndJumps",
            "tas_script_1.txt"
        },
        {
            "Level2_SnakesAndLasers",
            "tas_script_2.txt"
        },
        {
            "Level3_Scales",
            "tas_script_3.txt"
        },
        {
            "Level4_Test",
            "tas_script_4.txt"
        }
    };

    private Tas.CmdType cmdType;

    private enum CmdType
    {
        RegularInput,
        MoveTo,
        SetPos
    }
}
