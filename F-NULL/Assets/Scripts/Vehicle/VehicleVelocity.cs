using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleVelocity : MonoBehaviour {
	Player_InfoUI playerInfo;
	VehicleMover vc;

	void Start () {
		playerInfo = GetComponent<Player_InfoUI>();
		vc = GetComponent<VehicleMover>();
	}
	
	void Update () {
		float vel = vc.CurrentVelocity * 3.6f;
		playerInfo.VelocityText.text = vel.ToString("0");
	}
}
