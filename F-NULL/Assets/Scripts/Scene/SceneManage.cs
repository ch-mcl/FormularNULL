using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Scene変更
public class SceneManage : MonoBehaviour {
	[SerializeField] Pause_Manage pauseManage;

	public static string totalT;
	public static string[] lapT;

	string sceneName;

	void Start() {
		sceneName = SceneManager.GetActiveScene().name;
	}

	// ゴール処理
	public void Finish(string tt, string[] lt) {
		totalT = tt;
		lapT = lt;
		SceneManager.LoadScene("Finish");
	}

	// Buttonから文字列をもらって判定	
	public void ButtonAction(string str) {
		switch (str) {
			// レース画面へ移動
			case "gakusai":
				SceneManager.LoadScene("Gakusai");
				SceneManager.LoadScene("Main", LoadSceneMode.Additive);
				break;
			case "howto":
				SceneManager.LoadScene("HowTo");
				SceneManager.LoadScene("Main", LoadSceneMode.Additive);
				break;
			case "ice":
				SceneManager.LoadScene("Ice");
				SceneManager.LoadScene("Main", LoadSceneMode.Additive);
				break;
			// ポーズ解除
			case "resume":
				pauseManage.ChangePause();
				break;
			// 再スタート
			case "restart":
				SceneManager.LoadScene(sceneName);
				SceneManager.LoadScene("Main", LoadSceneMode.Additive);
				break;
			// タイトルへ戻る
			case "title":
				SceneManager.LoadScene("Title");
				break;
			// ゲーム終了
			case "quit":
				Application.Quit();
				break;
			// 予備(タイトルへ戻る)
			default:
				SceneManager.LoadScene("Title");
				break;
		}
	}
}
