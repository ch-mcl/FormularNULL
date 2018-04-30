using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCheckPoint : MonoBehaviour {

	[SerializeField] RaceManage raceManage;

	[Header("CheckPoints")]
	[SerializeField] private int hitcheckpoint = 0; // 接触したチェックポイント
	[SerializeField] private int nowcheckpoint = -1; // 現在のチェックポイント
	[SerializeField] private int lastcheckpoint; // 最後のチェックポイント
	[SerializeField] private int currentlap; // 現在のラップ

	[SerializeField] private int goalLap;

	// Use this for initialization
	void Start () {
		// ゴールするのに必要なラップ数をRaceManageから取得
		goalLap = raceManage.GoalLap;
	}

	// 初期化
	public void Init(int lastCP) {
		lastcheckpoint = lastCP; // 最後に通過するチェックポイントの設定
	}

	void OnTriggerEnter(Collider c) {
		// Playerが通過したらGameManagerのarrivecpに情報を渡す
		string triNmae = c.name;

		// CheckPointかどうか判定
		if(int.TryParse(triNmae, out hitcheckpoint)) { 
			arrivecp();
		}
	}

	//チェックポイントやラップ管理
	public void arrivecp(){
		//ぶつかった奴の番号が今のチェックポイントより大きな値か判定
		if (hitcheckpoint == nowcheckpoint + 1) {
			//チェックポイントが変わった時
			nowcheckpoint = hitcheckpoint;
			}

		if (nowcheckpoint == lastcheckpoint) {
			// 経過ラップの表示
			currentlap++; // ラップ更新
			//GetComponent<PlayerMessage>().Sender((goalLap - currentlap).ToString() + "LAPS LEFT");
			GetComponent<PlayerMessage>().Sender((currentlap + 1).ToString() + "LAPS TO GO");
			GetComponent<VehicleTime>().ChangeLapTime(currentlap); // 
			GetComponent<Player_InfoUI>().ChengeLap(currentlap);
			nowcheckpoint = 0;
			// 2周目
			if (currentlap == 1){
				GetComponent<PlayerMessage>().Sender("BOOSTER ON!");
				GetComponent<VehicleMover>().lapBooster = true;
			}
			// ファイナルラップ突入
			if (currentlap == goalLap - 1){
				GetComponent<PlayerMessage>().Sender("FINAL LAP");
			}
			// ゴール
			if (currentlap == goalLap){
				raceManage.Finish = true; // ゴール判定を有効にする
				GetComponent<PlayerMessage>().Sender("FINISH!");
				GetComponent<VehicleTime>().StopTotalStopWatch();
			}
		}
	}
}
