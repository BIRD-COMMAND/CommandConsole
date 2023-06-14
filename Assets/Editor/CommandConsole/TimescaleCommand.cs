using UnityEngine;

public class TimescaleCommand : ConsoleCommand
{
	
	public TimescaleCommand(CommandConsole console) : base(console) {}

	public override string Trigger => "timescale";
	public override string HelpText => "displays or sets the timescale";
	public override string ArgHelpText => "[(optional float) >= 0]";

	public override void Execute(params string[] args)
	{
		// 0 argument call, print current timescale
		if (args.Length == 0) { Console.AddLine("timescale: " + Time.timeScale); }
		
		// 1 argument call, set timescale
		else if (float.TryParse(args[0], out float timescale)) {
			Time.timeScale = timescale;
			Console.AddLine($"timescale set to {timescale}");
		}

		// invalid call, print error
		else { Console.AddLine("failed to parse argument"); }
	}

}