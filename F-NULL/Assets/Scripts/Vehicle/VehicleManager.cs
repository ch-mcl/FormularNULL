using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Vehicleを管理
// 順位を出すために　要素は参照される

public class VehicleManager : MonoBehaviour {

	[SerializeField] Vehicle[] vehicles;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.B)){
			vehicles[0].GetComponent<VehicleMover>().LapBooster = true;
		}
	}

	/*
	public void TextSend(int player_id, string str)
	{
		Vehicles[player_id - 1].GetComponent<MessageManager>().Sender(str);
	}
	*/
}
