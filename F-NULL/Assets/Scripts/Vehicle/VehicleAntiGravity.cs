using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAntiGravity : MonoBehaviour {
	[Header("Orients")]
	[SerializeField]
	Transform[] orients;

	[SerializeField]
	LayerMask mask;

	// 反重力
	float downforce = 1000f; // 機体を地面に引く力
	float airdownforce = 50f; // 機体を地面に引く力
	float downForceHeight = 2f; // 重力が有効になる　地面からの高度
	float falltime = 0f; // 落下時間 

	Vector3 m_appliedDownForce; // 落下距離から計算した落下力

	RaycastHit[] hits = new RaycastHit[4];

	VehicleMover vm; // VehicleControllerのキャッシュ用

	// 各種回転基本値
	float normalLerp = 0.25f;

	/// <summary>
	/// 初期化
	/// </summary>
	void Start() {
		GameObject vehicle = transform.parent.parent.parent.parent.gameObject;

		vm = vehicle.GetComponent<VehicleMover>();
	}

	/// <summary>
	/// 反重力
	/// </summary>
	public Vector3 AntiGravity() {
		Ray ray = new Ray(vm.transform.position, -vm.transform.up);
		RaycastHit hit;

		Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 3.0f);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
			if (hit.distance < downForceHeight) {
				vm.OnAir = false;
				m_appliedDownForce = -vm.transform.up * Mathf.Clamp(vm.CurrentVelocity * downforce, 10000, downforce);
				falltime = 0f;


				// Normalに添わせる
				// まだ修正が必用
				// 前方の1つだけでnormal取得すれば良いかも？
				Debug.DrawLine(vm.transform.position, vm.transform.position + hit.normal, Color.cyan);
				for (int i = 0; i < orients.Length; i++) {
					Physics.Raycast(orients[i].position, -vm.transform.up, out hits[i]);
					Debug.DrawRay(hits[i].point, hits[i].normal);
					var q = Quaternion.FromToRotation(vm.transform.up, hits[i].normal);
					q = q * vm.transform.rotation;
					vm.transform.rotation = Quaternion.Lerp(vm.transform.rotation, q, normalLerp);
				}

			} else {
				// 地面から遠すぎる場合(空中)
				// 地面へと落下する
				vm.OnAir = true;
				float proportionalHeight = (hit.distance - downForceHeight);
				proportionalHeight = Mathf.Clamp(proportionalHeight, 0, 10);
				m_appliedDownForce = -vm.transform.up * proportionalHeight * airdownforce;
				falltime += Time.fixedDeltaTime;
			}
		} else {
			// 地面が存在しない場合
			// 死亡判定領域まで落とす
			vm.OnAir = true;
			m_appliedDownForce = Vector3.down * vm.CurrentVelocity;
		}

		return m_appliedDownForce;
	}

}
