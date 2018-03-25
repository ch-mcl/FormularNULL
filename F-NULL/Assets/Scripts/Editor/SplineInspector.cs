using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// CatmullRomSpline を描画

[CustomEditor(typeof(Spline))]
[CanEditMultipleObjects]
public class SplineInspector : Editor {

	Spline spline;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (spline != null) {
			EditorGUILayout.LabelField("loops: " + spline.loops);
		}

		if (GUILayout.Button("1.Get Child")) {
			// splineに子オブジェクトでControlPointコンポーネントが付加されたオブジェクトを持たせる
			spline.controlPoints = spline.GetComponentsInChildren<ControlPoint>();


			ControlPoint[] cps = new ControlPoint[spline.controlPoints.Length];
			for (int i = 0; i < spline.controlPoints.Length; i++) {
				cps[i] = spline.controlPoints[i];
			}
			// 
			spline.controlPoints = cps;
		}

		// 名前変更ボタンの追加
		// 最初に通過させたいオブジェクトは子の一番上にすること
		if (GUILayout.Button("2.CP's name to numbers")) {
			spline.controlPoints[0].name = (spline.controlPoints.Length - 1).ToString();
			spline.controlPoints[0].m_id = (spline.controlPoints.Length - 1);
			for (int i = 0; i < spline.controlPoints.Length - 1; i++) {
				spline.controlPoints[i + 1].name = i.ToString();
				spline.controlPoints[i + 1].m_id = i;
			}
		}
	}

	public void OnSceneGUI(){
		spline = target as Spline;
		Gizmos.color = Color.white;

		spline.allPoints = new OrientedPoint[spline.controlPoints.Length * spline.loops]; // Cutmull-RomSpline曲線上の全点を保持
		spline.allIndex = 0;

		// CatmullRomSplineを始点から終点までに描画
		for (int i = 0; i < spline.controlPoints.Length; i++) {
			if ((i == 0 || i == spline.controlPoints.Length - 2 || i == spline.controlPoints.Length - 1) && !spline.IsLoop) {
				continue;
			}

			//spline.DisplayCatmullRomSpline(i); // Cutmull-Rom Spline線分を描画

			Quaternion handleRotation = spline.controlPoints[i].transform.rotation;


			// 位置変更確認
			EditorGUI.BeginChangeCheck();

			// 移動マニュピレータ表示(表示のみ)
			Vector3 handle = Handles.PositionHandle(spline.controlPoints[i].transform.position, handleRotation);
			Handles.Label(spline.controlPoints[i].transform.position, spline.controlPoints[i].name);
			Handles.SphereHandleCap(i, spline.controlPoints[i].transform.position, handleRotation, 8f, EventType.Repaint);

			// (移動検出)
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move CP");
				spline.controlPoints[i].transform.position = handle;
			}

		}
	}
}
