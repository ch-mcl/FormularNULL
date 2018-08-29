using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// CatmullRomSpline を描画

[CustomEditor(typeof(Spline))]
[CanEditMultipleObjects]
public class SplineInspector : Editor {

	Spline spline;
	int insertIndex = 0;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (spline != null) {
			EditorGUILayout.LabelField("loops: " + spline.Loops);
		}

		if (GUILayout.Button("1.Get Child")) {
			// splineに子オブジェクトでControlPointコンポーネントが付加されたオブジェクトを持たせる
			spline.GetCP();
		}

		// 名前変更ボタンの追加
		// 最初に通過させたいオブジェクトは子の一番上にすること
		if (GUILayout.Button("2.CP's name to numbers")) {
			spline.ChangeCPName();
		}

		insertIndex = EditorGUILayout.IntField("Insert CP index", insertIndex);

		if (GUILayout.Button("ADD CP")) {
			spline.AddCP();
		}

		if (GUILayout.Button("Insert CP")) {
			spline.InsertCP(insertIndex);
		}

		if (GUILayout.Button("REMOVE CP")) {
			//TODO: CP削除(エラーなし)
		}
	}

	public void OnSceneGUI(){
		spline = target as Spline;
		Gizmos.color = Color.white;

		// ControlPointが無効か判定
		if (spline.ControlPoints != null) {
			// CatmullRomSplineを始点から終点までに描画
			for (int i = 0; i < spline.ControlPoints.Count; i++) {
				if ((i == spline.ControlPoints.Count - 2 || i == spline.ControlPoints.Count - 1) && !spline.IsLoop) {
					continue;
				}

				Quaternion handleRotation;

				// pivotModeがグローバル座標の場合
				if (Tools.pivotRotation == PivotRotation.Global) {
					handleRotation = Quaternion.AngleAxis(0, Vector3.one);
				// pivotModeがローカル座標の場合
				} else {
					handleRotation = spline.ControlPoints[i].transform.rotation;
				}

				// 位置変更確認
				EditorGUI.BeginChangeCheck();

				// 移動マニュピレータ表示
				Vector3 handle = Handles.PositionHandle(spline.ControlPoints[i].transform.position, handleRotation);
				Handles.Label(spline.ControlPoints[i].transform.position, spline.ControlPoints[i].name);
				Handles.SphereHandleCap(i, spline.ControlPoints[i].transform.position, handleRotation, 8f, EventType.Repaint);

				// (移動検出)
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(spline, "Move CP");
					spline.ControlPoints[i].transform.position = handle;
					EditorUtility.SetDirty(spline);
				}

			}
		} else {
			Debug.LogError("Spline Brokne");
		}
	}
}
