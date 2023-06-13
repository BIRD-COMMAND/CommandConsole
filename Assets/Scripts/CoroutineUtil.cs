using System.Collections;
using UnityEngine;

/// <summary>
/// A singleton utility-gameobject exposing static methods to allow Coroutines to be run or stopped from anywhere
/// </summary>
public class CoroutineUtil : MonoBehaviour
{

	public static WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
	public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

	// a singleton instance of the above gameobject
	private static CoroutineUtil coroutineUtil;

	private static CoroutineUtil Instance {
		get {
			if (coroutineUtil == null) { coroutineUtil = FindFirstObjectByType<CoroutineUtil>(); }
			if (coroutineUtil == null) { coroutineUtil = new GameObject("CoroutineUtil") { hideFlags = HideFlags.HideAndDontSave }.AddComponent<CoroutineUtil>(); }
			return coroutineUtil;
		}
	}

	public static Coroutine Start(IEnumerator coroutine) { return Instance.StartCoroutine(coroutine); }
	public static void Stop(IEnumerator coroutine) { Instance.StopCoroutine(coroutine); }
	public static void Stop(Coroutine coroutine) { Instance.StopCoroutine(coroutine); }

}
