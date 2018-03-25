using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Vehicle))]
public class VehicleInspector : Editor {

	Vehicle vehicle;

	void OnEnable() {
		// Vehicle コンポーネントの取得
		vehicle = (Vehicle)target;
	}

	// 3属性のステータス、最高速度の表示
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Status");
		EditorGUILayout.LabelField("Max Speed: " + vehicle.maximumSpeed * 3.6f + " km/h");
		EditorGUILayout.LabelField("Body: "+rank(vehicle.body));
		EditorGUILayout.LabelField("Boost: " + rank(vehicle.boost));
		EditorGUILayout.LabelField("Grip: " + rank(vehicle.grip));
	}

	// 評価
	string rank(float value) {
		if (value >= 0.8f) {
			return "A";
		} else {
			if (value >= 0.6f) {
				return "B";
			} else {
				if (value >= 0.4f) {
					return "C";
				}
				else {
					if (value >= 0.2f) {
						return "D";
					}
					else {
						return "E";
					}
				}
			}
		};
	}
}
