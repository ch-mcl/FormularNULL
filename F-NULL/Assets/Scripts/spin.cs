using UnityEngine;
using System.Collections;

public class spin : MonoBehaviour {

	//回す為のScript
	public float x;
	public float y;
	public float z;

	//public float zikan = 0;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void FixedUpdate () {

		//zikan = zikan + Time.deltaTime;

		transform.Rotate (x, y, z);
	}
}
