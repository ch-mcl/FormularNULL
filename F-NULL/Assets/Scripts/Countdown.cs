using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カウントダウン
// コースイントロ終了後に、有効にすればいいんじゃないか？

public class Countdown : MonoBehaviour {

	[SerializeField] RaceManage raceManage;
	[SerializeField] AudioSource countDJ;
	[SerializeField] AudioClip[] DJc = new AudioClip[3];
	[SerializeField] AudioClip DJgo;

	[SerializeField] protected int countValue = 3; // カウントダウンする数 3から開始
	bool counted = false; //カウント終了

	// カウントダウン値の取得
	public int CountValue {
		get { return countValue; }
	}

	// カウント完了フラグの取得
	public bool Counted {
		get { return counted; }
	}

	void Start () {
		// カウントダウン開始
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
}
