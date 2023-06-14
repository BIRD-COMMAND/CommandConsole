using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

/// <summary> A simple console command interface. The window can be toggled with hotkey Ctrl+Shift+Q </summary>
[ExecuteAlways]
public class CommandConsole : EditorWindow
{

	/// <summary> Wrapper for custom GUIStyles to ensure they are initialized from the correct thread when used in OnGUI. </summary>
	private static class Styles
	{
		public static GUIStyle Monospace;
		static Styles() { 
			Monospace = new GUIStyle(EditorStyles.textArea) {
				font = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font,
				alignment = TextAnchor.LowerLeft
			};
		}
	}

	#region Fields and Properties

	/// <summary> A list of all available commands </summary>
	private List<ConsoleCommand> commands;
	
	/// <summary> The text for the command entry field </summary>
	private string command = "";
	/// <summary> The text for the output area </summary>
	private string output = "";
	/// <summary> The scroll position for the text area </summary>
	private Vector2 scrollPosition;
	/// <summary> The padding for help text between the command trigger and the help message. </summary>
	private int padding = 4;

	/// <summary> The name of the command entry field </summary>
	private const string k_CommandFieldControlName = "CommandEntryField";
	/// <summary> Whether the command entry field wants focus </summary>
	private bool commandFieldWantsFocus = true;
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
				
		commands?.Clear();
		commands = new List<ConsoleCommand>();

		// Automatically populate commands list with all subclasses of ConsoleCommand

		// Get all types in the current assembly.
		Type[] types = Assembly.GetExecutingAssembly().GetTypes();
		foreach (Type type in types) {
			// Check if the type is a subclass of ConsoleCommand, and if it is not abstract.
			if (type.IsSubclassOf(typeof(ConsoleCommand)) && !type.IsAbstract) {
				// Create an instance of the type and add it to the list of commands.
				ConsoleCommand instance = (ConsoleCommand)Activator.CreateInstance(type, new object[] { this });
				commands.Add(instance);
			}
		}
		// Sort the commands alphabetically by trigger
		commands = commands.OrderBy(c => c.Trigger).ToList();

		// calculate the padding for the help text
		foreach (var command in commands) {
			padding = Mathf.Max(padding, $"{command.Trigger} {command.ArgHelpText}".Length + 4);
		}

		if (output == "") { Help(); }

	}
	

	void OnGUI()
	{

		// handle page up/down keys
		if (hasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageUp  ) { scrollPosition -= new Vector2(0f, EditorGUIUtility.singleLineHeight); Repaint(); }
		if (hasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageDown) { scrollPosition += new Vector2(0f, EditorGUIUtility.singleLineHeight); Repaint(); }

		// display the text area for the output
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		EditorGUILayout.LabelField(output, Styles.Monospace, GUILayout.ExpandHeight(true));
		EditorGUILayout.EndScrollView();

		// focus the search field when window has focus and tab is pressed
		if (hasFocus && !CommandFieldHasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab) { commandFieldWantsFocus = true; }

		// handle enter key (otherwise it will be consumed by the text field and unfocus)
		if (CommandFieldHasFocus && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
			commandFieldWantsFocus = true; ReceiveCommand(command); command = ""; Event.current.Use();
		}

		// command field
		GUI.SetNextControlName(k_CommandFieldControlName);
		command = EditorGUILayout.TextField(command, Styles.Monospace, GUILayout.Height(EditorGUIUtility.singleLineHeight));

		// focus handling
		if (commandFieldWantsFocus && Event.current.type == EventType.Repaint) {
			GUI.FocusControl(k_CommandFieldControlName);
			EditorGUIUtility.editingTextField = true;
			commandFieldWantsFocus = false;
		}

	}
	

	/// <summary> Adds a line to the output </summary>
	public void AddLine(string line = "")
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
		
		if (command == "help") { Help(); return; }

		List<string> args = command.Split(' ').ToList();
		string trigger = args[0]; args.RemoveAt(0);
		
		ConsoleCommand consoleCommand = commands.FirstOrDefault(c => c.Trigger == trigger);
		if (consoleCommand != null) { consoleCommand.Execute(args.ToArray()); }
		else { AddLine("Unknown command - type 'help' for a list of available commands"); }

	}

	/// <summary> Clears the command console output </summary>
	public void Clear() { output = ""; }

	/// <summary> Lists all available commands </summary>
	private void Help()
	{
		AddLine($"help {new string(' ', padding - 5)}displays a list of available commands");
		foreach (ConsoleCommand item in commands) { 
			AddLine($"{item.Trigger} {item.ArgHelpText}{new string(' ', padding - $"{item.Trigger} {item.ArgHelpText}".Length)}{item.HelpText}");
		}
	}

}