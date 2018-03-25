using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rec : MonoBehaviour {

	[Header("press V to REC stop")]
	public bool recoding;


	// Use this for initialization
	void Start () {
		//this.GetComponent<UTJ.MovieRecorderUI>().record = recoding;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("v"))
		{
			recoding = false;
			//this.GetComponent<UTJ.MovieRecorderUI>().record = recoding;
		}
	}
}
