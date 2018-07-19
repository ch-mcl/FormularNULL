using UnityEngine;
using System.Collections;

// レース中におけるカメラ制御

public class Player_camera : MonoBehaviour {

	[SerializeField] GameObject cameraRoot; // カメラの親オブジェクト
	[SerializeField] Transform cameraTrans; // カメラのTransform
	[SerializeField] Transform[] cameraParameter; // 視点切り替え用のTransform達

	[SerializeField] Vehicle target; // 追尾対象
	[SerializeField] float changePosSpeed = 0.1f; // 視点切り替え速度

	[SerializeField] float sense = 0.1f; // カメラ回転における補間値
	[SerializeField] float defaultFOV = 80f; // デフォルトの視野 速度が上がると上昇
	[SerializeField] float zoom = 0.2f; // 視野の拡大率

	[SerializeField] float defultCamAngle = 16; // 停止状態のx角度
	[SerializeField] float runningCamAngle = 0; // 走行中のx角度


	bool around = false; // カメラ操作可能 
	float base_velocity = 340f; // 規定値 およそ 音速(340fm/s = 1224km/h)
	[SerializeField] float minZoomVel = 27f; // FOVの上昇を開始する速度　27m/s = 100km/h
	int viewType = 0; // 視点番号

	VehicleMover vm; // VehicleMoverキャッシュ用

	void Start() {
		vm = target.GetComponent<VehicleMover>(); // VehicleMoverのキャッシュ
	}

	void Update () {
		transform.position = target.transform.position;

		// FOV拡大度を計算
		float zoomRatio; // カメラに適用する拡大率
		float vel = Mathf.Abs(vm.CurrentVelocity);
		// 100(km/h)以上で走行しているか判定
		if(vel > minZoomVel) {
			float clampdVel = Mathf.Clamp(vel, 0, base_velocity*2);
			zoomRatio = (clampdVel * zoom / base_velocity) + 1f;
		} else {
			zoomRatio = 1f;
		}

		// カメラ操作モード用
		// TODO:PlayerController から情報をもらいなさい
		GamepadInput.GamepadState state = GamepadInput.GamePad.GetState(GamepadInput.GamePad.Index.Any);
		if (Input.GetKeyDown(KeyCode.Q)) {
			// カメラ操作　有効/無効　切替
			if (!around) {
				around = true;
			} else {
				around = false;
			}
		}

		// カメラ操作可能時処理
		if (around){
			// X方向に回す
			if (state.rightStickAxis.x != 0) {
				var q = Quaternion.AngleAxis(-state.rightStickAxis.x * 2f, target.transform.up);
				q = q * cameraRoot.transform.rotation;
				cameraRoot.transform.rotation = q;
			}
			// Y方向に回す
			if (state.rightStickAxis.y != 0) {
				var q = Quaternion.AngleAxis(state.rightStickAxis.y * 2f, cameraRoot.transform.right);
				q = q * cameraRoot.transform.rotation;
				cameraRoot.transform.rotation = q;
			}
		} else {
			// カメラ親の向きを　機体と同じ向きにする
			cameraRoot.transform.rotation = transform.rotation;
			// カメラのFOVを変更
			GetComponentInChildren<Camera>().fieldOfView = defaultFOV * zoomRatio;

			// カメラX軸角度
			float CamAngle = Mathf.Lerp(defultCamAngle, runningCamAngle, (vel / base_velocity));
			cameraTrans.rotation = cameraRoot.transform.rotation * Quaternion.AngleAxis(CamAngle, Vector3.right);
		}

		// カメラ親の向きを、機体とカメラ親の向きを補完した向き　にする
		transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, sense);
	}

	// 視点変更
	public void ChangeView() {
		//　視点番号を切り替え
		if (viewType < cameraParameter.Length-1) {
			viewType++;
		} else {
			viewType = 0;
		}

		// 主観視点か判定(最後の要素だけが主観視点)
		vm.VehicleShow(viewType != cameraParameter.Length-1);

		// 視点切り替え
		StartCoroutine("FadeCameraPos");
	}

	// 視点切り替え
	IEnumerator FadeCameraPos() {
		// カメラの位置を変更
		for (float f = 0.0f; f < 1.0f; f += changePosSpeed) {

			defultCamAngle = cameraParameter[viewType].eulerAngles.x;

			// 現在の位置と目的の位置の間で補間
			cameraTrans.transform.localPosition = Vector3.Lerp(cameraTrans.transform.localPosition, cameraParameter[viewType].localPosition, f);
			// 現在の向きと目的の向きの間で補間
			cameraTrans.transform.localRotation = Quaternion.Slerp(cameraTrans.transform.localRotation, cameraParameter[viewType].localRotation, f);
			yield return null;
		}
	}
}
