using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplineTEST : MonoBehaviour {

	//Use the transforms of GameObjects in 3d space as your points or define array with desired points
	public Transform[] points;

	//Store points on the Catmull curve so we can visualize them
	List<Vector3> newPoints = new List<Vector3>();

	//How many points you want on the curve
	float amountOfPoints = 10.0f;

	//set from 0-1
	[Range(-1f, 1f)]
	public float alpha = 0.5f;

	/////////////////////////////

	void Update() {
		CatmulRom();
	}

	void CatmulRom() {
		newPoints.Clear();

		for (int i = 0; i < points.Length; i++) {
			Vector3 p0 = points[ClampListPos(i - 1)].position;
			Vector3 p1 = points[i].position;
			Vector3 p2 = points[ClampListPos(i + 1)].position;
			Vector3 p3 = points[ClampListPos(i + 2)].position;
			/*
			Vector3 p0 = new Vector3(points[0].position.x, points[0].position.y, points[0].position.z);
			Vector3 p1 = new Vector3(points[1].position.x, points[1].position.y, points[1].position.z);
			Vector3 p2 = new Vector3(points[2].position.x, points[2].position.y, points[2].position.z);
			Vector3 p3 = new Vector3(points[3].position.x, points[3].position.y, points[3].position.z);
			*/

			float t0 = 0.0f;
			float t1 = GetT(t0, p0, p1);
			float t2 = GetT(t1, p1, p2);
			float t3 = GetT(t2, p2, p3);

			for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints)) {
				Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
				Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
				Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

				Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
				Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

				Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

				newPoints.Add(C);
			}
		}
	}

	float GetT(float t, Vector3 p0, Vector3 p1) {
		float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f) + Mathf.Pow((p1.z - p0.z), 2.0f);
		float b = Mathf.Pow(a, 0.5f);
		float c = Mathf.Pow(b, alpha);

		return (c + t);
	}

	// 位置を制限
	// 位置をループの中だけに制限する
	public int ClampListPos(int pos) {
		if (pos < 0) {
			pos = points.Length - 1;
		}

		if (pos > points.Length) {
			pos = 1;
		} else if (pos > points.Length - 1) {
			pos = 0;
		}

		return pos;
	}

	//Visualize the points
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Vector3 pre = points[0].position;

		foreach (Vector3 temp in newPoints) {
			Vector3 pos = new Vector3(temp.x, temp.y, temp.z);
			//Gizmos.DrawSphere(pos, 26f);

			Gizmos.DrawLine(pre, pos);
			pre = pos;
		}
	}
}