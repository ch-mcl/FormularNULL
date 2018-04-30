using UnityEngine;
using System.Collections;

public class RaceManage : MonoBehaviour {
	[SerializeField] private GameObject[] m_players; // 最大数のPlayerオブジェクトをEditorでアタッチ
	[SerializeField] private GameObject[] m_activePlayers;
	[SerializeField] private WayPoint m_waypoint; // waypoint

	// こいつらは setter で参照される
	[SerializeField] private bool m_canmove; // 動いていいか(VehicleControllerにて参照)
	[SerializeField] private bool m_finish = false; // ゴール (player分のbool配列であるべき)
	[SerializeField] private bool m_miss = false; // 失敗
	[SerializeField] int m_gaolLap = 3; // ゴールまでのラップ

	// 
	public int GoalLap {
		get { return m_gaolLap; }
	}

	// canmoveのプロパティ
	public bool Canmove {
		get { return m_canmove; }
	}

	// finishのプロパティ
	public bool Finish {
		get { return m_finish; }
		set { m_finish = value; }
	}

	// miss
	public bool Miss {
		get { return m_miss; }
	}

	// finishのプロパティ
	public bool Lap {
		get { return m_finish; }
	}

	VehicleTime[] vehicleTimes;
	PlayerMessage[] playerMessages;

	void Awake(){
		Debug.Log("excuted");

		m_canmove = false; // canmoveの初期化
		m_waypoint = GameObject.FindObjectOfType<WayPoint>();

		// ActiveになっているPlayer数の取得
		int actives = 0;
		for (int i = 0; i < m_players.Length; i++){
			if (m_players[i].activeInHierarchy) {
				actives++;
			}
		}

		
		m_activePlayers = new GameObject[actives]; // ActiveになっているPlayerをキャッシュ
		vehicleTimes = new VehicleTime[actives]; // VehicleTimeをキャッシュ
		playerMessages = new PlayerMessage[actives]; // PlayerMessageをキャッシュ

		// ActiveになっているPlayerの取得
		int j = 0;
		for (int i = 0; i < m_players.Length; i++) {
			if (m_players[i].activeInHierarchy) {
				m_activePlayers[j] = m_players[i];
				j++;
			}
		}
	}

	void Start() {
		// チェックポイント初期化

		for (int i = 0; i < m_activePlayers.Length; i++) {
			// チェックポイント初期化
			m_activePlayers[i].GetComponent<VehicleCheckPoint>().Init(m_waypoint.path_objs.Count - 1);

			vehicleTimes[i] = m_activePlayers[i].GetComponent<VehicleTime>(); // VehicleTimeをキャッシュ
			vehicleTimes[i].Setlap(m_gaolLap);

			playerMessages[i] = m_activePlayers[i].GetComponent<PlayerMessage>(); // Playermessageをキャッシュ

			// スタート地点へ移動する
			m_activePlayers[i].transform.position = m_waypoint.transform.GetChild(i).position + (Vector3.up * 0.6f); // 0.6は機体の浮遊する高さ
			m_activePlayers[i].transform.rotation = m_waypoint.transform.GetChild(i).rotation; // 向き
		}
	}

	/*
	void Update () {
	}
	*/

	// 終了ラップの取得
	public int GetLap() {
		return m_gaolLap;
	}

	// カウント表示
	public void RaceCount(string str){
		for (int i = 0; i < playerMessages.Length; i++) {
			playerMessages[i].Sender(str);
		}
	}

	// GOサイン処理
	public void RaceStart(string str){
		m_canmove = true;
		for (int i = 0; i < vehicleTimes.Length; i++) {
			// Playerにアタッチされているスクリプトを参照する
			playerMessages[i].Sender(str);
			// ストップウォッチ開始
			vehicleTimes[i].StartTotalStopWatch();
			vehicleTimes[i].StartLapStopWatch();
		}
	}

	// プレイヤの位置を渡す(Map_Position用)
	public Transform[] GetPlayers(){
		Transform[] playerTrans = new Transform[m_activePlayers.Length];
		for (int i = 0; i < m_activePlayers.Length; i++) {
			playerTrans[i] = m_activePlayers[i].transform;
		}
		return playerTrans;
	}
}

