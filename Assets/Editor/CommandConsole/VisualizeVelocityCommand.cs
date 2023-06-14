using System.Collections;
using UnityEngine;


public class VisualizeVelocityCommand : ConsoleCommand
{
	public VisualizeVelocityCommand(CommandConsole console) : base(console) { }

	public override string Trigger => "velocity";
	public override string HelpText => "toggle debug visualization of Rigidbody2D velocities";
	public override string ArgHelpText => "";

	public override void Execute(params string[] args) {
		if (Application.isPlaying) { ToggleCoroutine(); }
		else { Console.AddLine("only available while application is running"); }
	}

	public override IEnumerator CommandCoroutine()
	{
		while (true) {
			if (!Application.isPlaying) { ToggleCoroutine(); }
			foreach (Rigidbody2D body in Object.FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) {
				Debug.DrawLine(body.transform.position, body.transform.position + ((Vector3)body.velocity / 2f),
					Color.Lerp(Color.green, Color.red, body.velocity.magnitude / 20f), Time.fixedDeltaTime);
			}
			yield return CoroutineUtil.WaitForFixedUpdate;
		}
	}

}
