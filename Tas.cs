using System;
using System.IO;
using UnityEngine;

public class Tas : MonoBehaviour
{
    private void Awake()
    {
        this.player = base.gameObject.GetComponent<MyCharacterController>();
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
        Set frame count and movement flags according to the next input line.
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
		this.currentTotalFrames = int.Parse(array[0]);
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
		if (this.currentFrameCount == this.currentTotalFrames)
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
		GUI.Label(new Rect(50f, (float)(Screen.height - 70), 200f, 200f), this.currentTotalFrames.ToString());
		GUI.Label(new Rect(100f, (float)(Screen.height - 70), 200f, 200f), this.currentFrameCount.ToString());
		GUI.Label(new Rect(50f, (float)(Screen.height - 50), 200f, 200f), string.Format("{0} {1} {2}", Tas.left ? "Left " : "", Tas.right ? "Right " : "", Tas.jump ? "Jump " : ""));
		GUI.Label(new Rect(50f, (float)(Screen.height - 30), 200f, 200f), string.Format("{0:0.00} {1:0.00}", base.transform.position.x.ToString(), base.transform.position.y.ToString()));
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
}
