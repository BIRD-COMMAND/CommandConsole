using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ForceRigidbody2DsCommand : ConsoleCommand
{
	public ForceRigidbody2DsCommand(CommandConsole console) : base(console) { }

	public override string Trigger => "explode";
	public override string HelpText => "applies a randomized force to all Rigidbody2Ds";
	public override string ArgHelpText => "[(optional float)]";
	
	public override void Execute(params string[] args)
	{
		float forceMagnitude = 200f;
		if (args.Length > 0 && !float.TryParse(args[0], out forceMagnitude)) {
			Console.AddLine("failed to parse force value argument, using default force");
			forceMagnitude = 10f;
		}

		Rigidbody2D[] allRigidbodies = UnityEngine.Object.FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		foreach (Rigidbody2D body in allRigidbodies) {
			if (body.bodyType == RigidbodyType2D.Dynamic) {
				body.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * forceMagnitude);
			}
		}

		Console.AddLine("applied force to " + allRigidbodies.Length + " Rigidbody2D objects");
	}
}
