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
			EditorGUILayout.LabelField("loops: " + spline.Loops);
		}

		if (GUILayout.Button("1.Get Child")) {
			// splineに子オブジェクトでControlPointコンポーネントが付加されたオブジェクトを持たせる
			spline.m_controlPoints = spline.GetComponentsInChildren<ControlPoint>();


			ControlPoint[] cps = new ControlPoint[spline.m_controlPoints.Length];
			for (int i = 0; i < spline.m_controlPoints.Length; i++) {
				cps[i] = spline.m_controlPoints[i];
			}
			// 
			spline.m_controlPoints = cps;
		}

		// 名前変更ボタンの追加
		// 最初に通過させたいオブジェクトは子の一番上にすること
		if (GUILayout.Button("2.CP's name to numbers")) {
			spline.m_controlPoints[0].name = (spline.m_controlPoints.Length - 1).ToString();
			spline.m_controlPoints[0].ID = (spline.m_controlPoints.Length - 1);
			for (int i = 0; i < spline.m_controlPoints.Length - 1; i++) {
				spline.m_controlPoints[i + 1].name = i.ToString();
				spline.m_controlPoints[i + 1].ID = i;
			}
		}
	}

	public void OnSceneGUI(){
		spline = target as Spline;
		Gizmos.color = Color.white;

		// ControlPointが無効か判定
		if (spline.m_controlPoints != null) {
			spline.CurvePoints = new OrientedPoint[spline.m_controlPoints.Length * spline.Loops]; // Cutmull-RomSpline曲線上の全点を保持

			// CatmullRomSplineを始点から終点までに描画
			for (int i = 0; i < spline.m_controlPoints.Length; i++) {
				if ((i == 0 || i == spline.m_controlPoints.Length - 2 || i == spline.m_controlPoints.Length - 1) && !spline.IsLoop) {
					continue;
				}

				Quaternion handleRotation;

				// pivotModeがグローバル座標の場合
				if (Tools.pivotRotation == PivotRotation.Global) {
					handleRotation = Quaternion.AngleAxis(0, Vector3.one);
				// pivotModeがローカル座標の場合
				} else {
					handleRotation = spline.m_controlPoints[i].transform.rotation;
				}

				// 位置変更確認
				EditorGUI.BeginChangeCheck();

				// 移動マニュピレータ表示
				Vector3 handle = Handles.PositionHandle(spline.m_controlPoints[i].transform.position, handleRotation);
				Handles.Label(spline.m_controlPoints[i].transform.position, spline.m_controlPoints[i].name);
				Handles.SphereHandleCap(i, spline.m_controlPoints[i].transform.position, handleRotation, 8f, EventType.Repaint);

				// (移動検出)
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(spline, "Move CP");
					spline.m_controlPoints[i].transform.position = handle;
					EditorUtility.SetDirty(spline);
				}

			}
		} else {
			Debug.LogError("Spline Brokne");
		}
	}
}
