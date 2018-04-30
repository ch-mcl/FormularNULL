using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// レース中におけるプレイヤやGameMangeからもらった情報をUIに反映

public class Player_InfoUI : MonoBehaviour {
	[SerializeField] RaceManage raceManage;

	[Header("Message")]
	[SerializeField] Text m_message; // メッセージ用UI Text
	
	[Header("Velocity")]
	[SerializeField] UnityEngine.UI.Text m_messageVel; // 速度用UI Text

	[Header("Time")]
	[SerializeField] Text m_totalTime;
	[SerializeField] Text[] m_laptimes;

	[Header("Lap")]
	[SerializeField] Text m_lap;

	[Header("Energy")]
	[SerializeField] private RectTransform m_energybar;


	private float barWidth; // 体力バー
	private float m_energy;

	private int goalLap = 0;
	private Vehicle vehicle;

	// プロパティ
	// 総合タイム
	public Text TotalTimeText {
		get { return m_totalTime; }
	}

	// 各ラップタイム
	public Text[] LapTimesTexts {
		get { return m_laptimes; }
	}

	// 速度表示
	public Text VelocityText {
		get { return m_messageVel; }
	}

	// メッセージ	
	public Text MessageText {
		get { return m_message; }
	}


	// 初期化処理
	void Awake() {
		//goalLap
	}

	void Start() {
		barWidth = m_energybar.anchorMax.x - m_energybar.anchorMin.x; // エネルギーバーの長さを算出
		vehicle = GetComponent<Vehicle>();

		goalLap = raceManage.GoalLap;
		m_lap.text = 1 + "/" + goalLap.ToString("##");
	}

	void Update () {
		// エネルギーバーの設定
		m_energy = vehicle.hp;
		m_energybar.anchorMax = new Vector2((m_energy * barWidth) / 100f + m_energybar.anchorMin.x, m_energybar.anchorMin.y);
	}

	// ラップ表示　更新
	public void ChengeLap(int nowLap) {
		m_lap.text = (nowLap+1).ToString("##") + "/" + (goalLap).ToString("##");
	}
}
