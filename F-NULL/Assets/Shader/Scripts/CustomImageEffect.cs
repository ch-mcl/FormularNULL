using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ポストエフェクトを適用させる

public class CustomImageEffect : MonoBehaviour {
	[SerializeField]
	Shader shader;

	[SerializeField]
	Material material;

	void Start () {
		this.material = new Material(this.shader);
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, this.material);
	}
}
