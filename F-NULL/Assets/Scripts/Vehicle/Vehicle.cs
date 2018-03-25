using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// マシンステータス
// 最終的には外部DBから各種パラメータが変更される

// 3属性値について
// Body、Boost、Gripの3種類 0.0~1.0まで
// 評価は0.2ポイント毎に上がり　最大でA
// 0.0以上:E, 0.2以上:D, 0.4以上:C, 0.6以上:B, 0.8以上:A, 1.0:S(Aとして処理)

public class Vehicle : MonoBehaviour {

	[SerializeField] DBLoadVehicle loader;
	public int vehicleID;

	[Header("Performance")]
	public float maximumSpeed; // 最高速度
	public AnimationCurve agv_perform; // マシン加速パワーグラフ

	[Header("Energy")]
	public float hp = 100f; // エネルギー

	[Header("Weight")]
	public float weight = 1.0f; // 未設定時 1 kg

	[Header("Body")]
	[Range(0f, 1f)]
	public float body; // 耐久値

	[Header("Boost")]
	[Range(0f, 1f)]
	public float boost; // ブースト力

	[Header("Grip")]
	[Range(0f, 1f)]
	public float grip; // グリップ力

	[Header("Steering")]
	[Range(1f, 20f)]
	public float steering; // ステアリング速度

	[SerializeField] Transform model_root;

	void Start() {
		DBVehicle dbVehicle = loader.GetVehicleStatus(vehicleID);

		maximumSpeed = dbVehicle.m_maxSpeed;
		weight = dbVehicle.m_weight;
		body = dbVehicle.m_body;
		boost = dbVehicle.m_boost;
		grip = dbVehicle.m_grip;
		steering = dbVehicle.m_steer;

		// 性能曲線の生成
		Keyframe[] ks = new Keyframe[dbVehicle.m_perfom.Length];
		float time = 1.0f / (float)(dbVehicle.m_perfom.Length - 1);
		for (int i = 0; i < dbVehicle.m_perfom.Length; i++)
		{
			float inTime = time * (float)i;
			ks[i] = new Keyframe(inTime, dbVehicle.m_perfom[i]);
		}
		AnimationCurve ac = new AnimationCurve(ks);

		for (int i=0; i< dbVehicle.m_perfom.Length; i++) {
			UnityEditor.AnimationUtility.SetKeyLeftTangentMode(ac, i, UnityEditor.AnimationUtility.TangentMode.Auto);
		}
		agv_perform = ac;

		// 機体モデルの変更
		// これでいける!
		GameObject prefab = Resources.Load(dbVehicle.m_modelpath) as GameObject;

		GameObject v  = Instantiate(prefab, transform.position, transform.rotation);

		v.transform.parent = model_root;
	}
}

