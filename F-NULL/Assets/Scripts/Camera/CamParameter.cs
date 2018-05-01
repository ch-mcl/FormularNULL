using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カメラ設定用クラス

public class CamParameter {
	private Vector3 camPos; // 位置
	private float camPitch; // カメラのx軸(ピッチ)角度

	public Vector3 CamPos {
		get { return camPos; }
	}

	public float CamPitch {
		get { return camPitch; }
	}
}