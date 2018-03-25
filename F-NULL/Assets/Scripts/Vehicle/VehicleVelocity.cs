using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleVelocity : MonoBehaviour {
	Player_InfoUI playerInfo;
	VehicleMover vehicleController;

	void Start () {
		playerInfo = GetComponent<Player_InfoUI>();
		vehicleController = GetComponent<VehicleMover>();
	}
	
	void Update () {
		float vel = vehicleController.m_vel * 3.6f;
		playerInfo.messageVel.text = vel.ToString("0");
	}
}
