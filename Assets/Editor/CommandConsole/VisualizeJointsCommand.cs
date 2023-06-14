using System.Collections;
using UnityEngine;


public class VisualizeJointsCommand : ConsoleCommand
{
	public VisualizeJointsCommand(CommandConsole console) : base(console) { }

	public override string Trigger => "joints";
	public override string HelpText => "toggle debug visualization of Rigidbody2D joints";
	public override string ArgHelpText => "";

	public override void Execute(params string[] args) { ToggleCoroutine(); }

	public override IEnumerator CommandCoroutine()
	{
		while (true) {
			foreach (AnchoredJoint2D joint in Object.FindObjectsByType<AnchoredJoint2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) {
				if (!joint.connectedBody) { continue; }
				Debug.DrawLine(joint.transform.position, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor), Color.green, Time.fixedDeltaTime);
			}
			yield return CoroutineUtil.WaitForFixedUpdate;
		}
	}

}
