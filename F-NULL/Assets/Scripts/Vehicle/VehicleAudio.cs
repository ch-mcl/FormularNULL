using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAudio : MonoBehaviour {

	VehicleMover vehicleCintroller;

	public AudioSource se_engine;
	public AudioSource se_boost;
	public AudioSource se_attack;
	public AudioSource se_alarm;

	// Use this for initialization
	void Start () {
		vehicleCintroller = GetComponent<VehicleMover>();
	}
	
	// Update is called once per frame
	void Update () {
		float enginepitch = Mathf.Abs( GetComponent<Rigidbody>().velocity.magnitude / GetComponent<Vehicle>().maximumSpeed) + 1f;
		se_engine.pitch = Mathf.Abs(enginepitch);
	}

	public void Boost() {
		se_boost.Play();
	}

	public void SideAttack() {
		se_attack.Play();
	}

	public void Alarm() {
		// 警告音　再生
		// 0に近いと再生の間隔も狭くなる
		if(!se_alarm.isPlaying){
			se_alarm.Play();
		}
	}

	public void StopAlarm() {
		if(se_alarm.isPlaying){
			se_alarm.Stop();
		}
	}
}
