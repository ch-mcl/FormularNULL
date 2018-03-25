using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ポーズ機能
public class Pause_Manage : MonoBehaviour {

	bool pause = false;
	[SerializeField] GameObject pauseUI; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (pause) {
			Time.timeScale = 0;
			pauseUI.SetActive(true);
		} else{
			Time.timeScale = 1;
			pauseUI.SetActive(false);
		}
	}

	// ポーズ状態の解除/有効
	public void ChangePause() {
		if (pause) {
			pause = false;
		} else{
			pause = true;
		}
	}

	// ポーズ状態の取得  
	public bool GetPause() {
		return pause;
	}
}
