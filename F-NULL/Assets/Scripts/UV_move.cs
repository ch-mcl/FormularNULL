using UnityEngine;
using System.Collections;

public class UV_move : MonoBehaviour {


	[SerializeField]private float scrollSpeedX = 0f;
	[SerializeField]private float scrollSpeedY = 0f;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
	}
	
	// Update is called once per frame
	void Update () {
		float x = Mathf.Repeat(Time.time * scrollSpeedX, 1);
		float y = Mathf.Repeat(Time.time * scrollSpeedY, 1);

		Vector2 offset = new Vector2(x, y);

		GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
	}
}
