using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreate : MonoBehaviour {
	private OrientedPoint[] m_path;

	[SerializeField]
	private Spline spline;

	[SerializeField]
	private MeshCollider roadColl; // 路面用コリジョン

	[SerializeField]
	private MeshCollider wallColl; // 壁用コリジョン

	/// <summary>
	/// 生成するmeshタイプ
	/// </summary>
	enum MeshType {
		CollRoad,
		CollWall,
		Model
	}

	/// <summary>
	/// mesh生成
	/// </summary>
	public void CreateMesh() {

		// 生成可能か判断
		if (spline.CurvePoints != null) {
			// 各種meshの初期化
			Mesh m_Roadmesh = new Mesh(); // 路面用
			Mesh m_Wallmesh = new Mesh(); // 壁
			Mesh m_Modelmesh = new Mesh(); // 表示用

			m_path = spline.CurvePoints;

			m_Modelmesh.Clear();



			// 路面
			Extrude((int)MeshType.CollRoad, m_Roadmesh, m_path);
			// collisionの更新
			roadColl.sharedMesh = m_Roadmesh;

			// 壁
			Extrude((int)MeshType.CollWall, m_Wallmesh, m_path);
			// collisionの更新
			wallColl.sharedMesh = m_Wallmesh;

			Extrude((int)MeshType.Model, m_Modelmesh, m_path);
			MeshFilter mf = GetComponent<MeshFilter>();
			mf.mesh = m_Modelmesh;
		}
	}

	/// <summary>
	/// 路面形状を伸ばしてmeshを生成
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="shape"></param>
	/// <param name="path"></param>
	public void Extrude(int meshType, Mesh mesh, OrientedPoint[] path) {
		// 必要頂点数の算出
		/*
		ControlPoint currentCP;
		ControlPoint nextCP;

		for (int i = 0; i < spline.CurvePoints.Length; i++) {
			// 現在のCP番号を記憶
			currentCP = spline.ControlPoints[i];

			nextCP = spline.ControlPoints[spline.ClampCPPos(i + 1)];

			if (currentCP.WidthR != nextCP.WidthR) {
				Mathf.Lerp(currentCP.WidthR, nextCP.WidthR, 0.1f * i);
			}

			if (currentCP.WidthL != nextCP.WidthL) {

			}
		}
		*/
		ControlPoint cp = spline.ControlPoints[0];

		ExtrudeShape shape = new ExtrudeShape(meshType, cp.WidthR, cp.WidthL, cp.RoadType, cp.GimicType);
		// 現在CPの路面形状を生成


		/*

		shape = new ExtrudeShape(
			,
			currentCP.WidthR,
			currentCP.WidthL,
			currentCP.RoadType,
			currentCP.GimicType
		);
		*/

		int vertsInShape = shape.Verts.Length;

		//int segments = spline.Resolution; // edgeLoopの間の面の数
		//int edgeLoops = spline.Resolution+1; // edgeLoop(基本図形)の数
		int segments = path.Length; // edgeLoopの間の面の数
		int edgeLoops = path.Length + 1; // edgeLoop(基本図形)の数

		int vertCount = vertsInShape * edgeLoops; // 頂点数の算出
		int triCount = shape.Lines.Length * segments; // 三角形の数
		int triIndexCount = triCount * 3;

		int[] triangleIndices = new int[triIndexCount];
		Vector3[] vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector2[] uvs = new Vector2[vertCount];

		// 頂点の算出
		int uc = 0;
		float vc = 0f;

		for (int i = 0; i < edgeLoops; i++) {
			int offset = i * vertsInShape;
			for(int j = 0; j < vertsInShape; j++) {
				int id = offset + j;
				// Oriented pointを元に、頂点の位置を追加
				vertices[id] = path[i%path.Length].LocalToWorld(shape.Verts[j]);

				// Oriented pointを元に、法線の向きを追加
				//normals[id] = path[i].LocalToWorldDirction(shape.normals[j]);

				// U座標を元にUVを追加、Vはpathの長さに準ずる
				if (uc > shape.UCoords.Length-1) uc = 0;
				uvs[id] = new Vector2(
					shape.UCoords[uc],
					//vc / m_path.Length
					(i / ((float)edgeLoops)) * 98.6f
					);
				uc++;
			}
		}
		int ti = 0;
		for(int i = 0; i < segments; i++) {
			int offset = i * vertsInShape;
			for(int l = 0; l < shape.Lines.Length; l += 2) {
				// lineのindicesに基づいて2つの三角形を追加
				int a = offset + (shape.Lines[l] + vertsInShape);
				int b = offset + (shape.Lines[l]);
				int c = offset + (shape.Lines[l+1]);
				int d = offset + (shape.Lines[l + 1] + vertsInShape);

				triangleIndices[ti] = a; ti++;
				triangleIndices[ti] = b; ti++;
				triangleIndices[ti] = c; ti++;
				triangleIndices[ti] = c; ti++;
				triangleIndices[ti] = d; ti++;
				triangleIndices[ti] = a; ti++;
			}
		}

		// mesh生成用
		//mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangleIndices;
		//mesh.normals = normals;
		mesh.RecalculateNormals();
		mesh.uv = uvs;
	}

}
