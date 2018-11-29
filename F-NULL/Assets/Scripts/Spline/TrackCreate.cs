using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreate : MonoBehaviour {
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

	void Start() {
		spline.GetCP();

		spline.ChangeCPName();

		CreateMesh();
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

			List<OrientedPoint> m_path = spline.CurvePoints;

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
	public void Extrude(int meshType, Mesh mesh, List<OrientedPoint> path) {

		ControlPoint cp = spline.ControlPoints[0]; // 現在のCP

		// 現在CPの路面形状を生成
		ExtrudeShape shape = new ExtrudeShape(meshType, cp.WidthR, cp.WidthL, cp.RoadType, cp.GimicType);

		// edgeLoopの間の面の数
		int segments;
		segments = path.Count+1;

		// edgeLoop(基本図形)の数
		int edgeLoops;
		edgeLoops = path.Count+1; 

		List<int> triangleIndices = new List<int>();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();

		#region 頂点の追加
		// 頂点の算出

		int CurrentCP = 0; // 現在のCP番号
		ControlPoint next = spline.ControlPoints[spline.ClampCPPos(CurrentCP + 1)]; // 次のCP

		float widthR = 0;
		float widthL = 0;
		float t = 0f;
		float resolution = 1f / spline.Loops; // 粒度

		int uCoord = 0; // U座標
		float vCoord = 0f; // V座標
		float distance = 0f;

		int vertsInShape = 0;

		int offset = 0;

		Vector3 vert = Vector3.zero;

		for (int i = 0; i < edgeLoops; i++) {
			#region 路面形状の更新
			if ((i-1) % spline.Loops == 0) {
				t = resolution;

				cp = spline.ControlPoints[spline.ClampCPPos(CurrentCP)];
				next = spline.ControlPoints[spline.ClampCPPos(CurrentCP+1)];

				// 現在のCPにおける幅をキャッシュ
				widthR = cp.WidthR;
				widthL = cp.WidthL;

				// 路面形状の更新
				shape = new ExtrudeShape(meshType, cp.WidthR, cp.WidthL, cp.RoadType, cp.GimicType);

				CurrentCP++;
			} else {
				t += resolution;
			}

			// 路面幅の変更
			if(cp.WidthL!=next.WidthL || cp.WidthR != next.WidthR) {
				// 右幅が変化する場合
				if(cp.WidthR != next.WidthR) {
					// 右幅を更新
					widthR = cp.WidthR + t * (next.WidthR - cp.WidthR);
				}

				if (cp.WidthL != next.WidthL) {
					// 左幅を更新
					widthL = cp.WidthL + t * (next.WidthL - cp.WidthL);
				}

				// 路面形状の更新
				shape = new ExtrudeShape(meshType, widthR, widthL, cp.RoadType, cp.GimicType);
			}
			#endregion

			#region 路面頂点の追加
			if (shape.Vertices != null) {
				// 頂点群ありの場合
				vertsInShape = shape.Vertices.Length; // 路面頂点を取得

				for (int j = 0; j < vertsInShape; j++) {
					// 頂点のグローバル座標化
					vert = path[i % path.Count].LocalToWorld(shape.Vertices[j]);
					// 頂点を追加
					vertices.Add(vert);

					// 法線のグローバル座標化
					//Vector3 normal = path[i].LocalToWorldDirction(shape.Normals[j]);
					// 法線を追加
					//normals.Add(normal);

					// U座標を元にUVを追加、Vはpathの長さに準ずる
					if (uCoord > shape.UCoords.Length - 1) {
						// 最後のU座標の場合
						uCoord = 0;
						distance += spline.SectionDistances[Mathf.FloorToInt((i-1)/spline.Loops)];
					}
					vCoord = distance / 10f;

					// uv座標を追加
					Vector2 uv = new Vector2(shape.UCoords[uCoord], vCoord);
					uvs.Add(uv);

					uCoord++;
				}
			} else {
				vertsInShape = 0;
			}
			#endregion

			#region 三角形配列用
			if (i < segments) {
				// iが面数未満の場合
				if (shape.Lines != null) {

					int lines = shape.Lines.Length;

					if (i > 0) {
						for (int l = 0; l < lines; l += 2) {
							int a = offset + (shape.Lines[l]);
							int b = offset + (shape.Lines[l] - vertsInShape);
							int c = offset + (shape.Lines[l + 1] - vertsInShape);
							int d = offset + (shape.Lines[l + 1]);


							triangleIndices.Add(a);
							triangleIndices.Add(b);
							triangleIndices.Add(c);
							triangleIndices.Add(c);
							triangleIndices.Add(d);
							triangleIndices.Add(a);
						}
					}
				} else {
					vertsInShape = 0;
				}
				offset += vertsInShape;
			}
			#endregion


		}
		#endregion

		// mesh生成
		//mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangleIndices.ToArray();
		//mesh.normals = normals;
		mesh.RecalculateNormals();
		mesh.uv = uvs.ToArray();
	}
}
