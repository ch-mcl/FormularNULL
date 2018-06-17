using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour {
	[Header("Misc")]
	[SerializeField] bool lapBooster; // ブースト使用権限
	[SerializeField] bool booster; // ブースト使用判定
	[SerializeField] float flyHeight = 0.6f; // 飛行高 うまくいってないぞ... なんで?

	[Header("Orients")]
	[SerializeField] GameObject particle;
	[SerializeField] GameObject playerobj;
	[SerializeField] GameObject playermdl;
	[SerializeField] GameObject playerspin;
	public GameObject explosion;
	[SerializeField] Transform[] orients;
	[SerializeField] RaycastHit[] hits = new RaycastHit[4];
	[SerializeField] LayerMask mask;

	// ブースト
	float minBoostHp; // ブースト可能最低値
	float minSpinBoostHp; // スピンブースター可能最低値
	float boostPower = 0f; // ブースト力

	// 反重力
	float downforce = 200f; // 機体を地面に引く力
	float airdownforce = 50f; // 機体を地面に引く力
	float downForceHeight = 2f; // 重力が有効になる　地面からの高度
	float falltime = 0f; // 落下時間 
	Vector3 m_appliedDownForce; // 落下距離から計算した落下力

	bool GDiffuse = false; // マシン浮遊状態 (カウントダウンもしくは未入力時)

	// 物理演算時に使う値達(変更するとゲーム壊れる)
	float sideAttckForwardEnhance = 1000f; // サイドアタック時　機体を左右に動かす為の係数
	float sideAttckEnhance = 20000f; // サイドアタック時　機体を左右に動かす為の係数

	float slipEnhance = 10f; // スリップ時に　機体を動かす為の係数
	float slideEnhance = 100f; // スライド時に　機体を動かす為の係数
	float thrustEnhance = 1000f; // 前進時に　機体を動かす為の係数
	float minSlideVel = 2.7f; // スライド可能な最低速度 およそ1km/h

	// ブースト基本値
	// 基本ブースト力 * (機体性能 + ブースト力調整値)
	float boostForce = 50f; // 基本ブースト力
	float boostEnhance = 0.5f; // ブースト力調整値
	float bootDecrease = 50f; // ブースト力低下

	// 各種回転基本値(変更するとゲーム壊れる)
	float steerSlipAngle = 2f; // スリップ時における機体y軸回転角(ヨー)角度の係数 (値倍で旋回)
	float steerRoll = 6f; // 旋回時における機体の機体のz軸回転角(ロール)値
	float slideRoll = 12f; // スライド時における機体のz軸回転角(ロール)値 
	float steerLerp = 0.1f; // 旋回前と旋回後の角度の　補間値
	float pitchAngle = 2f; // 機首上下(ピッチ)時の
	float normalLerp = 0.25f;
	float gripAngle = 8f; // グリップ最大角上限

	public float m_vel; // 速度
	float healTime; // 回復

	// 機体振動関連
	float shakeLowAngle = 2.4f; // 上昇時機首の最大角
	float shakeAngle = 0.4f; // 機首の上昇する角度
	float shakeFrequency = 64f; // 振動の周期


	bool slipping; // スリップ状態(false だと グリップ)
	float m_Oldrotation; // 1フレーム前の機体y軸回転角(ヨー)角度
	
	// 物理用の変数達
	float m_forwardforce; // 推進力
	

	// ここの ステアリング力 は地雷
	// ステアリング力 (ドリフト時に使用)
	float m_steerForceForward;
	float m_steerForceSide;

	// ステアリングに対する摩擦力 (ドリフト時に使用)
	float m_steerSlipForward;
	float m_steerSlipSide;


	// スライド力(右)
	float m_slideRForceForward;
	float m_slideRForceSide;

	// スライド力(右)に対する摩擦力 (ドリフト時に使用)
	float m_slideRSlipForward;
	float m_slideRSlipSide;

	// スライド力(左)
	float m_slideLForceForward;
	float m_slideLForceSide;

	// スライド力(左)に対する摩擦力 (ドリフト時に使用)
	float m_slideLSlipForward;
	float m_slideLSlipSide;

	// PlayerContoroller, AIContorollerから受け取る情報
	bool accell = false;
	bool brake = false;
	bool slideR = false;
	bool slideL = false;
	bool sideAttackR = false;
	bool sideAttackL = false;
	bool steer = false;
	bool onAir = false;
	bool spinAttack = false;

	// キャッシュ用
	Rigidbody m_rigidbody;
	Vehicle m_vehicle;

	// 2週目以降にブースト使用可能にする為のプロパティ
	public bool LapBooster {
		set { lapBooster = value; }
	}


	// 初期化
	void Start () {
		m_rigidbody = GetComponent<Rigidbody>(); // RigidBodyをキャッシュ
		m_vehicle = GetComponent<Vehicle>(); // Vehicleをキャッシュ
		
		m_rigidbody.mass = m_vehicle.weight / 1000; // 機体重量 / 1000　が機体重量
		m_rigidbody.centerOfMass = Vector3.zero - transform.up * flyHeight; // 重心の設定

		// ブースト使用可能HP計算　おかしい...
		// 最低ブースト使用可能値の設定
		minBoostHp = 2000f / m_vehicle.body; 
		minBoostHp = minBoostHp * 0.004f;

		// 最低スピンブースター使用可能値の設定
		minSpinBoostHp = 2000f / m_vehicle.body;
		minSpinBoostHp = minSpinBoostHp * 0.004f;

		playermdl.transform.localPosition = new Vector3(0f, -flyHeight, 0f);
	}

	void Update() {
		// 重心位置の変更
		m_rigidbody.centerOfMass = Vector3.zero - transform.up * flyHeight;

		// 生存判定
		// hpが0もしくは　死亡高度(y:-100)の場合に適用 
		if (m_vehicle.hp <= 0f || gameObject.transform.position.y < -100f) {
			m_vehicle.hp = 0f;
		}
		
		// 反重力
		AntiGravity();

		m_vel = Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, transform.forward));

		// スリップ処理(ドリフトターンに関連する)
		if(steer) Slip();

		// 機体振動 浮遊時適用
		if(GDiffuse) Shake();
	}

	// 物理用処理
	// 0f はその方向へは力をかけないことを意味
	void FixedUpdate() {
		// 反重力の適用
		m_rigidbody.AddForce(m_appliedDownForce, ForceMode.Force);

		// 衝突時の吹っ飛び用処理 まだない

		// 0km/hで勝手に動かない為の処理
		// およそ1km/h以下にて停止
		// 0.2(s/m) = 0.72km/h
		if (m_vel <= 0.2) {
			Vector3 r_vel = m_rigidbody.velocity;
			r_vel.z = 0f;
			m_rigidbody.velocity = r_vel;
		}

		if (accell) {
			// ステア用の処理
			if (steer) SteerHelper();

			// スリップ時の処理
			if (slipping) {
				m_rigidbody.AddRelativeForce(
					m_steerSlipSide+m_slideRForceSide+ m_slideLForceSide * slipEnhance, 
					0f, 
					m_steerSlipForward+m_slideRForceForward+ m_slideLForceForward * slipEnhance, ForceMode.Force);
			}

			// サイドアタック時に動く為の処理
			if (sideAttackR) {
				m_rigidbody.AddRelativeForce(
					m_forwardforce * sideAttckEnhance, 
					0f, 
					-m_forwardforce * sideAttckForwardEnhance, ForceMode.Acceleration);
			}
			if (sideAttackL){
				m_rigidbody.AddRelativeForce(
					-m_forwardforce * sideAttckEnhance, 
					0f, 
					-m_forwardforce * sideAttckForwardEnhance, ForceMode.Acceleration);
			}

			// 加速用処理
			if (!brake) {
				m_rigidbody.AddRelativeForce(
				0f, 
				0f, 
				m_forwardforce * thrustEnhance, ForceMode.Force);
			}

			// ブースト用処理
			if(boostPower > 0) m_rigidbody.AddRelativeForce(
				0f, 
				0f, (m_forwardforce * boostPower * thrustEnhance), ForceMode.Force);
		}

		// スライド処理 (
		// 0Km/hにおけるスライドを防止するためスライド可能速度か判定
		if(m_vel >= minSlideVel) {
			if (slideR) {
				m_rigidbody.AddRelativeForce(
					m_slideRForceSide * slideEnhance, 
					0f, 
					m_slideLForceForward * slideEnhance, ForceMode.Acceleration);
			}
			if (slideL) {
				m_rigidbody.AddRelativeForce(
					m_slideLForceSide * slideEnhance, 
					0f, 
					m_slideRForceForward * slideEnhance, ForceMode.Acceleration);
			}
		}
		if (brake) {
			// 0.96 は減少させる為の値
			m_rigidbody.velocity *= 0.96f;
		}
	}

	// 要はミス　壁から吹っ飛ばす為の何かが必用
	void OnCollisionEnter(Collision coll) {
		foreach(ContactPoint cont in coll.contacts) { 
			if (coll.gameObject.tag == "wall") {
				//GetComponent<VehicleEffect>().HitEffect(cont);
				float dmg = m_vel * (1f - m_vehicle.body);
				dmg = dmg * 0.1f;
				/*
				float dmg = m_vel / m_vehicle.body;
				dmg = dmg * 0.05f;
				*/
				DamageHP(dmg);
			}
		}
	}

	// PlayerContoroller, AIContorollerから受け取った情報を処理
	public void MoveVehicle(bool move_accel, bool move_brake, bool move_boost, float move_steer, bool move_slideR, bool move_slideL, bool move_sideattackR, bool move_sideattackL, bool move_spin, float move_pitch) {
		// 機体のロール(y軸回転)をリセット
		playerobj.transform.localEulerAngles = Vector3.zero;

		float nowPower = m_vel / m_vehicle.maximumSpeed;

		// アクセル
		if (move_accel && m_vel < m_vehicle.maximumSpeed) {
			m_forwardforce = m_vehicle.agv_perform.Evaluate(nowPower);
			accell = true;
		} else {
			accell = false;
		}
		
		// ブレーキ
		if (move_brake) {
			brake = true;
		} else {
			brake = false;
		}

		// ブースタ関連
		if (boostPower > 0f) {
			// ブースト力低下 20は定数
			float decreaseBoost = Mathf.Sin(Time.deltaTime) * bootDecrease;
			boostPower -= decreaseBoost;
		} else {
			boostPower = 0f;
			booster = true;
		}

		if (move_boost) {
			if (lapBooster && booster && m_vehicle.hp > minBoostHp){
				booster = false;
				float bDamage;
				float bPower;
				//スピンブースタ時
				if ((spinAttack == true) && (m_vehicle.hp > minSpinBoostHp)) {
					bDamage = 16f * (1 + (1-m_vehicle.body));
					// 通常のブーストの2倍 + 機体性能値
					bPower = boostForce * (m_vehicle.boost + boostEnhance * 2);
				} else{
					bDamage = 16f;
					bPower = boostForce * (m_vehicle.boost + boostEnhance);
				}
				DamageHP(bDamage); // 体力減少
				Boost(bPower); // ブースト適応
			}
		}


		// ステアリング
		if (Mathf.Abs(move_steer) > 0) {
			steer = true;

			// ステア
			m_steerForceForward = -nowPower / m_vehicle.steering;
			m_steerForceSide = m_forwardforce * steerSlipAngle * move_steer;

			// ステア　スリップ(こいつが問題)
			m_steerSlipForward = nowPower / m_vehicle.steering;
			m_steerSlipSide = -m_forwardforce * (1- m_vehicle.grip) * move_steer;

			// 機体のy軸回転(ヨー)
			// スリップ時はどのマシンも2倍
			if (slipping) {
				var rot = Quaternion.AngleAxis(move_steer * (m_vehicle.steering * steerSlipAngle), transform.up);
				rot = rot * transform.rotation;
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, steerLerp);
			} else {
				var rot = Quaternion.AngleAxis(move_steer * m_vehicle.steering, transform.up);
				rot = rot * transform.rotation;
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, steerLerp);
			}

			// 機体のz軸回転(ロール)
			playerobj.transform.Rotate(Vector3.forward, -steerRoll * move_steer);
		} else  {
			steer = false;
			m_steerForceForward = m_steerForceSide = 0f;
		}

		// スライド
		// スライド右
		if (move_slideR) {
			// 機体のz軸回転(ロール)
			playerobj.transform.Rotate(Vector3.forward, -slideRoll);

			// スライド右
			m_slideRForceForward = -nowPower * m_vehicle.grip;
			m_slideRForceSide = nowPower * (1 + m_vehicle.grip);

			// ドリフト関連
			m_slideRSlipForward = nowPower / m_vehicle.grip;
			m_slideRSlipSide = -nowPower * (1 + m_vehicle.grip);
				
			slideR = true;

		} else {
			slideR = false;
			m_slideRForceForward = 0f;
			m_slideRForceSide = 0f;
			m_slideRSlipForward = 0f;
			m_slideRSlipSide = 0f;
		}

		// スライド左
		if (move_slideL) {
			// 機体のz軸回転(ロール)
			playerobj.transform.Rotate(Vector3.forward, slideRoll);

			// スライド左
			m_slideLForceForward = -nowPower * m_vehicle.grip;
			m_slideLForceSide = -nowPower * (1 + m_vehicle.grip);

			// ドリフト関連
			m_slideLSlipForward = nowPower / m_vehicle.grip;
			m_slideLSlipSide = nowPower * (1 + m_vehicle.grip);
				
			slideL = true;
		} else {
			slideL = false;
			m_slideLForceForward = 0f;
			m_slideLForceSide = 0f;
			m_slideLSlipForward = 0f;
			m_slideLSlipSide = 0f;
		}
		
		// サイドアタック右
		if (move_sideattackR) {
			GetComponent<VehicleAudio>().SideAttack();
			sideAttackR = true;
			//m_forwardforce *= 0.02f;
		} else {
			sideAttackR = false;
		}

		// サイドアタック左
		if (move_sideattackL) {
			GetComponent<VehicleAudio>().SideAttack();
			sideAttackL = true;
			//m_forwardforce *= 0.02f;
		} else {
			sideAttackL = false;
		}

		// スピン
		if (move_spin) {
			spinAttack = true;

			float spinAngle = Mathf.Clamp(10 * Time.time, 30, 45);

			var rot = Quaternion.AngleAxis(spinAngle, playerobj.transform.up);
			rot = rot * playerspin.transform.rotation;
			playerspin.transform.rotation = rot;

		} else {
			spinAttack = false;

			playerspin.transform.localEulerAngles = Vector3.zero;
		}

		
		// ピッチ操作(空中)
		if (onAir) {
			var rot = Quaternion.AngleAxis(move_pitch * (m_vehicle.steering * pitchAngle), transform.right);
			rot = rot * transform.rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.1f);
		}
	}

	IEnumerator Hover() {
		playermdl.transform.localPosition = new Vector3(
				playermdl.transform.localPosition.x,
				Mathf.Lerp(playermdl.transform.localPosition.y, 0, 0.2f),
				playermdl.transform.localPosition.z);
		yield return null;
	}

	// カウントダウン時における マシン浮遊
	public void StartHovr() {
		GDiffuse = playermdl.transform.localPosition.y >= -0.1;

		// 未浮遊状態
		if (!GDiffuse) {
			StartCoroutine("Hover");
		}
	}

	// 反重力
	void AntiGravity() {
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 3.0f);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
			if (hit.distance < downForceHeight) {
				onAir = false;
				m_appliedDownForce = -transform.up * Mathf.Clamp(m_vel * downforce, 1000, downforce);
				falltime = 0f;


				// Normalに添わせる
				// まだ修正が必用
				// 前方の1つだけでnormal取得すれば良いかも？
				Debug.DrawLine(transform.position, transform.position + hit.normal, Color.cyan);
				for (int i = 0; i < orients.Length; i++)
				{
					Physics.Raycast(orients[i].position, -transform.up, out hits[i]);
					Debug.DrawRay(hits[i].point, hits[i].normal);
					var q = Quaternion.FromToRotation(transform.up, hits[i].normal);
					q = q * transform.rotation;
					transform.rotation = Quaternion.Lerp(transform.rotation, q, normalLerp);
				}

			}
			else {
				// 地面から遠すぎる場合(空中)
				// 地面へと落下する
				onAir = true;
				float proportionalHeight = (hit.distance - downForceHeight);
				proportionalHeight = Mathf.Clamp(proportionalHeight, 0, 10);
				m_appliedDownForce = -transform.up * proportionalHeight * airdownforce;
				falltime += Time.fixedDeltaTime;
			}
		}
		else {
			// 地面が存在しない場合
			// 死亡判定領域まで落とす
			onAir = true;
			m_appliedDownForce = Vector3.down * m_rigidbody.velocity.magnitude;
		}
	}

	// 旋回(物理用)
	void SteerHelper() {
		if (Mathf.Abs(m_Oldrotation - transform.eulerAngles.y) < gripAngle * m_vehicle.grip) {
			// グリップが良いと旋回時のスピードロスが少ない
			float turnadjust = (transform.eulerAngles.y - m_Oldrotation) * m_vehicle.grip;

			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);

			m_rigidbody.velocity = velRotation * m_rigidbody.velocity;
		}
		m_Oldrotation = transform.eulerAngles.y;
	}

	// スリップ
	// F-NULL　にとって重要な処理
	void Slip() {
		// 最低スリップの取得
		float slipmin = Mathf.Abs(m_steerForceSide) + m_slideRForceSide + Mathf.Abs(m_slideLForceSide);
		// スリップの計算
		float abslip = Mathf.Abs(m_steerSlipSide) + Mathf.Abs(m_slideLSlipSide) + Mathf.Abs(m_slideRSlipSide);
		// スリップ閾値の取得
		float slipthreshold = slipmin * m_vehicle.grip;

		// スリップ中
		slipping = abslip > slipthreshold;

		//Debug.Log("grip: " + slipping + " abslip: " + abslip + " threshold: " + slipthreshold);

		if (slipping) {
			particle.GetComponent<ParticleSystem>().Play();
		} else {
		// グリップ中
			particle.GetComponent<ParticleSystem>().Stop();
			m_steerSlipForward = 0f;
			m_steerSlipSide = 0f;
			m_slideRSlipForward = 0f;
			m_slideRSlipSide = 0f;
			m_slideLSlipForward = 0f;
			m_slideLSlipSide = 0f;
		}
	}

	// 機体振動
	void Shake() {
		// Mario Kart Style (モデルをランダムに拡大縮小する)
		// playermdl.transform.localScale = Vector3.one * Random.Range(1f, 1.01f);


		// F-ZERO X (上下する)
		// 0.05は上下幅
		//float mdlPos = Mathf.PingPong(Time.time, 0.05f) * m_vel / m_vehicle.maximumSpeed;
		//playermdl.transform.localPosition = Vector3.up * mdlPos;

		
		// F-ZERO GX / DAYTONA USA / SEGA AM2 (機体のピッチが変わることにより機首が上下する)

		float noseAngle = m_vel / m_vehicle.maximumSpeed;
		noseAngle = -noseAngle * (shakeLowAngle + shakeAngle * Mathf.Sin(Time.time * shakeFrequency));

		var rot = Quaternion.AngleAxis(noseAngle, playerobj.transform.right);
		rot = rot * playerobj.transform.rotation;
		playermdl.transform.rotation = rot;

	}

	// HP減少
	public void DamageHP(float value) {
		m_vehicle.hp -= value;
	}

	// HP回復
	// 100は上限値
	public void HealthHP(float value) {
		if (m_vehicle.hp >= 100f) {
			m_vehicle.hp = 100f;
		}
		else {
			m_vehicle.hp += value;
		}
	}

	// ブースト
	public void Boost(float bPower) {
		GetComponent<VehicleAudio>().Boost();
		float nowPower = m_vel / m_vehicle.maximumSpeed;
		boostPower = bPower * nowPower; // 最高速から遠いと弱いブースト
		//boostPower = bPower;
	}

	// マシン表示非表示
	public void VehicleShow(bool showing) {
		playerobj.SetActive(showing);
	}

	// モーメントや重心、RayCastの表示
	void OnDrawGizmos() {
		// 重心(Center of Mass)表示
		// Gizmos.DrawSphere(transform.position + GetComponent<Rigidbody>().centerOfMass, 0.2f);

		// 前進 forward
		Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
		Vector3 gizForward = transform.forward * m_forwardforce;
		Gizmos.DrawRay(transform.position, gizForward);

		// ステアリング steerforce
		Gizmos.color = Color.red;
		Vector3 gizSteer = transform.rotation * new Vector3(m_steerForceSide, 0f, m_steerForceForward);
		Gizmos.DrawRay(transform.position, gizSteer);

		// ステアリングスリップ slipforce
		Gizmos.color = Color.cyan;
		Vector3 gizSteerSlip = transform.rotation * new Vector3(m_steerSlipSide, 0f, m_steerSlipForward);
		Gizmos.DrawRay(transform.position, gizSteerSlip);

		// スライド slideforce
		Gizmos.color = Color.yellow;
		// 右スライド
		Vector3 gizSlideR = transform.rotation * new Vector3(m_slideRForceSide, 0f, m_slideRForceForward);
		Gizmos.DrawRay(transform.position, gizSlideR);
		// 左スライド
		Vector3 gizSlideL = transform.rotation * new Vector3(m_slideLForceSide, 0f, m_slideLForceForward);
		Gizmos.DrawRay(transform.position, gizSlideL);

		// スライドスリップ 
		Vector3 gizSlideSlip = transform.rotation * new Vector3(m_slideRSlipSide + m_slideLSlipSide, 0f, m_slideRSlipForward + m_slideLSlipForward);
		Gizmos.color = new Color(0.5f, 0.4f, 0.0f);
		Gizmos.DrawRay(transform.position, gizSlideSlip);

		// 合力
		if (slipping) {
			// スリップ時　黒
			/*
			Gizmos.color = new Color(0.0f, 0.0f, 0.0f);
			Gizmos.DrawRay(transform.position, gizForward + gizSteer + gizSlideR + gizSlideL);
			*/
		} else {
			// グリップ時 白
			/*
			Gizmos.color = Color.white;
			Gizmos.DrawRay(transform.position, (gizForward + gizSteer + gizSteerSlip + gizSlideR + gizSlideL + gizSlideSlip));
			*/
		}
	}
}
