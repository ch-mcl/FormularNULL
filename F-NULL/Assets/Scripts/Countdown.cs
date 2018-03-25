using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

	[SerializeField] RaceManage raceManage;
	[SerializeField] AudioSource countDJ;
	[SerializeField] AudioClip[] DJc = new AudioClip[3];
	[SerializeField] AudioClip DJgo;

	public int countValue = 3; // カウントダウンする数 3から開始
	bool counted = false; //カウント終了

	void Start () {
		StartCoroutine(CountingDown());
	}

	// Countdown
    IEnumerator CountingDown(){
        while (countValue > 0) {
			raceManage.RaceCount(countValue.ToString("0"));
			countDJ.clip = DJc[countValue-1];
			countDJ.Play();
			countValue --;
            yield return new WaitForSeconds(1.0f);
        }
		counted = true;
		raceManage.RaceStart("GO");
		countDJ.clip = DJgo;
		countDJ.Play();
		
        yield return new WaitForSeconds(1.0f);
       // Message.text = "";
	}

	public bool GetCounted(){
		return counted;
	}
}
