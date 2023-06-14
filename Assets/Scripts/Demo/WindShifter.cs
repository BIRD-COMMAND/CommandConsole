using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindShifter : MonoBehaviour
{

	// wind control variables
	private float offset, noise;
	public ParticleSystem east, west;
	private ParticleSystem.EmissionModule eastEmission, westEmission;

	// Start is called before the first frame update
	void Start()
	{
		eastEmission = east.emission; westEmission = west.emission;
	}

	// Update is called once per frame
	void Update()
	{		
		ControlWind();
	}

	private void ControlWind()
	{
		// increment offset by time times a speed factor
		offset += Time.deltaTime * 0.01f;
		// Get a PerlinNoise value (0 to 1) to smoothly vary the wind
		noise = Mathf.PerlinNoise(offset, offset);
		// Use a cube root transformation to make the noise value more extreme
		noise = System.MathF.Cbrt((noise - 0.5f) / 4f) + 0.5f;
		// Set the emission rates of the opposing particle systems based on the noise value
		eastEmission.rateOverTime = Mathf.Lerp(20f, 4f, noise);
		westEmission.rateOverTime = Mathf.Lerp(4f, 20f, noise);
	}

}
