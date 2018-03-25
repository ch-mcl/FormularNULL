using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashZone : MonoBehaviour {

	public float boostPower = 50f;
	
	//通過した物体がプレイヤーならPlayerControlerのBoostにブースト力を与える　(ブーストさせる)
	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag.Equals("Player")) {
			c.transform.parent.parent.parent.parent.parent.parent.GetComponent<VehicleMover>().Boost(boostPower);
		}
	}
}
