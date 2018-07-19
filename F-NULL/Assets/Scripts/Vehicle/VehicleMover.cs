using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour {
	[Header("Misc")]
	[SerializeField]
	bool lapBooster; // ブースト使用権限
	[SerializeField]
	bool booster; // ブースト使用判定
	[SerializeField]
	float flyHeight = 0.6f; // 飛行高

	[Header("Orients")]
	[SerializeField]
	GameObject particle;
	[SerializeField]
	GameObject playerobj;
	[SerializeField]
	GameObject playermdl;
	[SerializeField]
	GameObject playerspin;

	// ブースト
	float minBoostHp; // ブースト可能最低値
	float minSpinBoostHp; // スピンブースター可能最低値
	float boostPower = 0f; // ブースト力

	bool hovering = false; // マシン浮遊状態 (カウントダウンもしくは未入力時)

	// 物理演算時に使う値達(変更するとゲーム壊れる)
	float sideAttckForwardCoefficient = 1000f; // サイドアタック時　機体を左右に動かす為の係数
	float sideAttckCoefficient = 20000f; // サイドアタック時　機体を左右に動かす為の係数

	float slipCoefficient = 10f; // スリップ時に　機体を動かす為の係数
	float slideCoefficient = 100f; // スライド時に　機体を動かす為の係数
	float thrustCoefficient = 1000f; // 前進時に　機体を動かす為の係数
	float minSlideVel = 2.7f; // スライド可能な最低速度 およそ1km/h

	Vector3 m_downForce; // 落下距離から計算した落下力

	// ブースト基本値
	// 基本ブースト力 * (機体性能 + ブースト力調整値)
	float boostForce = 50f; // 基本ブースト力
	float boostCoefficient = 0.5f; // ブースト力調整値
	float bootDecrease = 50f; // ブースト力低下

	// 各種回転基本値(変更するとゲーム壊れる)
	float steerSlipAngle = 2f; // スリップ時における機体y軸回転角(ヨー)角度の係数 (値倍で旋回)
	float steerRoll = 6f; // 旋回時における機体の機体のz軸回転角(ロール)値
	float slideRoll = 12f; // スライド時における機体のz軸回転角(ロール)値 
	float steerLerp = 0.1f; // 旋回前と旋回後の角度の　補間値
	float pitchAngle = 2f; // 機首上下(ピッチ)時の
	float gripAngle = 8f; // グリップ最大角上限

	private float m_vel; // 速度
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
	VehicleAntiGravity vag;

	/// <summary>
	/// 速度の取得
	/// </summary>
	public float CurrentVelocity {
		get { return m_vel; }
	}

	/// <summary>
	/// ブースト使用可能フラグの設定
	/// </summary>
	public bool LapBooster {
		set { lapBooster = value; }
	}

	/// <summary>
	/// 飛行フラグの取得
	/// </summary>
	public bool OnAir {
		get { return onAir; }
		set{ onAir = value; }
	}

	/// <summary>
	/// ダウンフォースの設定
	/// </summary>
	public Vector3 DownForce {
		set { m_downForce = value; }
	}

	/// <summary>
	/// 初期化
	/// </summary>
	void Start () {
		m_rigidbody = GetComponent<Rigidbody>(); // RigidBodyをキャッシュ
		m_vehicle = GetComponent<Vehicle>(); // Vehicleをキャッシュ
		
		m_rigidbody.mass = m_vehicle.weight / 1000; // 機体重量 / 1000　が機体重量
		m_rigidbody.centerOfMass = Vector3.zero - transform.up * flyHeight; // 重心の設定

		// TODO:ブースト使用可能HP計算　おかしい...
		// 最低ブースト使用可能値の設定
		minBoostHp = 2000f / m_vehicle.body; 
		minBoostHp = minBoostHp * 0.004f;

		// 最低スピンブースター使用可能値の設定
		minSpinBoostHp = 2000f / m_vehicle.body;
		minSpinBoostHp = minSpinBoostHp * 0.004f;

		playermdl.transform.localPosition = new Vector3(0f, -flyHeight, 0f);

		vag = playerspin.transform.GetChild(0).GetComponent<VehicleAntiGravity>();
	}


	void Update() {
		// 重心位置の変更
		m_rigidbody.centerOfMass = Vector3.zero - transform.up * flyHeight;

		// 生存判定
		// hpが0もしくは　死亡高度(y:-100)の場合に適用 
		if (m_vehicle.hp <= 0f || gameObject.transform.position.y < -100f) {
			m_vehicle.hp = 0f;
		}

		// Normal角度取得　とダウンフォース取得
		m_downForce = vag.AntiGravity();

		m_vel = Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, transform.forward));

		// スリップ処理(ドリフトターンに関連する)
		if(steer) Slip();

		// 機体振動 浮遊時適用
		if(hovering) Shake();
	}

	/// <summary>
	/// 物理用処理
	/// 0f はその方向へは力をかけないことを意味
	/// </summary>
	void FixedUpdate() {
		// 反重力の適用
		m_rigidbody.AddForce(m_downForce, ForceMode.Force);

		// 衝突時の吹っ飛び用処理 まだない

		// 0km/hで勝手に動かない為の処理
		// およそ1km/h以下にて停止
		// 0.2(s/m) = 0.72km/h
		if (m_vel <= 0.2) {
			Vector3 r_vel = m_rigidbody.velocity;
			r_vel.z = 0f;
			m_rigidbody.velocity = transform.rotation * r_vel;
		}

		if (accell) {
			// ステア用の処理
			if (steer) SteerHelper();

			// スリップ時の処理
			if (slipping) {
				m_rigidbody.AddRelativeForce(
					m_steerSlipSide+m_slideRForceSide+ m_slideLForceSide * slipCoefficient, 
					0f, 
					m_steerSlipForward+m_slideRForceForward+ m_slideLForceForward * slipCoefficient, ForceMode.Force);
			}

			// サイドアタック時に動く為の処理
			if (sideAttackR) {
				m_rigidbody.AddRelativeForce(
					m_forwardforce * sideAttckCoefficient, 
					0f, 
					-m_forwardforce * sideAttckForwardCoefficient, ForceMode.Acceleration);
			}
			if (sideAttackL){
				m_rigidbody.AddRelativeForce(
					-m_forwardforce * sideAttckCoefficient, 
					0f, 
					-m_forwardforce * sideAttckForwardCoefficient, ForceMode.Acceleration);
			}

			// 加速用処理
			if (!brake) {
				m_rigidbody.AddRelativeForce(
				0f, 
				0f, 
				m_forwardforce * thrustCoefficient, ForceMode.Force);
			}

			// ブースト用処理
			if(boostPower > 0) m_rigidbody.AddRelativeForce(
				0f, 
				0f, (m_forwardforce * boostPower * thrustCoefficient), ForceMode.Force);
		}

		// スライド処理 (
		// 0Km/hにおけるスライドを防止するためスライド可能速度か判定
		if(m_vel >= minSlideVel) {
			if (slideR) {
				m_rigidbody.AddRelativeForce(
					m_slideRForceSide * slideCoefficient, 
					0f, 
					m_slideLForceForward * slideCoefficient, ForceMode.Acceleration);
			}
			if (slideL) {
				m_rigidbody.AddRelativeForce(
					m_slideLForceSide * slideCoefficient, 
					0f, 
					m_slideRForceForward * slideCoefficient, ForceMode.Acceleration);
			}
		}
		if (brake) {
			Vector3 r_vel = m_rigidbody.velocity;
			// 0.96 は減少させる為の値
			r_vel.z = r_vel.z * 0.96f;
			r_vel.x = r_vel.x * 0.96f;

			m_rigidbody.velocity = r_vel;
		}
	}

	//TODO:壁から吹っ飛ばす為の何かが必用
	/// <summary>
	/// 衝突処理
	/// </summary>
	/// <param name="coll"></param>
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

	/// <summary>
	/// PlayerContoroller, AIContorollerから受け取った情報を処理
	/// </summary>
	/// <param name="move_accel"></param>
	/// <param name="move_brake"></param>
	/// <param name="move_boost"></param>
	/// <param name="move_steer"></param>
	/// <param name="move_slideR"></param>
	/// <param name="move_slideL"></param>
	/// <param name="move_sideattackR"></param>
	/// <param name="move_sideattackL"></param>
	/// <param name="move_spin"></param>
	/// <param name="move_pitch"></param>
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
					bPower = boostForce * (m_vehicle.boost + boostCoefficient * 2);
				} else{
					bDamage = 16f;
					bPower = boostForce * (m_vehicle.boost + boostCoefficient);
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

	/// <summary>
	/// カウントダウン時における マシン浮遊
	/// </summary>
	public void StartHovr() {
		hovering = playermdl.transform.localPosition.y >= -0.1;

		// 未浮遊状態
		if (!hovering) {
			StartCoroutine("Hover");
		}
	}

	
	/// <summary>
	/// 旋回(物理用)
	/// </summary>
	void SteerHelper() {
		if (Mathf.Abs(m_Oldrotation - transform.eulerAngles.y) < gripAngle * m_vehicle.grip) {
			// グリップが良いと旋回時のスピードロスが少ない
			float turnadjust = (transform.eulerAngles.y - m_Oldrotation) * m_vehicle.grip;

			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);

			m_rigidbody.velocity = velRotation * m_rigidbody.velocity;
		}
		m_Oldrotation = transform.eulerAngles.y;
	}

	/// <summary>
	/// スリップ
	/// </summary>
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

	/// <summary>
	/// 機体振動
	/// </summary>
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

	/// <summary>
	/// HP減少
	/// </summary>
	/// <param name="value"></param>
	public void DamageHP(float value) {
		m_vehicle.hp -= value;
	}

	/// <summary>
	/// HP回復
	/// 100は上限値
	/// </summary>
	/// <param name="value"></param>
	public void HealthHP(float value) {
		if (m_vehicle.hp >= 100f) {
			m_vehicle.hp = 100f;
		}
		else {
			m_vehicle.hp += value;
		}
	}

	/// <summary>
	/// ブースト
	/// </summary>
	/// <param name="bPower"></param>
	public void Boost(float bPower) {
		GetComponent<VehicleAudio>().Boost();
		float nowPower = m_vel / m_vehicle.maximumSpeed;
		boostPower = bPower * nowPower; // 最高速から遠いと弱いブースト
		//boostPower = bPower;
	}

	/// <summary>
	/// マシン表示非表示
	/// </summary>
	/// <param name="showing"></param>
	public void VehicleShow(bool showing) {
		playerobj.SetActive(showing);
	}

	/// <summary>
	/// モーメントや重心、RayCastの表示
	/// </summary>
	void OnDrawGizmos() {
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
