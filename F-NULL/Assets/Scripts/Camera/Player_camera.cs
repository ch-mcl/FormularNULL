using UnityEngine;
using System.Collections;

public class Player_camera : MonoBehaviour {

	[SerializeField] GameObject cameraRoot;
	[SerializeField] Transform cameraTrans;
	[SerializeField] Transform[] cameraParameter;

	[SerializeField] Vehicle target; // 追尾対象
	public float sense = 0.1f; // 補間値
	public float defaultFOV = 80f; // デフォルトの視野 速度が上がると上昇
	public float positionType = 0; // 視点の種類


	public float zoom = 0.2f; // 視野の拡大率
	float zoomRatio; 
	
	bool around = false; // カメラ操作可能 
	float base_velocity = 340f; // 規定値 およそ 音速(1224km/h)
	float minZoomVel = 2.7f;
	int viewType = 0;

	/*
	void Start () {
	}
	*/

	void Update () {

		transform.position = target.transform.position;

		// FOV拡大度を計算
		float vel = Mathf.Abs(target.GetComponent<VehicleMover>().m_vel);
		if(vel > minZoomVel) {
			double clampdVel = Mathf.Clamp(vel, 0, base_velocity*2);
			zoomRatio = (vel * zoom / base_velocity) + 1f;
		} else {
			zoomRatio = 1f;
		}

		// カメラ操作モード用
		// PlayerController から情報をもらう(予定)
		GamepadInput.GamepadState state = GamepadInput.GamePad.GetState(GamepadInput.GamePad.Index.Any);
		if (Input.GetKeyDown(KeyCode.Q)) {
			if (!around) {
				around = true;
			} else {
				around = false;
			}
		}
		// カメラ操作可能時処理
		if (around){
			if (state.rightStickAxis.x != 0) {
				var q = Quaternion.AngleAxis(-state.rightStickAxis.x * 2f, target.transform.up);
				q = q * cameraRoot.transform.rotation;
				cameraRoot.transform.rotation = q;
			}
		
			if (state.rightStickAxis.y != 0) {
				var q = Quaternion.AngleAxis(state.rightStickAxis.y * 2f, cameraRoot.transform.right);
				q = q * cameraRoot.transform.rotation;
				cameraRoot.transform.rotation = q;
			}
		} else {
			cameraRoot.transform.rotation = transform.rotation;
			GetComponentInChildren<Camera>().fieldOfView = defaultFOV * zoomRatio;
		}
		transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, sense);

		bool inside = viewType < cameraParameter.Length - 1;
	}

	// 視点変更
	public void ChangeView() {
		if (viewType < cameraParameter.Length-1) {
			viewType++;
		} else {
			viewType = 0;
		}

		target.GetComponent<VehicleMover>().VehicleShow(viewType != cameraParameter.Length - 1);
		/*
		if  {
			target.GetComponent<VehicleController>().VehicleShow(true);
		} else {
			target.GetComponent<VehicleController>().VehicleShow(false);
		}
		*/
		// 位置を変更
		cameraTrans.transform.localPosition = cameraParameter[viewType].localPosition;
		cameraTrans.transform.localRotation = cameraParameter[viewType].localRotation;
	}
}
