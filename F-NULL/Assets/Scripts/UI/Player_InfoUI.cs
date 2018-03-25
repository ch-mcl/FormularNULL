using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// レース中におけるプレイヤやGameMangeからもらった情報をUIに反映

public class Player_InfoUI : MonoBehaviour {
	[SerializeField] RaceManage raceManage;

	[Header("Message")]
	public Text message; // メッセージ用UI Text
	
	[Header("Velocity")]
	public UnityEngine.UI.Text messageVel; // 速度用UI Text

	[Header("Time")]
	public Text totalTime;
	public Text[] laptimes;

	[Header("Lap")]
	public Text lap;

	[Header("Energy")]
	[SerializeField] private RectTransform energybar;

	float barWidth; // 体力バー
	float m_energy;

	int goalLap = 0;
	Vehicle vehicle;

	void Awake() {
		//goalLap
	}

	void Start() {
		barWidth = energybar.anchorMax.x - energybar.anchorMin.x; // エネルギーバーの長さを算出
		vehicle = GetComponent<Vehicle>();

		goalLap = raceManage.GoalLap;
		lap.text = 1 + "/" + goalLap.ToString("##");
	}

	void Update () {
		// エネルギーバーの設定
		m_energy = vehicle.hp;
		energybar.anchorMax = new Vector2((m_energy * barWidth) / 100f + energybar.anchorMin.x, energybar.anchorMin.y);
	}

	// ラップ表示　更新
	public void ChengeLap(int nowLap) {
		lap.text = (nowLap+1).ToString("##") + "/" + (goalLap).ToString("##");
	}
}
