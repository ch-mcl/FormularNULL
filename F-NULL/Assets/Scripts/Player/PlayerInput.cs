using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
	
	[Header("0:any, 1=1p,2=2p,3=3p,4=4p")]

	[SerializeField] RaceManage raceManage;

	[SerializeField] private GamePad.Index player_id;  //自分が何人目のプレイヤーか設定する。

	[SerializeField] private float doublTapTime = 0.5f;
	[SerializeField] private Player_camera playerCamera;
	[SerializeField] private Pause_Manage pauseManage;

	VehicleMover vehicleController;

	bool m_accel; // アクセル
	bool m_brake; // ブレーキ
	bool m_boost; // ブースト
	float m_steer; // ステア
	bool m_slideR; // スライド右
	bool m_slideL; // スライド左
	bool m_sideAttackR; // サイドアタック右
	bool m_sideAttackL; // サイドアタック左
	bool m_spin; // スピンアタック
	float m_pitch; // ピッチ

	bool m_pause; // ポーズ切り替え
	bool m_camera; // カメラ切り替え

	// サイドアタック右用
	bool m_PressR;
	bool m_triggerRDown;
	float rPressTime = 0;
	// サイドアタック左用
	bool m_PressL;
	bool m_triggerLDown;
	float lPressTime = 0;

	int viewMode = 0;


	void Start () {
		vehicleController = GetComponent<VehicleMover>(); // VehicleControllerをキャッシュ
	}

	void Update() {
		GamepadState state = GamePad.GetState(player_id);

		// ポーズ
		m_pause = (GamePad.GetButtonDown(GamePad.Button.Start, player_id) || Input.GetKeyDown(KeyCode.Escape));

		// 視点切り替え
		m_camera = (GamePad.GetButtonDown(GamePad.Button.Back, player_id) || Input.GetKeyDown(KeyCode.Keypad2));

		// アクセル
		m_accel = (state.A || Input.GetKey(KeyCode.X));

		// ブレーキ
		m_brake = (state.X || Input.GetKey(KeyCode.LeftShift));

		// ブースター
		m_boost = (GamePad.GetButtonDown(GamePad.Button.B, player_id) || Input.GetKeyDown(KeyCode.Space));

		// ステアリングz
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f) {
			m_steer = Input.GetAxis("Horizontal");
		} else {
			m_steer = 0;
		}
		if (Mathf.Abs(state.LeftStickAxis.x) > 0) {
			m_steer = state.LeftStickAxis.x;
		}

		// スライド右
		m_slideR = (Mathf.Abs(state.RightTrigger) > 0 || Input.GetKey(KeyCode.C));

		// サイドアタック右
		#region SideAttack R
		// トリガー右　ワンタイム入力化
		m_triggerRDown = false;
		if (Mathf.Abs(state.RightTrigger) != 0f) {
			if (m_PressR == false) {
				m_triggerRDown = true;
				m_PressR = true;
			}
		}
		if (Mathf.Abs(state.RightTrigger) == 0f) {
			m_PressR = false;
		}

		// トリガー右　連打判定
		m_sideAttackR = false;
		if (m_triggerRDown || Input.GetKeyDown(KeyCode.C)) {

			m_sideAttackR = Time.time < rPressTime + doublTapTime;
			m_slideR = false;
			rPressTime = Time.time;
		}
		#endregion

		// スライド左
		m_slideL = (Mathf.Abs(state.LeftTrigger) > 0 || Input.GetKey(KeyCode.Z));

		// サイドアタック左
		#region sideAttack L
		// トリガー左　ワンタイム入力化
		m_triggerLDown = false;
		if (Mathf.Abs(state.LeftTrigger) != 0f) {
			if (m_PressL == false) {
				m_triggerLDown = true;
				m_PressL = true;
			}
		}
		if (Mathf.Abs(state.LeftTrigger) == 0f) {
			m_PressL = false;
		}

		// トリガー左　連打判定
		m_sideAttackL = false;
		if (m_triggerLDown || Input.GetKeyDown(KeyCode.Z)) {

			m_sideAttackL = Time.time < lPressTime + doublTapTime;
			m_slideL = false;
			lPressTime = Time.time;
		}

		/*
		m_sideAttackL = false;
		if (GamePad.GetButtonDown(GamePad.Button.LeftShoulder, player_id) || Input.GetKeyDown("q")) {
			m_sideAttackL = Time.time < lPressTime + doublTapTime;
			lPressTime = Time.time;
		}*/


		/*
		m_bumperL = false;
		if (GamePad.GetButtonDown(GamePad.Button.LeftShoulder, player_id) || Input.GetKeyDown("q")) {
			if (Time.time < lPressTime + doublTapTime) {
				m_bumperL = true;
			}
			lPressTime = Time.time;
		}

		if (m_bumperL) {
			m_sideAttackL = true;
		} else {
			m_sideAttackL = false;
		}
		*/
		#endregion

		// スピンアタック
		m_spin = (state.Y || Input.GetKey(KeyCode.Keypad1));

		// ピッチ
		//if (Mathf.Abs(state.LeftStickAxis.y) > 0) {
		//	m_pitch = state.LeftStickAxis.y;
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f) {
			m_pitch = Input.GetAxis("Vertical");
		} else {
			m_pitch = 0;
		}

		if (m_pause) {
			pauseManage.ChangePause();
		}

		if (m_camera) {
			playerCamera.ChangeView();
		}

		if (raceManage.Canmove && Time.timeScale > 0) {
			vehicleController.MoveVehicle(m_accel, m_brake, m_boost, m_steer, m_slideR, m_slideL, m_sideAttackR, m_sideAttackL, m_spin, m_pitch);
		}

		if (m_accel) vehicleController.StartHovr();
	}
}