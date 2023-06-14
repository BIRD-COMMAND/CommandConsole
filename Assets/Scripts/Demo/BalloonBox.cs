using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBox : MonoBehaviour
{

    public GameObject burstEffect;
    private Vector3 spawn;

    void Start() { spawn = transform.position; StartCoroutine(Respawn()); }

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.relativeVelocity.magnitude > 5f) { StartCoroutine(Respawn()); }
	}

	private IEnumerator Respawn()
    {
        foreach (BoxBalloon item in transform.parent.GetComponentsInChildren<BoxBalloon>()) { item.PopBalloon(); }
        Instantiate(burstEffect, transform.position, Quaternion.identity);
        transform.position = spawn;
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime;
            transform.position = Vector3.Slerp(spawn, spawn + Vector3.up, t);
            yield return null;
        }
    }

}
