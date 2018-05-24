using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacePlace : MonoBehaviour {

	[SerializeField] VehiclePlace[] vehicles; // 順位を出す対象
	[SerializeField] float[] distance; // 順位用距離配列

	[SerializeField] Dictionary<VehiclePlace, float> carDic = 
		new Dictionary<VehiclePlace, float>(); // VehiclePlace と Place 紐付ける辞書

	// Use this for initialization
	void Start () {
		/* VehicleManageから情をもらう */

		distance = new float[vehicles.Length];
		GetVehiclePlace(); // Dictionaryの初期化
		InvokeRepeating("GetVehiclePlace", 0f, 0.5f);
	}

	// 順位を計算
	/*
	void Update () {
		GetVehiclePlace();
	}
	*/
	// Car達の順位を取得
	private void GetVehiclePlace() {
		for(int i = 0; i < vehicles.Length; i++) {
			distance[i] = vehicles[i].GetDistance();
			carDic[ vehicles[i] ] = distance[i]; // Car_CheckPointとそいつのdistanceを辞書へ登録 / 上書き
		}

		System.Array.Sort(distance, ComparerByDistance);


	}

	private static int ComparerByDistance(float a, float b) {
		if (a > b) return -1;
		if (a < b) return 1;

		return 0;
	}

	// Car_CheckPointの順位取得
	public float GetPlace(VehiclePlace vehicle) {
		int place = System.Array.IndexOf(distance, carDic[vehicle]);
		return place+1;
	}
}
