using System.Collections;
using System.Collections.Generic;
using System;

// DBLoadVehicleが各種ステータスを読み、その値を保持させる

[Serializable]
public class VehicleStatus {
	public string m_name;      // マシン名
	public float m_maxSpeed;   // 最高速度
	public int m_weight;       // 機体重量
	public float m_body;        // ボディ耐久度
	public float m_boost;       // ブースト力
	public float m_grip;        // グリップ
	public float m_steer;       // 機体旋回速度
	public string m_modelpath; // モデルへのパス
	public float[] m_perfom;	// 出力カーブ

	// コンストラクタ
	public VehicleStatus (
		string name, float maxspeed, int weight, float body, float boost, float grip, float steer, string path, float[] performs
	) {
		m_name = name;
		m_maxSpeed = maxspeed;
		m_weight = weight;
		m_body = body;
		m_boost = boost;
		m_grip = grip;
		m_steer = steer;
		m_modelpath = path;
		m_perfom = performs;
	}
}
