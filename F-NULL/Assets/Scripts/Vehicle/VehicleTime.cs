using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VehicleTime : MonoBehaviour {

	[SerializeField] System.Diagnostics.Stopwatch[] lapTimes; // タイムを保存
	[SerializeField] Countdown counDown; // カウントダウン完了したか
	[SerializeField] RaceManage raceManage; // ゴールしたか
	
	// ラップタイム
	private System.Diagnostics.Stopwatch lapStopWatch = new System.Diagnostics.Stopwatch();
	// 総合タイム
	private System.Diagnostics.Stopwatch TotalStopWatch = new System.Diagnostics.Stopwatch(); 

	int m_goalLap; // ゴールまでのラップ数
	Player_InfoUI playerInfo;	
	

	// 初期化処理
	void Start () {
		lapStopWatch.Reset();
		TotalStopWatch.Reset();
		playerInfo = GetComponent<Player_InfoUI>();
	}
	
	void Update () {
		// ポーズ中、もしくはゴール時
		if(Time.timeScale <= 0 || raceManage.Finish == true) {
			// 各種タイマーを停止
			TotalStopWatch.Stop();
			lapStopWatch.Stop();
		// カウントダウン完了時
		} else if(counDown.GetCounted()) {
			// 各種タイマーをスタート
			TotalStopWatch.Start();
			lapStopWatch.Start();
		}

		// タイムを文字列に変換
		playerInfo.TotalTimeText.text = TotalStopWatch.Elapsed.Minutes.ToString("00") + ":" + TotalStopWatch.Elapsed.Seconds.ToString("00") + ":" + TotalStopWatch.Elapsed.Milliseconds.ToString("000");
		/*
		playerInfo.laptimes[(m_goalLap - 1)].text =
					lap.ToString().PadLeft(2) + " " + lapSw.Elapsed.Minutes.ToString("00") + ":" + lapSw.Elapsed.Seconds.ToString("00") + ":" + lapSw.Elapsed.Milliseconds.ToString("000");
		*/
	}

	// ラップ変更時におけるStopWatchの処理
	public void ChangeLapTime(int lap) {
		lapStopWatch.Stop();
		if (lap - 1 < m_goalLap) {
			lapTimes[lap - 1] = lapStopWatch;
			//lapTimes[lap - 1] = lap.ToString().PadLeft(2) + " " + lapSw.Elapsed.Minutes.ToString("00") + ":" + lapSw.Elapsed.Seconds.ToString("00") + ":" + lapSw.Elapsed.Milliseconds.ToString("000");
			// 表示用配列に収まる時
			if (lap - 1 < playerInfo.LapTimesTexts.Length) {
				// タイムを文字列に変換
				playerInfo.LapTimesTexts[(lap - 1)].text = 
					lap.ToString().PadLeft(2) + " " + lapTimes[lap - 1].Elapsed.Minutes.ToString("00") + ":" + lapTimes[lap - 1].Elapsed.Seconds.ToString("00") + ":" + lapTimes[lap - 1].Elapsed.Milliseconds.ToString("000");
				//playerInfo.laptimes[(lap - 1)].text = lapTimes[lap - 1];
			}
			else {
				// 要素を入れ替える
				//SwapArray(lap);
			}
			lapStopWatch.Reset();
			lapStopWatch.Start();
		}
	}

	// ラップタイム計測開始
	public void StartLapStopWatch() {
		lapStopWatch.Reset();
		lapStopWatch.Start();
	}

	// 総合タイム計測開始
	public void StartTotalStopWatch() {
		TotalStopWatch.Reset();
		TotalStopWatch.Start();
	}

	// 総合タイム停止
	public void StopTotalStopWatch() {
		TotalStopWatch.Stop();
	}
	
	// ゴールまでのラップを取得
	public void Setlap(int goalLap) {
		m_goalLap = goalLap;
		lapTimes = new System.Diagnostics.Stopwatch[m_goalLap];
	}


	// lapTimesの要素を1つずらす
	// これ動くか?
	public void SwapLapTimesArray(int lap) {
		for (int i = 0; i < playerInfo.LapTimesTexts.Length-1; i++){
			playerInfo.LapTimesTexts[i].text = playerInfo.LapTimesTexts[i+1].text;
		}
		playerInfo.LapTimesTexts[lapTimes.Length-1].text = playerInfo.LapTimesTexts[lap - 1].text;
		playerInfo.LapTimesTexts[lapTimes.Length-1].text = "00:00:000";
	}
}
