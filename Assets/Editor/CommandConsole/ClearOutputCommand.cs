public class ClearOutputCommand : ConsoleCommand
{
	public ClearOutputCommand(CommandConsole console) : base(console) { }

	public override string Trigger => "clear";
	public override string HelpText => "clears the console";
	public override string ArgHelpText => "";

	public override void Execute(params string[] args) { Console.Clear(); }
}
