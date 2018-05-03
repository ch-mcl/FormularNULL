using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 機体の　チェックポイント更新、ラップ更新、ゴール判定

public class VehicleCheckPoint : MonoBehaviour {

	[SerializeField] RaceManage raceManage; // ゴールラップもらい、ゴール判定を渡すオブジェクト

	[Header("CheckPoints")]
	[SerializeField] private int m_checkpoint = 0; // 接触したチェックポイント
	[SerializeField] private int m_nowcheckpoint = -1; // 現在のチェックポイント
	[SerializeField] private int m_lastcheckpoint; // 最後のチェックポイント
	[SerializeField] private int m_currentlap; // 現在のラップ
	private int m_goalLap; // ゴールラップ

	void Start () {
		// ゴールラップをRaceManageから取得
		m_goalLap = raceManage.GoalLap;
	}

	// 初期化
	public void Init(int lastCP) {
		m_lastcheckpoint = lastCP; // 最後に通過するチェックポイントの設定
	}

	void OnTriggerEnter(Collider c) {
		// Playerが通過したらGameManagerのarrivecpに情報を渡す
		string triNmae = c.name;

		// CheckPointかどうか判定
		if(int.TryParse(triNmae, out m_checkpoint)) { 
			HitCheckPoint();
		}
	}

	//チェックポイントやラップ管理
	public void HitCheckPoint(){
		//ぶつかった奴の番号が今のチェックポイントより大きな値か判定
		if (m_checkpoint == m_nowcheckpoint + 1) {
			//チェックポイントが変わった時
			m_nowcheckpoint = m_checkpoint;
		}

		if (m_nowcheckpoint == m_lastcheckpoint) {
			string message; // メッセージ文字列

			//
			if (m_currentlap < m_goalLap) {

				// ラップ更新
				m_currentlap++;

				message = (m_currentlap + 1).ToString() + "LAPS TO GO";
				// ラップタイムを更新
				GetComponent<VehicleTime>().ChangeLapTime(m_currentlap);
				// ラップを更新
				GetComponent<Player_InfoUI>().ChengeLap(m_currentlap);

				// チェックポイントを再度初期化
				m_nowcheckpoint = 0;

				// 2周目
				if (m_currentlap == 1) {
					message = "BOOSTER ON!";
					GetComponent<VehicleMover>().LapBooster = true;
				}
				// ファイナルラップ突入
				if (m_currentlap == m_goalLap - 1) {
					message = "FINAL LAP";
				}
				// ゴール
				if (m_currentlap == m_goalLap) {
					raceManage.Finish = true; // ゴール判定を有効にする
					message = "FINISH!";
					GetComponent<VehicleTime>().StopTotalStopWatch();
				}

				GetComponent<PlayerMessage>().Sender(message);
			}
		}
	}
}
