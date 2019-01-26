using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCreate : MonoBehaviour {
	[SerializeField]
	private bool setLoopTimes;

	[SerializeField]
	private float roadTexureDiv = 20f;

	[SerializeField]
	[Range(1, 1000)]
	int handLoop = 2;

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
		int segments = path.Count;
		segments = segments + 1;

		// edgeLoop(基本図形)の数
		int edgeLoops = path.Count; 
		edgeLoops = edgeLoops + 1;

		if(setLoopTimes == true) {
			//生成するCP数を指定する設定が有効の場合
			segments = handLoop;
			edgeLoops = handLoop;
		}

		//生成するmeshのList
		List<Vector3> vertices = new List<Vector3>(); //頂点用
		List<Vector2> uvs = new List<Vector2>(); //UV用
		List<Vector3> normals = new List<Vector3>(); //Noramal/法線用
		List<int> triangleIndices = new List<int>(); //三角形配列用

		#region 頂点の追加
		int CurrentCP = 0; //現在のCP番号
		ControlPoint next = spline.ControlPoints[spline.ClampCPPos(CurrentCP + 1)]; //次のCP

		float widthR = 0; //路面の幅(右)
		float widthL = 0; //路面の幅(左)
		float t = 0f; //スプライン曲線上における位置(0.0~1.0を指定)
		float resolution = 1f / spline.Loops; //粒度

		float vCoord = 0f; //V座標
		float distance = 0f; //コースの長さ(V座標算出に使用)

		//路面に関する値
		int vertsInShape = shape.Vertices.Length;	//頂点数
		int lines = shape.Lines.Length; ; //辺の数

		//路面(1つ前)に関する値
		int preVertsInShape = shape.Lines.Length;//頂点数(1つ前の路面)
		int preLines = shape.Vertices.Length; //辺の数(1つ前の路面)

		int offset = shape.Vertices.Length; //三角形配列で使用する値

		Vector3 vert = Vector3.zero; //路面の頂点

		for (int i = 0; i < edgeLoops; i++) {
			#region 路面形状の更新
			//TODO:路面形状が変わる場合への対応
			if ((i-1) % (spline.Loops) == 0) {
				t = 0;

				cp = spline.ControlPoints[spline.ClampCPPos(CurrentCP)];
				next = spline.ControlPoints[spline.ClampCPPos(CurrentCP+1)];

				//現在のCPにおける幅をキャッシュする
				widthR = cp.WidthR;
				widthL = cp.WidthL;

				//路面形状を更新する
				shape = new ExtrudeShape(meshType, cp.WidthR, cp.WidthL, cp.RoadType, cp.GimicType);

				CurrentCP++;
			} else {
				t += resolution;
			}

			//路面幅の変更
			if(cp.WidthL!=next.WidthL || cp.WidthR != next.WidthR) {
				//右幅が変化する場合
				if(cp.WidthR != next.WidthR) {
					//右幅を更新する
					widthR = cp.WidthR + t * (next.WidthR - cp.WidthR);
				}
				//左幅が変化する場合
				if (cp.WidthL != next.WidthL) {
					//幅を更新する
					widthL = cp.WidthL + t * (next.WidthL - cp.WidthL);
				}

				//路面形状を更新する
				shape = new ExtrudeShape(meshType, widthR, widthL, cp.RoadType, cp.GimicType);
			}
			#endregion

			#region 路面頂点の空チェック
			//空チェック
			if (shape.Vertices == null) {
				//空の場合
				vertsInShape = 0;
			} else {
				vertsInShape = shape.Vertices.Length;
			}

			for (int j = 0; j < vertsInShape; j++) {
				// 頂点をグローバル座標化する
				vert = path[i % path.Count].LocalToWorld(shape.Vertices[j]);
				// 頂点を追加する
				vertices.Add(vert);

				// 法線のグローバル座標化
				//Vector3 normal = path[i].LocalToWorldDirction(shape.Normals[j]);
				// 法線を追加する
				//normals.Add(normal);

				#region UV追加
					if (j == 0 && CurrentCP > 0) {
						//1つ目の頂点の場合 かつ 現在のCPが0より大きい場合
						//1つ目の頂点の場合(V座標の値はその路面形状に対して一定になるようにするため)
						//現在のCPが0より大きい
						//CurrentCPが0の場合0-1で-1になるのを防ぐ
						distance += spline.SectionDistances[CurrentCP - 1];
					}
					//V座標を算出する
					vCoord = distance / roadTexureDiv;

					// uv座標を追加する
					Vector2 uv = new Vector2(shape.UCoords[j], vCoord);
					uvs.Add(uv);
				#endregion
			}
			#endregion

			#region 三角形配列用
			if (i > 0) {
				//iが0以上の場合
				//路面(1つ前)と接続しようとして負の値が出るため三角形配列は生成しない

				int verts = 0;  //路面(1つ前)と接続するために引く値
				//路面(1つ前)と頂点数が異なるか
				//路面(1つ前)の
				if (vertsInShape != preVertsInShape) {
					//路面(1つ前)の頂点数と異なる場合
					//路面(1つ前)の頂点数分を引く
					verts = preVertsInShape;
				} else {
					verts = vertsInShape;
				}

				//空チェック
				if (shape.Lines == null) {
					//1つ前が空の場合
					//三角形配列の生成をしない
					lines = 0;
				} else { 
					lines = shape.Lines.Length;
				}

				if (lines > preLines) {
					//路面(1つ前)より辺が多い場合
					//路面(1つ前)の辺の数に合わせる
					lines = preLines;
				}

				//三角形配列生成
				//2つの三角形が出来るよう辺を結ぶ
				//路面(現在)から路面(1つ前)に向かって面を構築する
				for (int l = 0; l < lines; l += 2) {
					int a = offset + (shape.Lines[l]);
					int b = offset + (shape.Lines[l] - verts);
					int c = offset + (shape.Lines[l + 1] - verts);
					int d = offset + (shape.Lines[l + 1]);

					triangleIndices.Add(a);
					triangleIndices.Add(b);
					triangleIndices.Add(c);
					triangleIndices.Add(c);
					triangleIndices.Add(d);
					triangleIndices.Add(a);
				}
				offset += vertsInShape;
			}
			#endregion

			#region 1つ前の頂点と辺の数を記憶する
			//路面(1つ前)の頂点数をキャッシュする
			if (shape.Vertices == null) {
				//空の場合
				preVertsInShape = 0;
			} else {
				preVertsInShape = vertsInShape;
			}

			//路面(1つ前)の辺数をキャッシュする
			if (shape.Lines == null) {
				//空の場合
				preLines = 0;
			} else {
				preLines = shape.Lines.Length;
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