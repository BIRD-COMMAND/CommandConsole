using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEditor;
using System;


/// <summary> A simple console command interface. The window can be toggled with hotkey Ctrl+Shift+Q </summary>
[ExecuteAlways]
public class CommandConsole : EditorWindow
{

	#region Fields and Properties

	/// <summary> A dictionary of all available commands </summary>
	private Dictionary<string, Action<string[]>> commandDictionary;
	/// <summary> A dictionary of all running coroutines </summary>
	private Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();
	/// <summary> A dictionary of all available toggle coroutines </summary>
	private Dictionary<string, IEnumerator> toggleCoroutines = new Dictionary<string, IEnumerator>();
	
	/// <summary> The text for the command entry field </summary>
	private string command = "";
	/// <summary> The text for the output area </summary>
	private string output = "";
	/// <summary> The scroll position for the text area </summary>
	private Vector2 scrollPosition;
	/// <summary> Monospace text area GUIStyle </summary>
	private GUIStyle monospaceStyle;

	/// <summary> The name of the command entry field </summary>
	private const string k_CommandFieldControlName = "CommandEntryField";
	/// <summary> Whether the command entry field wants focus </summary>
	private bool commandFieldWantsFocus;
	/// <summary> Whether the command entry field has focus </summary>
	private bool CommandFieldHasFocus => GUI.GetNameOfFocusedControl() == k_CommandFieldControlName;

	#endregion

	[MenuItem("Window/Command Console %#q")]
	static void InitWindow() { GetWindow<CommandConsole>("Command Console"); }

	private void Awake() { Init(); }
	private void OnEnable() { Init(); }
	/// <summary> Initializes the window </summary>
	private void Init()
	{
		if (output == "") { Help(); }
		commandDictionary = new Dictionary<string, Action<string[]>>
		{
			{ "help", _ => Help() },
			{ "clear",_ => Clear() },
			{ "timescale",		Timescale },
			{ "gravity",		Gravity },
			{ "explode",		ApplyForceToRigidbodies },
			{ "joints",			ToggleTrackedCoroutine },
			{ "velocity",		ToggleTrackedCoroutine },
		};
		toggleCoroutines = new Dictionary<string, IEnumerator>
		{
			{ "joints",			VisualizeJointsCoroutine() },
			{ "velocity",		VisualizeVelocityCoroutine() },
		};
	}
	

	void OnGUI()
	{

		// if the monospace style is null, create it
		if (monospaceStyle == null) { 
			monospaceStyle = new GUIStyle(EditorStyles.textArea) { 
				font = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font,
				alignment = TextAnchor.LowerLeft
			};
			commandFieldWantsFocus = true;
		}

		// handle page up/down keys
		if (hasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageUp  ) { scrollPosition -= new Vector2(0f, EditorGUIUtility.singleLineHeight); Repaint(); }
		if (hasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageDown) { scrollPosition += new Vector2(0f, EditorGUIUtility.singleLineHeight); Repaint(); }

		// display the text area for the output
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		EditorGUILayout.LabelField(output, monospaceStyle, GUILayout.ExpandHeight(true));
		EditorGUILayout.EndScrollView();

		// focus the search field when window has focus and tab is pressed
		if (hasFocus && !CommandFieldHasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab) { commandFieldWantsFocus = true; }

		// handle enter key (otherwise it will be consumed by the text field and unfocus)
		if (CommandFieldHasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
			commandFieldWantsFocus = true; ReceiveCommand(command); command = ""; Event.current.Use();
		}

		// command field
		GUI.SetNextControlName(k_CommandFieldControlName);
		command = EditorGUILayout.TextField(command, monospaceStyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));

		// focus handling
		if (commandFieldWantsFocus && Event.current.type == EventType.Repaint) {
			GUI.FocusControl(k_CommandFieldControlName);
			EditorGUIUtility.editingTextField = true;
			commandFieldWantsFocus = false;
		}

	}
	

	/// <summary> Adds a line to the output </summary>
	private void AddLine(string line = "")
	{
		if (string.IsNullOrWhiteSpace(line)) { line = ""; }
		if (!string.IsNullOrWhiteSpace(output)) {
			if (output.EndsWith('\n')) { output += $"> {line}"; }
			else { output += $"\n> {line}"; }
		}
		else { output = $"> {line}"; }
	}

	/// <summary> Trim and add the command to the output, then try to execute it </summary>
	private void ReceiveCommand(string command)
	{
		// trim the command
		command = command.Trim();

		// if the command is empty, add a newline to the output
		if (command == "") { AddLine(); }

		// if the command is not empty, execute it
		else {
			// add the command to the output
			AddLine(command);
			// try to execute the command
			TryExecuteCommand(command);
		}

		// automatically scroll down to newest entry and repaint
		scrollPosition = new Vector2(scrollPosition.x, float.MaxValue); 
		Repaint();
	}

	/// <summary> Try to execute the command </summary>
	public void TryExecuteCommand(string command)
	{
		string[] args = command.Split(' ');
		try {
			if (args.Length > 0 && commandDictionary.TryGetValue(args[0], out Action<string[]> commandFunction)) {
				commandFunction(args); return;
			}
		} catch (Exception e) { Debug.Log($"command: {command ?? "NULL"}"); Debug.LogException(e); }
		AddLine("Unknown command - type 'help' for a list of available commands");
	}


	#region Commands
	
	/// <summary> Clears the command console output </summary>
	private void Clear() { output = ""; }

	/// <summary> Lists all available commands </summary>
	private void Help()
	{
		AddLine("help\t\t\t\tdisplays a list of available commands");
		AddLine("clear\t\t\t\tclears the console");
		AddLine("timescale [value (optional float)]\tdisplays or sets the timescale");
		AddLine("gravity [force (optional float)]\tdisplays or sets the gravity force");
		AddLine("explode [force (optional float)]\tapplies a randomized force to all Rigidbody2Ds");
		AddLine("joints\t\t\t\ttoggle debug visualization of Rigidbody2D joints");
		AddLine("velocity\t\t\ttoggle debug visualization of Rigidbody2D velocities");
	}

	/// <summary> Displays or sets the timescale </summary>
	private void Timescale(params string[] args)
	{
		if (args.Length == 1) { 
			AddLine("timescale: " + Time.timeScale); 
			return; 
		}
		try {
			Time.timeScale = float.Parse(args[1]);
			AddLine("timescale set to " + Time.timeScale);
		}
		catch { AddLine("failed to parse argument"); }
	}

	/// <summary> Displays or sets the gravity force </summary>
	private void Gravity(params string[] args)
	{
		if (args.Length == 1) { 
			AddLine("gravity: " + Physics2D.gravity.y); 
			return; 
		}
		try {
			Physics2D.gravity = new Vector2(0, float.Parse(args[1]));
			AddLine("gravity set to " + Physics2D.gravity.y);
		}
		catch { AddLine("failed to parse argument"); }
	}

	/// <summary> Applies a randomized force to all Rigidbody2Ds </summary>
	private void ApplyForceToRigidbodies(params string[] args)
	{
		float forceMagnitude = 10f;
		if (args.Length > 1) { 
			try { forceMagnitude = float.Parse(args[1]); } 
			catch { AddLine("failed to parse force value argument, using default force"); } 
		}

		Rigidbody2D[] allRigidbodies = FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		foreach (Rigidbody2D body in allRigidbodies) {
			if (body.bodyType == RigidbodyType2D.Dynamic) {
				body.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * forceMagnitude);
			}
		}

		AddLine("applied force to " + allRigidbodies.Length + " Rigidbody2D objects");
	}

	/// <summary> Toggles the coroutine with the given key if it exists </summary>
	public void ToggleTrackedCoroutine(params string[] args)
	{		
		if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0])) { 
			AddLine("unable to toggle coroutine"); 
			return; 
		}
		string key = args[0];

		if (coroutines.ContainsKey(key)) {
			CoroutineUtil.Stop(coroutines[key]);
			coroutines.Remove(key);
			AddLine($"{key} coroutine stopped"); 
			return;
		}
		else if (toggleCoroutines.TryGetValue(key, out IEnumerator coroutine)) {
			coroutines.Add(key, CoroutineUtil.Start(coroutine));
			AddLine($"{key} coroutine started"); 
			return;
		}
		else { AddLine("unable to toggle coroutine"); }
	}

	/// <summary> A coroutine that visualizes all Rigidbody2D joints </summary>
	private IEnumerator VisualizeJointsCoroutine()
	{
		while (true) {
			foreach (AnchoredJoint2D joint in FindObjectsByType<AnchoredJoint2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) {
				if (!joint.connectedBody) { continue; }
				Debug.DrawLine(joint.transform.position, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor), Color.green, Time.fixedDeltaTime);
			}
			yield return CoroutineUtil.WaitForFixedUpdate;
		}
	}

	/// <summary> A coroutine that visualizes all Rigidbody2D velocities </summary>
	private IEnumerator VisualizeVelocityCoroutine()
	{
		while (true) {
			foreach (Rigidbody2D body in FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) {
				Debug.DrawLine(body.transform.position, body.transform.position + (Vector3)body.velocity, 
					Color.Lerp(Color.green, Color.red, body.velocity.magnitude / 20f), Time.fixedDeltaTime);
			}
			yield return CoroutineUtil.WaitForFixedUpdate;
		}
	}

	#endregion

}
