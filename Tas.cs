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
        switch (Application.loadedLevelName) {
            case "Level_Menu"":
                inputFilename = "tas_script_0.txt";
                break;
            case "Level1_BeatsAndJumps"":
                inputFilename = "tas_script_1.txt";
                break;
            case "Level2_SnakesAndLasers"":
                inputFilename = "tas_script_2.txt";
                break;
            case "Level3_Scales"":
                inputFilename = "tas_script_3.txt";
                break;
            case "Level4_Test"":
                inputFilename = "tas_script_4.txt";
                break;
            default:
                return;
        }
		this.inputLines = File.ReadAllLines(inputFilename);
		Debug.Log(string.Format("Tas input file {0} loaded", inputFilename));
		this.ResetState();
		this.isRunning = true;
	}

	private void StopTas()
	{
		this.ResetState();
		this.isRunning = false;
	}

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
		string[] array = text.Split(new char[]
		{
			','
		});
		this.currentFrameCount = int.Parse(array[0]);
		for (int i = 1; i < array.Length; i++)
		{
            switch (array[i].ToLower()) {
                case "left":
                    Tas.left = true;
                    break;
                case "right":
                    Tas.right = true;
                    break;
                case "jump":
                    Tas.jump = true;
                    break;
            }
		}
	}

	public void UpdateTas()
	{
		if (!this.isRunning)
		{
			return;
		}
		MyCharacterController component = Globals.player.GetComponent<MyCharacterController>();
		if (component == null || component.IsControlPaused() || component.IsLogicPause() || component.visualPlayer.StateIsLocked())
		{
			return;
		}
		if (this.currentFrameCount == 0)
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
		this.currentFrameCount--;
		this.totalFrameCount++;
	}

	private void ResetInputs()
	{
		Tas.left = false;
		Tas.right = false;
		Tas.jump = false;
	}

	private void OnGUI()
	{
		if (!Tas.showGUI)
		{
			return;
		}
		GUI.Label(new Rect(50f, (float)(Screen.height - 60), 200f, 200f), this.totalFrameCount.ToString());
		GUI.Label(new Rect(50f, (float)(Screen.height - 40), 200f, 200f), string.Format("{0} {1} {2}", Tas.left ? "Left " : "", Tas.right ? "Right " : "", Tas.jump ? "Jump " : ""));
	}

	private void Destroy()
	{
		this.ResetInputs();
	}

	private void ResetState()
	{
		this.currentLineIdx = -1;
		this.currentFrameCount = 0;
		this.totalFrameCount = 0;
		this.ResetInputs();
	}

	public bool isRunning;

	private string[] inputLines;

	private int currentLineIdx;

	public static bool left;

	public static bool right;

	public static bool jump;

	public int currentFrameCount;

	private int totalFrameCount;

	private static bool showGUI;
}
