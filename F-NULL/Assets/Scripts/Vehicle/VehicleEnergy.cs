using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEnergy : MonoBehaviour {
	[SerializeField]
	GameObject explosion;

	float energy; // 
	float barWidth; // 体力バー

	Vehicle vehilce;
	PlayerMessage message;

	void Start () {
		vehilce = GetComponent<Vehicle>();
		message = GetComponent<PlayerMessage>();
	}

	void Update () {
		energy = vehilce.hp;

		// エネルギーが0より減った
		// 「ぶっ壊れた」と表示し、爆発
		if (energy < 0f) {
			message.Sender("Broken Down");
			// マシン壊れる
			FallDeath();
		}

		// 高度-100より下へ落ちた時
		// 爆発
		if (gameObject.transform.position.y < -100f) {
			FallDeath();
		}

		// エネルギー25%以下で警告アラーム
		// 25 = 25%
		if (energy <= 25f && energy >= 0) {
			// アラーム再生
			// 0%に近くなるほど再生までの間が短くなる
			GetComponent<VehicleAudio>().Alarm();
		} else {
			GetComponent<VehicleAudio>().StopAlarm();
		}
	}

	// 爆発させる(ついでにマシンを非表示)
	void FallDeath() {
		GetComponent<VehicleMover>().VehicleShow(false);
		// 速度を無くす
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		// 爆発エフェクト
		explosion.SetActive(true);
		// マシンの操作を無効化
		

	}

	// 死亡
	void Death() {

	}
}
