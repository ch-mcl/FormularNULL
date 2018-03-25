using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VehicleTime : MonoBehaviour {

	[SerializeField] System.Diagnostics.Stopwatch[] lapTimes; // タイムを保存
	[SerializeField] Countdown counDown; // カウントダウン完了したか
	[SerializeField] RaceManage raceManage; // ゴールしたか
	
	// ラップタイム
	private System.Diagnostics.Stopwatch lapSw = new System.Diagnostics.Stopwatch();
	// 総合タイム
	private System.Diagnostics.Stopwatch Totalsw = new System.Diagnostics.Stopwatch(); 

	int m_goalLap; // ゴールまでのラップ数
	Player_InfoUI playerInfo;	
	
	void Start () {
		lapSw.Reset();
		Totalsw.Reset();
		playerInfo = GetComponent<Player_InfoUI>();
	}
	
	void Update () {
		// ポーズ中、もしくはゴール時
		if(Time.timeScale <= 0 || raceManage.finish) {
			// 各種タイマーを停止
			Totalsw.Stop();
			lapSw.Stop();
		// カウントダウン完了時
		} else if(counDown.GetCounted()) {
			// 各種タイマーをスタート
			Totalsw.Start();
			lapSw.Start();
		}

		// タイムを文字列に変換
		playerInfo.totalTime.text = Totalsw.Elapsed.Minutes.ToString("00") + ":" + Totalsw.Elapsed.Seconds.ToString("00") + ":" + Totalsw.Elapsed.Milliseconds.ToString("000");
		/*
		playerInfo.laptimes[(m_goalLap - 1)].text =
					lap.ToString().PadLeft(2) + " " + lapSw.Elapsed.Minutes.ToString("00") + ":" + lapSw.Elapsed.Seconds.ToString("00") + ":" + lapSw.Elapsed.Milliseconds.ToString("000");
		*/
	}

	public void Sw(int lap) {
		lapSw.Stop();
		if (lap - 1 < m_goalLap) {
			lapTimes[lap - 1] = lapSw;
			//lapTimes[lap - 1] = lap.ToString().PadLeft(2) + " " + lapSw.Elapsed.Minutes.ToString("00") + ":" + lapSw.Elapsed.Seconds.ToString("00") + ":" + lapSw.Elapsed.Milliseconds.ToString("000");
			// 表示用配列に収まる時
			if (lap - 1 < playerInfo.laptimes.Length) {
				// タイムを文字列に変換
				playerInfo.laptimes[(lap - 1)].text = 
					lap.ToString().PadLeft(2) + " " + lapTimes[lap - 1].Elapsed.Minutes.ToString("00") + ":" + lapTimes[lap - 1].Elapsed.Seconds.ToString("00") + ":" + lapTimes[lap - 1].Elapsed.Milliseconds.ToString("000");
				//playerInfo.laptimes[(lap - 1)].text = lapTimes[lap - 1];
			}
			else {
				// 要素を入れ替える
				//SwapArray(lap);
			}
			lapSw.Reset();
			lapSw.Start();
		}
	}

	// ラップタイム計測開始
	public void LapSwstart() {
		lapSw.Reset();
		lapSw.Start();
	}

	// 総合タイム計測開始
	public void TotalSwStart() {
		Totalsw.Reset();
		Totalsw.Start();
	}

	// 総合タイム停止
	public void TotalSwStop() {
		Totalsw.Stop();
	}
	
	// ゴールまでのラップを取得
	public void Setlap(int goalLap) {
		m_goalLap = goalLap;
		lapTimes = new System.Diagnostics.Stopwatch[m_goalLap];
	}

	/*
	// lapTimesの要素を1つずらす
	public void SwapArray(int lap) {
		for (int i = 0; i < laptimes.Length-1; i++){
			laptimes[i].text = laptimes[i+1];
		}
		laptimes[laptimes.Length-1].text = laptimes[lap - 1].text;
		//LtimeText[LtimeText.Length-1].text = "00:00:000";
	}
	*/
}
