using System;
using UnityEngine;

public class GravityCommand : ConsoleCommand {

	public GravityCommand(CommandConsole console) : base(console) { }

	public override string Trigger => "gravity";
	public override string HelpText => "displays or sets the gravity y-vector force";
	public override string ArgHelpText => "[(optional float)]";

	public override void Execute(params string[] args)
	{
		// 0 argument call, print current gravity
		if (args.Length == 0) { Console.AddLine("gravity: " + Physics2D.gravity.y); }
		
		// 1 argument call, set gravity
		else if (float.TryParse(args[0], out float gravity)) {
			Physics2D.gravity = new Vector2(0, gravity);
			Console.AddLine($"gravity set to {gravity}");
		}
		
		// invalid call, print error
		else { Console.AddLine("failed to parse argument"); }
	}

}
