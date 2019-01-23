using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 生成する形の定義
// ControlPointから生成される
public class ExtrudeShape {
	#region 定数
	/// <summary>
	/// ハーフパイプ、パイプ、シリンダーの頂点数
	/// </summary>
	private int m_polyNum = 24;

	/// <summary>
	/// 路肩
	/// </summary>
	private float m_edge = 0.8f;

	/// <summary>
	/// 路肩幅
	/// </summary>
	private float m_shoulder = 0.8f;

	/// <summary>
	/// 
	/// </summary>
	private float m_edgeBottom = -0.2f;

	/// <summary>
	/// 路面　底辺
	/// </summary>
	private float m_roadBottom = -2f;

	/// <summary>
	/// 壁幅
	/// </summary>
	private float m_wallWidth = 4f;

	/// <summary>
	/// 壁　高さ(低い場合)
	/// </summary>
	private float m_wallLow = 0.8f;

	/// <summary>
	/// 壁　高さ(高い場合)
	/// </summary>
	private float m_wallHigh = 4f;
	#endregion

	#region フィールド
	/// <summary>
	/// 頂点位置
	/// </summary>
	private Vector2[] m_Vertices;
	
	/// <summary>
	/// 法線の向き
	/// </summary>
	private Vector2[] m_Normals;
	
	/// <summary>
	/// 辺
	/// </summary>
	private int[] m_Lines;
	
	/// <summary>
	/// U座標
	/// (V座標はモデル作成時に設定)
	/// </summary>
	private float[] m_UCoords;
	#endregion

	#region プロパティ
	/// <summary>
	/// 頂点位置を取得
	/// </summary>
	public Vector2[] Vertices {
		get {
			return m_Vertices;
		}
	}

	/// <summary>
	/// 法線を取得
	/// </summary>
	public Vector2[] Normals {
		get {
			return m_Normals;
		}
	}

	/// <summary>
	/// 辺を取得
	/// </summary>
	public int[] Lines {
		get {
			return m_Lines;
		}
	}

	/// <summary>
	/// U座標を取得
	/// </summary>
	public float[] UCoords {
		get {
			return m_UCoords;
		}
	}
	#endregion

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="widthR"></param>
	/// <param name="widthL"></param>
	/// <param name="road"></param>
	/// <param name="gimic"></param>
	public ExtrudeShape(int meshType, float widthR, float widthL, int road, int gimic) {

		// 道路の中心
		float center = widthR - widthL;
		if(Mathf.Abs(center) > 0) {
			// 幅が右/左のどちらかに寄っている場合
			// 右幅が大きい: 正の値 
			// 左幅が大きい: 負の値
			center = center / 2f;
		}

		// 半径
		float radius = widthR + widthL;
		radius = radius / 2;

		float edgeR = widthR - m_edge;
		float edgeL = widthL - m_edge;

		float shoulderR = edgeR - m_shoulder;
		float shoulderL = edgeL - m_shoulder;

		// 壁
		float wallR = widthR + m_wallWidth;
		float wallL = widthL + m_wallWidth;

		switch (meshType) {
			case 0:
				#region 路面用
				switch (road) {
					case 0:
					case 1:
					case 2:
					case 3:
						#region 基本
						// 0~3
						m_Vertices = new Vector2[] {

							// 路面
							new Vector2(widthR, 0),	// 右
							new Vector2(center, 0),		// 中央
							new Vector2(-widthL, 0)	// 左
						};
						m_UCoords = new float[] {
							// 路面
							1f, 0.5f, 0f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							1, 2
						};
						#endregion
						break;
					case 4:
						#region ハーフパイプ
						CreateCircle(radius, false, false);
						#endregion
						break;
					case 5:
						#region パイプ
						CreateCircle(radius, true, false);
						#endregion
						break;
					case 6:
						#region シリンダー
						CreateCircle(radius, true, true);
						#endregion
						break;
					case 7:
						#region 空白

						#endregion
						break;
					default:
						#region 異常
						Debug.LogError("Invalued RoadType");
						#endregion
						break;
				}
				#endregion
				break;
			case 1:
				#region 壁
				switch (road) {
					case 0:
					case 1:
					case 3:
						#region 基本

						m_Vertices = new Vector2[] {

							// 路面
							new Vector2(widthR, m_wallHigh),		
							new Vector2(widthR, 0),	// 右
							new Vector2(-widthL, 0),	// 左
							new Vector2(-widthL, m_wallHigh)
						};
						m_UCoords = new float[] {
							// 路面
							0f, 1f, 0f, 1f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							2, 3
						};
						#endregion
						break;
					case 2:
					case 4:
					case 5:
					case 6:
					case 7:
						#region 空白/壁無し

						#endregion
						break;
					default:
						#region 異常
						Debug.LogError("Invalued RoadType");
						#endregion
						break;
				}
				#endregion
				break;
			case 2:
				#region 表示用
				switch (road) {
					case 0:
						#region 基本
						m_Vertices = new Vector2[] {
							// 路面
							new Vector2(shoulderR, m_edgeBottom),	// 右
							new Vector2(center, 0),	// 中央
							new Vector2(center, 0),	// 中央
							new Vector2(-shoulderL, m_edgeBottom),// 左

							// 左 路肩
							new Vector2(-shoulderL, m_edgeBottom),// 左
							new Vector2(-edgeL, 0),// 左 道境界

							// 左 縁
							new Vector2(-edgeL, 0),// 左 道境界
							new Vector2(-(widthL + m_wallWidth), 0),	// 左端

							// 左 横
							new Vector2(-(widthL + m_wallWidth), 0),   // 左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下

							// 裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	// 右端下

							// 右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom), // 右端下
							new Vector2(widthR + m_wallWidth, 0),	// 右端

							// 右 縁
							new Vector2(widthR + m_wallWidth, 0),	// 右端
							new Vector2(edgeR, 0),	// 右 道境界
					
							// 右 路肩
							new Vector2(edgeR, 0),	// 右 道境界
							new Vector2(shoulderR, m_edgeBottom),	// 右

							// 右壁
							new Vector2(wallR, m_wallLow),
							new Vector2(widthR, 0),

							// 左壁
							new Vector2(-widthL, 0),
							new Vector2(-wallL, m_wallLow)
						};
						m_UCoords = new float[] {
							// 路面
							0.5f, 0f, 
							0f, 0.5f,
							// 左 路肩
							0.75f, 0.625f,
							// 左 縁
							0.75f, 0.625f,
							// 左 横
							0.75f, 0.625f,
							// 裏
							1f, 0.75f, 
							// 右 横
							0.75f, 0.625f,
							// 右 縁
							0.75f, 0.625f,
							// 右 路肩
							0.75f, 0.625f,
							// 右壁
							0.625f, 0.5f,
							// 左壁
							0.625f, 0.5f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							2, 3,
							// 左 路肩
							4, 5,
							// 左 縁
							6, 7,
							// 左 横
							8, 9,
							// 裏
							10, 11,
							// 右 横
							12, 13,
							// 右 縁
							14, 15,
							// 右 路肩
							16, 17,
							// 右壁
							18, 19,
							// 左壁
							20, 21
						};
						#endregion
						break;
					case 1:
						#region 高い壁
						m_Vertices = new Vector2[] {
							// 路面
							new Vector2(shoulderR, m_edgeBottom),	// 右
							new Vector2(center, 0),	// 中央
							new Vector2(center, 0),	// 中央
							new Vector2(-shoulderL, m_edgeBottom),// 左

							// 左 路肩
							new Vector2(-shoulderL, m_edgeBottom),// 左
							new Vector2(-edgeL, 0),// 左 道境界

							// 左 縁
							new Vector2(-edgeL, 0),// 左 道境界
							new Vector2(-(widthL + m_wallWidth), 0),	// 左端

							// 左 横
							new Vector2(-(widthL + m_wallWidth), 0),   // 左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下

							// 裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	// 右端下

							// 右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom), // 右端下
							new Vector2(widthR + m_wallWidth, 0),	// 右端

							// 右 縁
							new Vector2(widthR + m_wallWidth, 0),	// 右端
							new Vector2(edgeR, 0),	// 右 道境界
					
							// 右 路肩
							new Vector2(edgeR, 0),	// 右 道境界
							new Vector2(shoulderR, m_edgeBottom),	// 右

							// 右壁
							new Vector2(wallR, m_wallHigh*2),
							new Vector2(widthR, 0),

							// 左壁
							new Vector2(-widthL, 0),
							new Vector2(-wallL, m_wallHigh*2)
						};
						m_UCoords = new float[] {
							// 路面
							0.5f, 0f,
							0f, 0.5f,
							// 左 路肩
							0.75f, 0.625f,
							// 左 縁
							0.75f, 0.625f,
							// 左 横
							0.75f, 0.625f,
							// 裏
							1f, 0.75f, 
							// 右 横
							0.75f, 0.625f,
							// 右 縁
							0.75f, 0.625f,
							// 右 路肩
							0.75f, 0.625f,
							// 右壁
							0.625f, 0.5f,
							// 左壁
							0.625f, 0.5f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							2, 3,
							// 左 路肩
							4, 5,
							// 左 縁
							6, 7,
							// 左 横
							8, 9,
							// 裏
							10, 11,
							// 右 横
							12, 13,
							// 右 縁
							14, 15,
							// 右 路肩
							16, 17,
							// 右壁
							18, 19,
							// 左壁
							20, 21
						};
						#endregion
						break;
					case 2:
						#region 壁無し
						m_Vertices = new Vector2[] {
							// 路面
							new Vector2(shoulderR, m_edgeBottom),	// 右
							new Vector2(center, 0),	// 中央
							new Vector2(center, 0),	// 中央
							new Vector2(-shoulderL, m_edgeBottom),// 左

							// 左 路肩
							new Vector2(-shoulderL, m_edgeBottom),// 左
							new Vector2(-edgeL, 0),// 左 道境界

							// 左 縁
							new Vector2(-edgeL, 0),// 左 道境界
							new Vector2(-(widthL + m_wallWidth), 0),	// 左端

							// 左 横
							new Vector2(-(widthL + m_wallWidth), 0),   // 左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下

							// 裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	// 右端下

							// 右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom), // 右端下
							new Vector2(widthR + m_wallWidth, 0),	// 右端

							// 右 縁
							new Vector2(widthR + m_wallWidth, 0),	// 右端
							new Vector2(edgeR, 0),	// 右 道境界
					
							// 右 路肩
							new Vector2(edgeR, 0),	// 右 道境界
							new Vector2(shoulderR, m_edgeBottom),	// 右
						};
						m_UCoords = new float[] {
							// 路面
							0.5f, 0f,
							0f, 0.5f,
							// 左 路肩
							0.75f, 0.625f,
							// 左 縁
							0.75f, 0.625f,
							// 左 横
							0.75f, 0.625f,
							// 裏
							1f, 0.75f, 
							// 右 横
							0.75f, 0.625f,
							// 右 縁
							0.75f, 0.625f,
							// 右 路肩
							0.75f, 0.625f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							2, 3,
							// 左 路肩
							4, 5,
							// 左 縁
							6, 7,
							// 左 横
							8, 9,
							// 裏
							10, 11,
							// 右 横
							12, 13,
							// 右 縁
							14, 15,
							// 右 路肩
							16, 17
						};
						#endregion
						break;
					case 3:
						// TODO:天井部 定義追加
						#region トンネル
						m_Vertices = new Vector2[] {
							// 路面
							new Vector2(shoulderR, m_edgeBottom),	// 右
							new Vector2(center, 0),	// 中央
							new Vector2(center, 0),	// 中央
							new Vector2(-shoulderL, m_edgeBottom),// 左

							// 左 路肩
							new Vector2(-shoulderL, m_edgeBottom),// 左
							new Vector2(-edgeL, 0),// 左 道境界

							// 左 縁
							new Vector2(-edgeL, 0),// 左 道境界
							new Vector2(-(widthL + m_wallWidth), 0),	// 左端

							// 左 横
							new Vector2(-(widthL + m_wallWidth), 0),   // 左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下

							// 裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),// 左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	// 右端下

							// 右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom), // 右端下
							new Vector2(widthR + m_wallWidth, 0),	// 右端

							// 右 縁
							new Vector2(widthR + m_wallWidth, 0),	// 右端
							new Vector2(edgeR, 0),	// 右 道境界
					
							// 右 路肩
							new Vector2(edgeR, 0),	// 右 道境界
							new Vector2(shoulderR, m_edgeBottom),	// 右

							// 右壁
							new Vector2(wallR*0.8f, m_wallHigh*4),
							new Vector2(widthR, 0),

							// 左壁
							new Vector2(-widthL, 0),
							new Vector2(-wallL*0.8f, m_wallHigh*4),

							new Vector2(-wallL*0.8f, m_wallHigh*4),
							new Vector2(center, m_wallHigh*6f),

							new Vector2(center, m_wallHigh*6f),
							new Vector2(wallR*0.8f, m_wallHigh*4)
						};
						m_UCoords = new float[] {
							// 路面
							0.5f, 0f,
							0f, 0.5f,
							// 左 路肩
							0.75f, 0.625f,
							// 左 縁
							0.75f, 0.625f,
							// 左 横
							0.75f, 0.625f,
							// 裏
							1f, 0.75f, 
							// 右 横
							0.75f, 0.625f,
							// 右 縁
							0.75f, 0.625f,
							// 右 路肩
							0.75f, 0.625f,
							// 右壁
							0.625f, 0.5f,
							// 左壁
							0.625f, 0.5f,

							0,0,
							0,0
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							2, 3,
							// 左 路肩
							4, 5,
							// 左 縁
							6, 7,
							// 左 横
							8, 9,
							// 裏
							10, 11,
							// 右 横
							12, 13,
							// 右 縁
							14, 15,
							// 右 路肩
							16, 17,
							// 右壁
							18, 19,
							// 左壁
							20, 21,

							22, 23,
							24, 25
						};

						//TunnelCelling(m_Vertices, m_UCoords ,m_Lines);

						#endregion
						break;
					case 4:
						#region ハーフパイプ
						CreateCircle(radius, false, false);
						#endregion
						break;
					case 5:
						#region パイプ
						CreateCircle(radius, true, false);
						#endregion
						break;
					case 6:
						#region シリンダー
						CreateCircle(radius, true, true);
						#endregion
						break;
					case 7:
						#region 空白

						#endregion
						break;
					default:
						#region 異常
						Debug.LogError("Invalued RoadType");
						#endregion
						break;
				}
				#endregion
				break;
			default:
				Debug.LogError("meshType:Unexpected");
				break;
		}
	}

	private void TunnelCelling(Vector2[] verts, float[] uvs, int[] lines) {
		float theta = 90 * Mathf.Deg2Rad;

		int loop = m_polyNum/2;
		float reslution = Mathf.PI / loop;

		/*
		for (int i = 0; i < loop; i++) {
			float mx = Mathf.Sin(i * reslution);
			float my = Mathf.Cos(i * reslution);

			x = mx * width + Ox;
			y = my * b + Oy;
		}*/
	}

	/// <summary>
	/// 円型道路の頂点を計算
	/// </summary>
	/// <param name="width"></param>
	/// <param name="full"></param>
	/// <param name="outside"></param>
	private void CreateCircle(float width, bool full, bool outside) {
		float theta; // 回転角度

		width = width - 2f;

		// 円の種類
		if (full == true) {
			// 全円
			theta = (2 * Mathf.PI) / m_polyNum;
		} else {
			// 半円
			m_polyNum = m_polyNum / 2;
			theta = Mathf.PI / m_polyNum;
			m_polyNum = m_polyNum + 1;
		}

		// 各種初期化
		m_Vertices = new Vector2[m_polyNum];
		m_UCoords = new float[m_polyNum];

		// 辺を構成する頂点群　関連
		int edges;
		edges = m_polyNum * 2;
		/*
		// 半円の場合
		if (full == false) {
			edges = edges - 1;
		}
		*/
		m_Lines = new int[edges];

		// 外向きにする
		if (outside == false) {
			theta *= -1;
		}

		int vt = 0; // 辺を構成する頂点配列中の添え字

		for (int i = 0; i < m_polyNum; i++) {
			// Cos(theta), Sin(theta)から位置を求める
			// それにwidth(半径)をかける
			float m_x = Mathf.Cos(theta * i) * width;
			float m_y = Mathf.Sin(theta * i) * width;

			m_Vertices[i] = new Vector2(m_x, m_y);

			m_UCoords[i] = 1 / m_polyNum;

			vt = i * 2;
			m_Lines[vt] = i;

			if (i == (m_polyNum - 1)) {
				// 全円の場合
				if (full == true) {
					m_Lines[vt + 1] = 0;
				} else {
					m_Lines[vt + 1] = i;
				}
			} else {
				m_Lines[vt + 1] = i + 1;
			}
		}
	}
}
