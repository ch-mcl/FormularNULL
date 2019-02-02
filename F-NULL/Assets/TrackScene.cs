using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScene : MonoBehaviour {
	[SerializeField]
	Color beamColor;

	[SerializeField]
	Color ambientColor;

	[SerializeField]
	Material trackMaterial;

	[SerializeField]
	[Range(0.0f, 1.0f)]
	float colorLearp = 0.5f;

	// Use this for initialization
	void Start () {
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = ambientColor;
	}

	void Update() {
		Color glow;
		Color learpedColor;

		float t;

		t = Mathf.PingPong(Time.time, 1.0f);

		learpedColor = Color.Lerp(Color.white, Color.black, t);

		glow = Color.Lerp(beamColor, learpedColor, colorLearp);

		trackMaterial.SetColor("_EmissionColor", glow);
	}

}
