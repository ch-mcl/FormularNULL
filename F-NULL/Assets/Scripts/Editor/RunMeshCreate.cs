using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackCreate))] // TrackCreateのInspectorを拡張
[CanEditMultipleObjects] // 複数のオブジェクト変更が可能
public class RunMeshCreate : Editor {

	static string meshName; // meshの名前になる

	static bool realUpdate;

	TrackCreate creater;

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		realUpdate = EditorGUILayout.Toggle("Real Update", realUpdate);

		meshName = EditorGUILayout.TextField("Mesh Name", meshName); // meshの名前を変更

		if (GUILayout.Button("createMesh")) {
			creater.createMesh();
		}

		if (GUILayout.Button("SaveMesh")) {
			MeshFilter mf = creater.GetComponent<MeshFilter>();
			string path = "Assets/Procedual/" + meshName + ".asset";
			AssetDatabase.CreateAsset(mf.sharedMesh, path);
			AssetDatabase.SaveAssets();
		}
	}

	// 強制変更
	void OnSceneGUI(){
		creater = target as TrackCreate;
		if (realUpdate) {
			creater.createMesh ();
		}
	}
}
