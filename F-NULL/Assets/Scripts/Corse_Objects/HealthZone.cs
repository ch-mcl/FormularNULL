using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthZone : MonoBehaviour {

	public float heal = 50f; 

	//通過した物体がプレイヤーならPlayerControlerのHealthHPに値を与える
	void OnTriggerStay(Collider c) {
		if (c.gameObject.tag.Equals("Player")) {
			//Debug.Log(c.transform.parent.parent.parent.parent.parent.parent.name);
			c.transform.parent.parent.parent.parent.parent.parent.GetComponent<VehicleMover>().HealthHP(heal * Time.deltaTime);
		}
	}
}
