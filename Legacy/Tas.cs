using System;
using System.IO;
using UnityEngine;

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
        string inputFilename;
        switch (Application.loadedLevelName)
        {
            case "Level_Menu":
                inputFilename = "tas_script_0.txt";
                break;
            case "Level1_BeatsAndJumps":
                inputFilename = "tas_script_1.txt";
                break;
            case "Level2_SnakesAndLasers":
                inputFilename = "tas_script_2.txt";
                break;
            case "Level3_Scales":
                inputFilename = "tas_script_3.txt";
                break;
            case "Level4_Test":
                inputFilename = "tas_script_4.txt";
                break;
            default:
                return;
        }
        this.inputLines = File.ReadAllLines(inputFilename);
        Debug.Log(string.Format("Tas input file {0} loaded", inputFilename));
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
        // Frame movement or pos?
        if (int.TryParse(array[0], out this.currentTotalFrames))
        {
            this.isPos = false;
            for (int i = 1; i < array.Length; i++)
            {
                switch (array[i].ToLower())
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
        else if (array[0].ToLower() == "pos" && array.Length >= 2)
        {
            this.isPos = true;
            this.toX = float.Parse(array[1]);
            if (this.toX > base.transform.position.x)
            {
                Tas.right = true;
            }
            else if (this.toX < base.transform.position.x)
            {
                Tas.left = true;
            }
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
        GUI.Label(new Rect(50f, (float)(Screen.height - 70), 200f, 200f), this.currentFrameCount.ToString(), this.guiStyle);
        GUI.Label(new Rect(100f, (float)(Screen.height - 70), 200f, 200f), this.currentTotalFrames.ToString(), this.guiStyle);
        if (this.isPos)
        {
            GUI.Label(new Rect(50f, (float)(Screen.height - 50), 200f, 200f), string.Format("To {0}", this.toX), this.guiStyle);
        }
        else
        {
            GUI.Label(new Rect(50f, (float)(Screen.height - 50), 200f, 200f), string.Format("{0} {1} {2}", Tas.left ? "Left " : "", Tas.right ? "Right " : "", Tas.jump ? "Jump " : ""), this.guiStyle);
        }
        GUI.Label(new Rect(50f, (float)(Screen.height - 30), 200f, 200f), string.Format("{0} {1}",
            base.transform.position.x.ToString("0.00"), base.transform.position.y.ToString("0.00")), this.guiStyle);
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
        return (this.isPos && ((Tas.right && base.transform.position.x >= this.toX) || (Tas.left && base.transform.position.x <= this.toX)))
            || this.currentFrameCount == this.currentTotalFrames;
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

    private bool isPos;

    private GUIStyle guiStyle;
}
