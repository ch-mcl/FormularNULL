using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour {

	public Color rayColor = Color.white;
	public List<Transform> path_objs = new List<Transform>();
	Transform[] pathArray;

	public void GetWayPoints() {
		Gizmos.color = rayColor;
		pathArray = GetComponentsInChildren<Transform>();
		path_objs.Clear();

		foreach (Transform path_obj in pathArray)
		{
			if (path_obj != this.transform)
			{
				path_objs.Add(path_obj);
			}
		}
	}

	void OnDrawGizmos() {
		for (int i = 0; i < path_objs.Count; i++) {
			Vector3 pos = path_objs[i].position;
			if(i > 0) {
				Vector3 previous = path_objs[i - 1].position;
				Gizmos.DrawLine(previous, pos);
				Gizmos.DrawSphere(pos, 10f);
			}
		}
	}

}
