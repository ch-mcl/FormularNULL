using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// チェックポイントの名前を 自動で数字に変える

[CustomEditor(typeof(WayPoint))]
[CanEditMultipleObjects]
public class CHKPT_NameChanger : Editor {

	WayPoint waypoint;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI(); // 元のインスペクターの描画

		if (GUILayout.Button("1.Get WayPoints")) {
			waypoint = target as WayPoint;
			waypoint.GetWayPoints();
		}

		// 名前変更ボタンの追加
		// 最初に通過させたいオブジェクトは子の一番上にすること
		if (GUILayout.Button("2.CHKPTs to numbers")) {
			waypoint = target as WayPoint;

			waypoint.m_PathObjects[0].name = (waypoint.m_PathObjects.Count-1).ToString();
			for (int i = 0; i < waypoint.m_PathObjects.Count-1; i++) {
				waypoint.m_PathObjects[i+1].name = i.ToString();
			}

		}
	}

	public void OnSceneGUI() {
		waypoint = target as WayPoint;
	}

}
