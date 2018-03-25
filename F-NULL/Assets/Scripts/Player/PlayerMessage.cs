using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Race_UI
public class PlayerMessage : MonoBehaviour {

	Player_InfoUI playerInfo;

	float messageTime = 2f; // メッセージが表示される時間(秒)

	void Start() {
		playerInfo = GetComponent<Player_InfoUI>();
	}

	// 2秒間表示させる
	IEnumerator MessageShow(string mssg){
		playerInfo.message.text = mssg;
		yield return new WaitForSeconds(messageTime);
		playerInfo.message.text = "";
	}

	public void Sender(string str) {
		StartCoroutine(MessageShow(str));
	}
}
