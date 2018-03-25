using UnityEngine;
using System.Collections;
using System;

// スクリーンショット撮影
public class ScreenShot : MonoBehaviour {

	[Header("撮影:pキー")]
	public int dpi = 1; // 解像度
	
	// 解像度をあげると大きな画面サイズで撮影が可能
	// 4とかだと、4倍の大きさで撮影される

	void Update () {
		if (Input.GetKey("p")){
			TakeScreenShot();
		}
	}

	// スクリーンショットの撮影
	public void TakeScreenShot(){
		// 現在の時刻をファイル名にする
		String fileName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png";
		// スクリーンショット撮影
		ScreenCapture.CaptureScreenshot(fileName, dpi);
		Debug.Log("ScreenShot Saved" + fileName + ".png");
	}
}
