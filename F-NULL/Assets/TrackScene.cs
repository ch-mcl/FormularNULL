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

	// Use this for initialization
	void Start () {
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = ambientColor;
	}
}
