using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 機体の　チェックポイント更新、ラップ更新、ゴール判定

public class VehicleCheckPoint : MonoBehaviour {

	[SerializeField] RaceManage raceManage; // ゴールラップもらい、ゴール判定を渡すオブジェクト

	[Header("CheckPoints")]
	[SerializeField] private int m_CheckPoint = 0; // 接触したチェックポイント
	[SerializeField] private int m_CurrentCheckPoint = -1; // 現在のチェックポイント
	[SerializeField] private int m_LastCheckPoint; // 最後のチェックポイント
	[SerializeField] private int m_CurrentLap; // 現在のラップ
	private int m_goalLap; // ゴールラップ

	protected Vector3 m_CheckointPos; // 接触したチェックポイント座標
	protected Vector3 m_CurrentCheckointPos; // 現在のチェックポイント座標

	// プロパティ

	// チェックポイント値
	public int CheckPoint {
		get { return m_CurrentCheckPoint; }
	}

	// ラップ
	public int CurrentLap {
		get { return m_CurrentLap; }
	}

	// 現在のチェックポイント座標を取得
	public Vector3 CurrentWayPoint {
		get { return m_CurrentCheckointPos; }
	}

	void Start () {
		// ゴールラップをRaceManageから取得
		m_goalLap = raceManage.GoalLap;
	}

	// 初期化
	public void Init(int lastCP) {
		// 最後に通過するチェックポイントの設定
		m_LastCheckPoint = lastCP;

		// チェックポイント座標の初期化
		m_CheckointPos = raceManage.WayPoint.transform.GetChild(0).position;
	}

	void OnTriggerEnter(Collider c) {
		string triName = c.name;

		// CheckPointかどうか判定
		if(int.TryParse(triName, out m_CheckPoint)) {
			m_CurrentCheckointPos = c.transform.position;
			HitCheckPoint();
		}
	}

	//チェックポイントやラップ管理
	public void HitCheckPoint(){
		//ぶつかった奴の番号が今のチェックポイントより大きな値か判定
		if (m_CheckPoint == m_CurrentCheckPoint + 1) {
			//チェックポイントが変わった時
			m_CurrentCheckPoint = m_CheckPoint;
			// チェックポイント座標を更新
			m_CurrentCheckointPos = raceManage.WayPoint.transform.GetChild(m_CheckPoint).position;
		}

		if (m_CurrentCheckPoint == m_LastCheckPoint) {
			string message; // メッセージ文字列

			//
			if (m_CurrentLap < m_goalLap) {

				// ラップ更新
				m_CurrentLap++;

				message = (m_CurrentLap + 1).ToString() + "LAPS TO GO";
				// ラップタイムを更新
				GetComponent<VehicleTime>().ChangeLapTime(m_CurrentLap);
				// ラップを更新
				GetComponent<Player_InfoUI>().ChengeLap(m_CurrentLap);

				// チェックポイント座標を再度初期化
				m_CurrentCheckPoint = 0;
				//m_CurrentCheckointPos = raceManage.WayPoint.transform.GetChild(0).position;

				// 2周目
				if (m_CurrentLap == 1) {
					message = "BOOSTER ON!";
					GetComponent<VehicleMover>().LapBooster = true;
				}
				// ファイナルラップ突入
				if (m_CurrentLap == m_goalLap - 1) {
					message = "FINAL LAP";
				}
				// ゴール
				if (m_CurrentLap == m_goalLap) {
					raceManage.Finish = true; // ゴール判定を有効にする
					message = "FINISH!";
					GetComponent<VehicleTime>().StopTotalStopWatch();
				}

				GetComponent<PlayerMessage>().Sender(message);
			}
		}
	}
}
