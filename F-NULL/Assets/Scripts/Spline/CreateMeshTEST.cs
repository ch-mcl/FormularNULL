using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CreateMeshTEST : MonoBehaviour {
	
	[SerializeField] ExtrudeShape m_shape;
	[SerializeField] OrientedPoint[] m_path;

	[SerializeField] Spline spline;

	/*
	void Update(){
		createMesh ();
	}
	*/

	public void createMesh() {
		Mesh m_mesh = new Mesh();

		m_shape = new ExtrudeShape(48f, 48, 0, 0);

		// 生成可能か判断
		if (spline.allPoints != null) {
			m_path = spline.allPoints;

			m_mesh.Clear();

			if (m_shape != null) {
				Extrude (m_mesh, m_shape, m_path);
			}

			MeshFilter mf = GetComponent<MeshFilter>();
			mf.mesh = m_mesh;

		}
	}


	public void Extrude(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path) {
		int vertsInShape = shape.m_verts.Length;
		int segments = path.Length; // edgeLoopの間の面の数
		int edgeLoops = path.Length + 1; // edgeLoop(基本図形)の数
		int vertCount = vertsInShape * edgeLoops; // 頂点数の算出
		int triCount = shape.m_lines.Length * segments; // 三角形の数
		int triIndexCount = triCount * 3;

		int[] triangleIndices = new int[triIndexCount];
		Vector3[] vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector2[] uvs = new Vector2[vertCount];

		// 頂点の算出
		int uc = 0;
		for(int i = 0; i < path.Length+1; i++) {
			int offset = i * vertsInShape;
			for(int j = 0; j < vertsInShape; j++) {
				int id = offset + j;
				// Oriented pointを元に、頂点の位置を追加
				vertices[id] = path[i% path.Length].LocalToWorld(shape.m_verts[j]);

				// Oriented pointを元に、法線の向きを追加
				//normals[id] = path[i].LocalToWorldDirction(shape.normals[j]);

				// U座標を元にUVを追加、Vはpathの長さに準ずる
				if (uc > shape.m_uCoords.Length-1) uc = 0;
				uvs[id] = new Vector2(shape.m_uCoords[uc], i / ((float)edgeLoops) * 98.2f);
				uc++;
			}
		}
		int ti = 0;
		for(int i = 0; i < segments; i++) {
			int offset = i * vertsInShape;
			for(int l = 0; l < shape.m_lines.Length; l += 2) {
				// lineのindicesに基づいて2つの三角形を追加
				int a = offset + (shape.m_lines[l] + vertsInShape);
				int b = offset + (shape.m_lines[l]);
				int c = offset + (shape.m_lines[l+1]);
				int d = offset + (shape.m_lines[l + 1] + vertsInShape);

				triangleIndices[ti] = a; ti++;
				triangleIndices[ti] = b; ti++;
				triangleIndices[ti] = c; ti++;
				triangleIndices[ti] = c; ti++;
				triangleIndices[ti] = d; ti++;
				triangleIndices[ti] = a; ti++;
			}
		}



		// 以下mesh生成用コード

		//mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangleIndices;
		//mesh.normals = normals;
		mesh.RecalculateNormals();
		mesh.uv = uvs;
	}
}
