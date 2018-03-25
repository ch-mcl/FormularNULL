using UnityEngine;
using System.Collections;

public class RaceManage : MonoBehaviour {
	[SerializeField] private GameObject[] players; // 最大数のPlayerオブジェクトをEditorでアタッチ
	[SerializeField] private GameObject[] activePlayers;
	[SerializeField] private WayPoint waypoint; // waypoint

	// こいつらは setter で参照される
	public static bool canmove; // 動いていいか(VehicleControllerにて参照)
	public bool finish = false; // ゴール
	public bool miss = false; // 失敗
	[SerializeField] int laps = 3; // ゴールまでのラップ

	public int GoalLap {
		get { return laps; }
	}

	/*
	// canmoveのプロパティ
	public bool Canmove {
		get { return canmove; }
	}

	// finishのプロパティ
	public bool Finish {
		get { return finish; }
	}

	// miss
	public bool Miss {
		get { return miss; }
	}

	// finishのプロパティ
	public bool Lap {
		get { return finish; }
	}
	*/

	VehicleTime[] vehicleTimes;
	PlayerMessage[] playerMessages;

	void Awake(){
		canmove = false; // canmoveの初期化
		waypoint = GameObject.FindObjectOfType<WayPoint>();

		// ActiveになっているPlayer数の取得
		int actives = 0;
		for (int i = 0; i < players.Length; i++){
			if (players[i].activeInHierarchy) {
				actives++;
			}
		}

		
		activePlayers = new GameObject[actives]; // ActiveになっているPlayerをキャッシュ
		vehicleTimes = new VehicleTime[actives]; // VehicleTimeをキャッシュ
		playerMessages = new PlayerMessage[actives]; // PlayerMessageをキャッシュ

		// ActiveになっているPlayerの取得
		int j = 0;
		for (int i = 0; i < players.Length; i++) {
			if (players[i].activeInHierarchy) {
				activePlayers[j] = players[i];
				j++;
			}
		}
	}

	void Start() {
		// チェックポイント初期化

		for (int i = 0; i < activePlayers.Length; i++) {
			// チェックポイント初期化
			activePlayers[i].GetComponent<VehicleCheckPoint>().Init(waypoint.path_objs.Count - 1);

			vehicleTimes[i] = activePlayers[i].GetComponent<VehicleTime>(); // VehicleTimeをキャッシュ
			vehicleTimes[i].Setlap(laps);

			playerMessages[i] = activePlayers[i].GetComponent<PlayerMessage>(); // Playermessageをキャッシュ

			// スタート地点へ移動する
			activePlayers[i].transform.position = waypoint.transform.GetChild(i).position + (Vector3.up * 0.6f); // 0.6は機体の浮遊する高さ
			activePlayers[i].transform.rotation = waypoint.transform.GetChild(i).rotation; // 向き
		}
	}

	/*
	void Update () {
	}
	*/

	// 終了ラップの取得
	public int GetLap() {
		return laps;
	}

	// カウント表示
	public void RaceCount(string str){
		for (int i = 0; i < playerMessages.Length; i++) {
			playerMessages[i].Sender(str);
		}
	}

	// GOサイン処理
	public void RaceStart(string str){
		canmove = true;
		for (int i = 0; i < vehicleTimes.Length; i++) {
			// Playerにアタッチされているスクリプトを参照する
			playerMessages[i].Sender(str);
			// ストップウォッチ開始
			vehicleTimes[i].TotalSwStart();
			vehicleTimes[i].LapSwstart();
		}
	}

	// プレイヤの位置を渡す(Map_Position用)
	public Transform[] GetPlayers(){
		Transform[] playerTrans = new Transform[activePlayers.Length];
		for (int i = 0; i < activePlayers.Length; i++) {
			playerTrans[i] = activePlayers[i].transform;
		}
		return playerTrans;
	}
}

