using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour {
	/// <summary>
	/// ブースト使用権限
	/// </summary>
	[Header("Misc")]
	[SerializeField]
	bool lapBooster;

	/// <summary>
	/// ブースト使用判定
	/// </summary>
	[SerializeField]
	bool booster;

	/// <summary>
	/// 飛行高
	/// </summary>
	[SerializeField]
	float flyHeight = 0.6f;

	[Header("Orients")]
	[SerializeField]
	GameObject particle;
	[SerializeField]
	GameObject playerobj;
	[SerializeField]
	GameObject playermdl;
	[SerializeField]
	GameObject playerspin;




	#region 定数

	/// <summary>
	/// スライド可能な最低速度 およそ1km/h
	/// </summary>
	float m_minSlideVel = 2.7f;
	
	#region 物理演算時に使う値達(変更するとゲーム壊れる)
	/// <summary>
	/// 機体を左右に動かす係数(サイドアタック用)
	/// </summary>
	float m_sideAttckForwardCoefficient = 1000f;

	/// <summary>
	/// 機体を左右に動かす為の係数(サイドアタック時)
	/// </summary>
	float m_sideAttckCoefficient = 20000f;

	/// <summary>
	/// スリップ時に　機体を動かす為の係数
	/// </summary>
	float m_slipCoefficient = 10f;

	/// <summary>
	/// スライド時に　機体を動かす為の係数
	/// </summary>
	float m_slideCoefficient = 100f;

	/// <summary>
	/// 前進時に　機体を動かす為の係数
	/// </summary>
	float m_thrustCoefficient = 1000f;
	#endregion

	# region 減速関係
	/// <summary>
	/// ブレーキ係数
	/// </summary>
	float m_brakeCoefficient = 0.96f;

	/// <summary>
	/// スライドブレーキ係数
	/// </summary>
	float m_SlidebrakeCofficient = 0.98f;
	#endregion

	#region ブースト定数
	// 基本ブースト力 * (機体性能 + ブースト力調整値)

	/// <summary>
	/// 基本ブースト力
	/// </summary>
	float m_boostForce = 50f;

	/// <summary>
	/// ブースト力調整値
	/// </summary>
	float m_boostCoefficient = 0.5f;

	/// <summary>
	/// ブースト力低下
	/// </summary>
	float m_bootDecrease = 50f;

	/// <summary>
	/// ブースト消費エネルギ
	/// </summary>
	float m_boostCoast = 16f;
	#endregion

	#region 各種回転基本値(変更するとゲーム壊れる)
	/// <summary>
	/// スリップ時における機体y軸回転角(ヨー)角度の係数 (値倍で旋回)
	/// </summary>
	float m_steerSlipAngle = 2f;

	/// <summary>
	/// 旋回時における機体の機体のz軸回転角(ロール)値
	/// </summary>
	float m_steerRoll = 6f;
	 
	/// <summary>
	/// スライド時における機体のz軸回転角(ロール)値
	/// </summary>
	float m_slideRoll = 12f;

	/// <summary>
	/// 旋回前と旋回後の角度の　補間値
	/// </summary>
	float m_steerLerp = 0.1f;

	/// <summary>
	/// 機首上下(ピッチ)時の角度
	/// </summary>
	float m_pitchAngle = 2f;

	/// <summary>
	/// グリップ最大角上限
	/// </summary>
	float m_gripAngle = 8f;
	#endregion

	#region 機体振動関連
	/// <summary>
	/// 上昇時機首の最大角
	/// </summary>
	float m_shakeLowAngle = 2.4f;
	
	/// <summary>
	/// 機首の上昇する角度
	/// </summary>
	float m_shakeAngle = 0.4f;
	
	/// <summary>
	/// 振動の周期
	/// </summary>
	float m_shakeFrequency = 64f;
	#endregion

	#endregion

	#region フィールド変数
	/// <summary>
	/// マシン浮遊状態 (カウントダウンもしくは未入力時)
	/// </summary>
	bool m_hovering = false;

	/// <summary>
	/// 速度
	/// </summary>
	private float m_vel;

	/// <summary>
	/// 回復
	/// </summary>
	float m_healTime;

	/// <summary>
	/// 落下距離から計算した落下力
	/// </summary>
	Vector3 m_downForce;

	/// <summary>
	/// スリップ状態(false だと グリップ)
	/// </summary>
	bool m_slipping;

	/// <summary>
	/// 1フレーム前の機体y軸回転角(ヨー)角度
	/// </summary>
	float m_Oldrotation;

	#region ブースト
	/// <summary>
	/// ブースト可能最低値
	/// </summary>
	float m_minBoostHp;

	/// <summary>
	/// スピンブースター可能最低値
	/// </summary>
	float m_minSpinBoostHp;

	/// <summary>
	/// ブースト力
	/// </summary>
	float m_boostPower = 0f;
	#endregion

	#region 物理用の変数

	/// <summary>
	/// 推進力
	/// </summary>
	float m_forwardforce;
	

	/// <summary>
	/// ステアリング力 X方向
	/// </summary>
	float m_steerForceForward;

	/// <summary>
	/// ステアリング力 Z方向
	/// </summary>
	float m_steerForceSide;

	/// <summary>
	/// ステアリングに対する摩擦力 Z方向
	/// ドリフト時に使用
	/// </summary>
	float m_steerSlipForward;

	/// <summary>
	/// ステアリングに対する摩擦力 X方向
	/// ドリフト時に使用
	/// </summary>
	float m_steerSlipSide;

	/// <summary>
	/// スライド力(右) Z方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideRForceForward;

	/// <summary>
	/// スライド力(右) X方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideRForceSide;

	/// <summary>
	/// スライド力(右)に対する摩擦力 Z方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideRSlipForward;

	/// <summary>
	/// スライド力(右)に対する摩擦力 X方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideRSlipSide;

	/// <summary>
	/// スライド力(左) Z方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideLForceForward;

	/// <summary>
	/// スライド力(左) X方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideLForceSide;

	/// <summary>
	/// スライド力(左)に対する摩擦力 Z方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideLSlipForward;

	/// <summary>
	/// スライド力(左)に対する摩擦力 X方向
	/// ドリフト時に使用
	/// </summary>
	float m_slideLSlipSide;

	#endregion

	#region PlayerContoroller, AIContorollerから受け取る情報
	bool m_accell = false;
	bool m_brake = false;
	bool m_slideR = false;
	bool m_slideL = false;
	bool m_sideAttackR = false;
	bool m_sideAttackL = false;
	bool m_steer = false;
	bool m_onAir = false;
	bool m_spinAttack = false;
	#endregion
	#endregion

	#region キャッシュ用
	/// <summary>
	/// Rigidbodyをキャッシュ
	/// </summary>
	Rigidbody m_rigidbody;

	/// <summary>
	/// Vehicleをキャッシュ
	/// </summary>
	Vehicle m_vehicle;

	/// <summary>
	/// VehicleAntiGravityをキャッシュ
	/// </summary>
	VehicleAntiGravity vag;

	#endregion

	#region プロパティ
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
		get { return m_onAir; }
		set{ m_onAir = value; }
	}

	/// <summary>
	/// ダウンフォースの設定
	/// </summary>
	public Vector3 DownForce {
		set { m_downForce = value; }
	}

	#endregion

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
		m_minBoostHp = 2000f / m_vehicle.body; 
		m_minBoostHp = m_minBoostHp * 0.004f;

		// 最低スピンブースター使用可能値の設定
		m_minSpinBoostHp = 2000f / m_vehicle.body;
		m_minSpinBoostHp = m_minSpinBoostHp * 0.004f;

		playermdl.transform.localPosition = new Vector3(0f, -flyHeight, 0f);

		// VehicleAntiGravityをキャッシュ
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

		if(vag == null) {
			// VehicleAntiGravity取得失敗の場合

			// VehicleAntiGravityをキャッシュ
			vag = playerspin.transform.GetChild(0).GetComponent<VehicleAntiGravity>();
		}
		// Normal角度取得　とダウンフォース取得
		m_downForce = vag.AntiGravity();

		m_vel = Mathf.Abs(Vector3.Dot(m_rigidbody.velocity, transform.forward));

		// スリップ処理(ドリフトターンに関連する)
		if(m_steer) Slip();

		// 機体振動 浮遊時適用
		if(m_hovering) Shake();
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

		if (m_accell) {
			// ステア用の処理
			if (m_steer) SteerHelper();

			// スリップ時の処理
			if (m_slipping) {
				m_rigidbody.AddRelativeForce(
					m_steerSlipSide+m_slideRForceSide+ m_slideLForceSide * m_slipCoefficient, 
					0f, 
					m_steerSlipForward+m_slideRForceForward+ m_slideLForceForward * m_slipCoefficient, ForceMode.Force);
			}

			// サイドアタック時に動く為の処理
			if (m_sideAttackR) {
				m_rigidbody.AddRelativeForce(
					m_forwardforce * m_sideAttckCoefficient, 
					0f, 
					-m_forwardforce * m_sideAttckForwardCoefficient, ForceMode.Acceleration);
			}
			if (m_sideAttackL){
				m_rigidbody.AddRelativeForce(
					-m_forwardforce * m_sideAttckCoefficient, 
					0f, 
					-m_forwardforce * m_sideAttckForwardCoefficient, ForceMode.Acceleration);
			}

			// 加速用処理
			if (!m_brake) {
				m_rigidbody.AddRelativeForce(
				0f, 
				0f, 
				m_forwardforce * m_thrustCoefficient, ForceMode.Force);
			}

			// ブースト用処理
			if(m_boostPower > 0) m_rigidbody.AddRelativeForce(
				0f, 
				0f, (m_forwardforce * m_boostPower * m_thrustCoefficient), ForceMode.Force);
		}

		// スライド処理 (
		// 0Km/hにおけるスライドを防止するためスライド可能速度か判定
		if(m_vel >= m_minSlideVel) {
			if (m_slideR) {
				m_rigidbody.AddRelativeForce(
					m_slideRForceSide * m_slideCoefficient, 
					0f, 
					m_slideLForceForward * m_slideCoefficient, ForceMode.Acceleration);
			}
			if (m_slideL) {
				m_rigidbody.AddRelativeForce(
					m_slideLForceSide * m_slideCoefficient, 
					0f, 
					m_slideRForceForward * m_slideCoefficient, ForceMode.Acceleration);
			}
		}
		// ブレーキ
		if (m_brake) {
			Vector3 r_vel = m_rigidbody.velocity;

			r_vel.z = r_vel.z * m_brakeCoefficient;
			r_vel.x = r_vel.x * m_brakeCoefficient;

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
			m_accell = true;
		} else {
			m_accell = false;
		}
		
		// ブレーキ
		if (move_brake) {
			m_brake = true;
		} else {
			m_brake = false;
		}

		// ブースタ関連
		if (m_boostPower > 0f) {
			// ブースト力低下
			float decreaseBoost = m_bootDecrease;
			m_boostPower -= decreaseBoost;
		} else {
			m_boostPower = 0f;
			booster = true;
		}

		if (move_boost) {
			if (lapBooster && booster && m_vehicle.hp > m_minBoostHp){
				booster = false;
				float bDamage; // ブーストで消費するエネルギー
				float bPower; // ブースト力
				//スピンブースタ時
				if ((m_spinAttack == true) && (m_vehicle.hp > m_minSpinBoostHp)) {
					bDamage = m_boostCoast * (1 + (1-m_vehicle.body));
					// 通常のブーストの2倍 + 機体性能値
					bPower = m_boostForce * (m_vehicle.boost + m_boostCoefficient);
				} else{
					bDamage = m_boostCoast;
					bPower = m_boostForce * (m_vehicle.boost + m_boostCoefficient);
				}
				DamageHP(bDamage); // 体力減少
				Boost(bPower); // ブースト適応
			}
		}


		// ステアリング
		if (Mathf.Abs(move_steer) > 0) {
			m_steer = true;

			// ステア
			/*
			m_steerForceForward = -nowPower / m_vehicle.steering;
			*/
			m_steerForceSide = m_forwardforce * m_steerSlipAngle * move_steer;

			// ステア　スリップ(こいつが問題)
			/*
			m_steerSlipForward = nowPower / m_vehicle.steering;
			m_steerSlipSide = -m_forwardforce * (1- m_vehicle.grip) * move_steer;
			*/

			// 機体のy軸回転(ヨー)
			// スリップ時はどのマシンも2倍
			if (m_slipping) {
				var rot = Quaternion.AngleAxis(move_steer * (m_vehicle.steering * m_steerSlipAngle), transform.up);
				rot = rot * transform.rotation;
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, m_steerLerp);
			} else {
				var rot = Quaternion.AngleAxis(move_steer * m_vehicle.steering, transform.up);
				rot = rot * transform.rotation;
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, m_steerLerp);
			}

			// 機体のz軸回転(ロール)
			playerobj.transform.Rotate(Vector3.forward, -m_steerRoll * move_steer);
		} else  {
			m_steer = false;
			m_steerForceForward = m_steerForceSide = 0f;
		}

		// スライド
		// スライド右
		if (move_slideR) {
			// 機体のz軸回転(ロール)
			playerobj.transform.Rotate(Vector3.forward, -m_slideRoll);

			// スライド右

			//m_slideRForceForward = -nowPower * m_vehicle.grip;
			m_slideRForceSide = nowPower * (1 + m_vehicle.grip);

			// ドリフト関連
			//m_slideRSlipForward = nowPower / m_vehicle.grip;
			//m_slideRSlipSide = -nowPower * (1 + m_vehicle.grip);

			m_slideR = true;

		} else {
			m_slideR = false;
			m_slideRForceForward = 0f;
			m_slideRForceSide = 0f;
			m_slideRSlipForward = 0f;
			m_slideRSlipSide = 0f;
		}

		// スライド左
		if (move_slideL) {
			// 機体のz軸回転(ロール)
			playerobj.transform.Rotate(Vector3.forward, m_slideRoll);

			// スライド左
			//m_slideLForceForward = -nowPower * m_vehicle.grip;
			m_slideLForceSide = -nowPower * (1 + m_vehicle.grip);

			// ドリフト関連
			//m_slideLSlipForward = nowPower / m_vehicle.grip;
			//m_slideLSlipSide = nowPower * (1 + m_vehicle.grip);

			m_slideL = true;
		} else {
			m_slideL = false;
			m_slideLForceForward = 0f;
			m_slideLForceSide = 0f;
			m_slideLSlipForward = 0f;
			m_slideLSlipSide = 0f;
		}
		
		// サイドアタック右
		if (move_sideattackR) {
			GetComponent<VehicleAudio>().SideAttack();
			m_sideAttackR = true;
		} else {
			m_sideAttackR = false;
		}

		// サイドアタック左
		if (move_sideattackL) {
			GetComponent<VehicleAudio>().SideAttack();
			m_sideAttackL = true;
		} else {
			m_sideAttackL = false;
		}

		// スピン
		if (move_spin) {
			m_spinAttack = true;

			float spinAngle = Mathf.Clamp(10 * Time.time, 30, 45);

			var rot = Quaternion.AngleAxis(spinAngle, playerobj.transform.up);
			rot = rot * playerspin.transform.rotation;
			playerspin.transform.rotation = rot;

		} else {
			m_spinAttack = false;

			playerspin.transform.localEulerAngles = Vector3.zero;
		}

		
		// ピッチ操作(空中)
		if (m_onAir) {
			var rot = Quaternion.AngleAxis(move_pitch * (m_vehicle.steering * m_pitchAngle), transform.right);
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
		m_hovering = playermdl.transform.localPosition.y >= -0.1;

		// 未浮遊状態
		if (!m_hovering) {
			StartCoroutine("Hover");
		}
	}

	
	/// <summary>
	/// 旋回
	/// </summary>
	void SteerHelper() {
		if (Mathf.Abs(m_Oldrotation - transform.eulerAngles.y) < m_gripAngle * m_vehicle.grip) {
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
		m_slipping = abslip > slipthreshold;

		if (m_slipping) {
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
		noseAngle = -noseAngle * (m_shakeLowAngle + m_shakeAngle * Mathf.Sin(Time.time * m_shakeFrequency));

		var rot = Quaternion.AngleAxis(noseAngle, playerobj.transform.right);
		rot = rot * playerobj.transform.rotation;
		playermdl.transform.rotation = rot;

	}

	/// <summary>
	/// HP減少
	/// </summary>
	/// <param name="value"></param>
	public void DamageHP(float value) {
		float damage = value;
		m_vehicle.hp -= damage;
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
			float heal = value;
			m_vehicle.hp += heal;
		}
	}

	/// <summary>
	/// ブースト
	/// </summary>
	/// <param name="bPower"></param>
	public void Boost(float bPower) {
		GetComponent<VehicleAudio>().Boost();
		float nowPower = m_vel / m_vehicle.maximumSpeed;
		m_boostPower = bPower * nowPower; // 最高速から遠いと弱いブースト
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
		if (m_slipping) {
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
