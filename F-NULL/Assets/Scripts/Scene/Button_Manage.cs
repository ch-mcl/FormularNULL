using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Manage : MonoBehaviour {
	public Button[] button;
	public SceneManage scenemanage;
	public AudioSource audiosource;
	public AudioClip select;
	public AudioClip press;

	//ボタンのループを自動的に行う

	//有効化されると最初のボタンにフォーカスする
	void OnEnable() {
		audiosource = GetComponent<AudioSource>();
		scenemanage = GameObject.FindObjectOfType<SceneManage>();
		button = GetComponentsInChildren<Button>();

		button[0].Select();
		for (int i = 0; i < button.Length; i++)
		{
			Navigation nv = new Navigation();
			nv.mode = Navigation.Mode.Explicit;
			//次のボタンを最後のボタンに設定する
			if (i == 0)
			{
				nv.selectOnUp = button[button.Length - 1];
				nv.selectOnDown = button[i + 1];
			}
			else {
				//次のボタンを最初のボタンに設定する
				if (i == button.Length - 1) {
					nv.selectOnDown = button[0];
					nv.selectOnUp = button[i - 1];
				}
				else {
					nv.selectOnDown = button[i + 1];
					nv.selectOnUp = button[i - 1];
				}
			}
			button[i].navigation = nv;
		}
	}

	// カーソルがあった時に再生する
	public void selectButton() {
		audiosource.clip = select;
		audiosource.Play();
	}

	// ボタン押下時の処理
	public void pressButton(string str) {
		audiosource.clip = press;
		audiosource.Play();
		scenemanage.ButtonAction(str);
	}
}
