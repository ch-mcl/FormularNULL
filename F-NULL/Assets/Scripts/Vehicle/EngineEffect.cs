using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// エフェクト
public class EngineEffect : MonoBehaviour {

	public ParticleSystem[] engines; // エンジン

	public GameObject spark; // 衝突時のエフェクト

	float m_vel;
	float maximumSpeed;
	[SerializeField] float defaultThrustersize = 0.8f; // スラスターの基本サイズ
	[SerializeField] float ThrusterDiv = 400f;
	[SerializeField] float emissionDiv = 10f;

	VehicleMover vc; // VehicleControllerのキャッシュ用

	void Start () {
		GameObject vehicle = transform.parent.parent.parent.parent.parent.gameObject;

		vc = vehicle.GetComponent<VehicleMover>();
		maximumSpeed = vehicle.GetComponent<Vehicle>().maximumSpeed; // 最高速値を取得
	}

	void Update () {
		m_vel = vc.m_vel; // 速度を取得
		ThrusterEffect();
	}

	// スラスターエフェクト
	public void ThrusterEffect() {
		float vaniasize;
		if (m_vel > maximumSpeed) {
			vaniasize = m_vel / ThrusterDiv;
		} else {
			vaniasize = defaultThrustersize;
		}

		for(int i=0; i < engines.Length; i++) {
			engines[i].GetComponent<ParticleSystem>().emissionRate = m_vel / emissionDiv;
			engines[i].GetComponent<ParticleSystem>().startSize = vaniasize;
		}
	}

	/*
	// 衝突エフェクト
	public void HitEffect(ContactPoint cont) {
		GameObject f = (GameObject)Instantiate(spark, cont.point, Quaternion.identity);
		f.name = "spark";
		f.transform.rotation = transform.rotation;
		Destroy(f, 0.5f);
	}
	*/
}
