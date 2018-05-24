using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// レース中におけるプレイヤーやRaceMangeからもらった情報をUIに反映

public class Player_InfoUI : MonoBehaviour {
	[SerializeField] RaceManage raceManage; // RaceManage(情報をもらうオブジェクト)
	[SerializeField] RacePlace racePlace; // 順位情報

	[Header("Message")]
	[SerializeField] Text m_message; // メッセージ用UI Text
	
	[Header("Velocity")]
	[SerializeField] UnityEngine.UI.Text m_messageVel; // 速度用UI Text

	[Header("Time")]
	[SerializeField] Text m_totalTime; // 総合タイムを表示するテキストエリア
	[SerializeField] Text[] m_laptimes; // ラップタイムを表示するテキストエリア

	[Header("Position")]
	[SerializeField] Text m_position; // ラップとゴールラップを表示するテキストエリア (現在のラップ / ゴールラップ)

	[Header("Lap")]
	[SerializeField] Text m_lap; // ラップとゴールラップを表示するテキストエリア (現在のラップ / ゴールラップ)

	[Header("Energy")]
	[SerializeField] private RectTransform m_energybar; // エネルギーバー用UI


	private float m_barWidth; // 体力バーの長さ
	private float m_energy; // 機体のエネルギー

	private int m_goalLap = 0; // ゴールラップ
	private Vehicle vehicle; // 情報をもらう機体
	private VehiclePlace vehiclePlace; // 順位を出したい機体

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
		// エネルギーバーの長さを算出
		m_barWidth = m_energybar.anchorMax.x - m_energybar.anchorMin.x;
		// 情報をもらうオブジェクトをキャッシュ
		vehicle = GetComponent<Vehicle>();
		vehiclePlace = GetComponent<VehiclePlace>();


		// ゴールラップの取得
		m_goalLap = raceManage.GoalLap;
		// ラップとゴールラップを表示
		m_lap.text = 1 + "/" + m_goalLap.ToString("##");

		m_position.text = racePlace.GetPlace(vehiclePlace).ToString("##") + "/" + "";
	}

	void Update () {
		// エネルギーバーの設定
		m_energy = vehicle.hp; // 機体のエネルギー値を取得
		// エネルギーバーに現在のエネルギー値を適用
		m_energybar.anchorMax = new Vector2(
			(m_energy * m_barWidth) / 100f + m_energybar.anchorMin.x, 
			m_energybar.anchorMin.y);

		m_position.text = racePlace.GetPlace(vehiclePlace).ToString("##") + "/" + "";
	}

	// ラップ表示　更新
	public void ChengeLap(int nowLap) {
		// ゴールしていない場合
		if (nowLap < m_goalLap) {
			// ラップ表示を更新
			m_lap.text = (nowLap + 1).ToString("##") + "/" + (m_goalLap).ToString("##");
		}
	}
}
