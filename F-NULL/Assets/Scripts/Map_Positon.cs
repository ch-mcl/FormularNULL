using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map_Positon : MonoBehaviour {

	[SerializeField] RaceManage raceManage; // GameManage
	[SerializeField] Camera mapCam;
	[SerializeField] Canvas canvas;
	[SerializeField] Transform[] playerpos; // プレイヤーの位置
	[SerializeField] RectTransform[] icons; // プレイヤーアイコン

	// Use this for initialization
	void Start () {
		playerpos = raceManage.GetPlayers();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < playerpos.Length; i++) {
			// World(グローバル)座標からScreen(画面)座標へ変換
			var screenPos = RectTransformUtility.WorldToScreenPoint(mapCam, playerpos[i].position);

			// Screen座標からScreen Space座標へ変換
			var canvasRect = canvas.GetComponent<RectTransform>();
			var pos = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, mapCam, out pos);
			icons[i].localPosition = pos;

			// 機体の向きに合わせる
			icons[i].localEulerAngles = new Vector3(0f, 0f, -playerpos[i].eulerAngles.y);
		}
	}
}
