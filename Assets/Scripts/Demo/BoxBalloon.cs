using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBalloon : MonoBehaviour
{

    // references
    public Transform box;
    public GameObject burstEffect;

    // components
    private DistanceJoint2D joint;
    private Rigidbody2D body;

    // coroutine variables
    private Coroutine inflationCoroutine;
    private float distance;

	private void Awake()
	{
		joint = GetComponent<DistanceJoint2D>();
		body = GetComponent<Rigidbody2D>();
		distance = joint.distance;
	}

	private void OnParticleCollision(GameObject other) { 
        BurstEffect(); 
        PopBalloon(); 
    }

    public void BurstEffect() { 
        Instantiate(burstEffect, transform.position, Quaternion.identity); 
    }

    public void PopBalloon()
    {
		if (inflationCoroutine != null) { StopCoroutine(inflationCoroutine); }
		transform.localScale = Vector3.zero;
		body.gravityScale = 0f;
		body.position = box.position;
        joint.distance = 0.1f;
        inflationCoroutine = StartCoroutine(InflateBalloon(Random.Range(1f, 3f)));
	}

    public IEnumerator InflateBalloon(float delay)
    {
        yield return new WaitForSeconds(delay);
        float t = 0f;
		while (t < 1f) {
            t += 0.5f * Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.4f, 0.4f, 1f), t);
            body.gravityScale = Mathf.Lerp(0f, -1f, t);
            joint.distance = Mathf.Lerp(0.1f, distance, t);
			yield return null;
		}
    }

}
