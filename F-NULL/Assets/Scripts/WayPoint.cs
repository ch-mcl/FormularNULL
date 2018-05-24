using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WayPointオブジェクト (実質チェックポイント)

public class WayPoint : MonoBehaviour {

	public Color m_RayColor = Color.white; // Editorで描画する色
	public List<Transform> m_PathObjects = new List<Transform>(); // WayPoint更新時に使用するリスト
	Transform[] m_pathArray; // WayPointの配列

	// プロパティ
	//
	public Transform[] WayPoints {
		get { return m_pathArray; }
	}

	// 子オブジェクトになっているWayPointの取得
	public void GetWayPoints() {
		Gizmos.color = m_RayColor;
		m_pathArray = GetComponentsInChildren<Transform>();
		m_PathObjects.Clear();

		foreach (Transform path_obj in m_pathArray)
		{
			if (path_obj != this.transform)
			{
				m_PathObjects.Add(path_obj);
			}
		}
	}

	void OnDrawGizmos() {
		for (int i = 0; i < m_PathObjects.Count; i++) {
			Vector3 pos = m_PathObjects[i].position;
			if(i > 0) {
				Vector3 previous = m_PathObjects[i - 1].position;
				Gizmos.DrawLine(previous, pos);
				Gizmos.DrawSphere(pos, 10f);
			}
		}
	}

}
