using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 順位判定用

public class VehiclePlace : MonoBehaviour {

	[SerializeField] RaceManage raceManage;

	VehicleCheckPoint vehicleCheckPoint;
	WayPoint m_wayPoint;

	// 順位判定用
	protected int m_checkPointValue = 100; // 順位用の位置を算出するcheckPoinyのウェイト
	protected int m_lapValue = 10000; // 順位用の位置を算出するlapのウェイト
	protected float m_distance = 0; // 距離

	// 初期化
	void Start () {
		// RaceManageをキャッシュ
		vehicleCheckPoint = GetComponent<VehicleCheckPoint>();
		// WayPointをキャッシュ
		m_wayPoint = raceManage.WayPoint;
	}

	// CheckPoint値とlap値を含めた距離の取得
	public float GetDistance() {
		return (transform.position - vehicleCheckPoint.CurrentWayPoint).magnitude +
			vehicleCheckPoint.CheckPoint * m_checkPointValue +
			vehicleCheckPoint.CurrentLap * m_lapValue;
	}
}
