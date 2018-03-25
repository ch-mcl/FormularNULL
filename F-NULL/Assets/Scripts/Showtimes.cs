using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Showtimes : MonoBehaviour {

	[SerializeField]
	UnityEngine.UI.Text totalT;
	[SerializeField]
	UnityEngine.UI.Text[] lapTs;

	// Use this for initialization
	void Start () {
		/*
		totalT.text = SceneManage.totalT;
		for(int i = 0; i < lapTs.Length; i++) {
			lapTs[i].text = SceneManage.lapT[i];
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
