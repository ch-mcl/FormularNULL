using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 生成する形の定義
// ControlPointから生成される
public class ExtrudeShape {
	private Vector2[] m_Verts;		// 頂点位置
	private Vector2[] m_Normals;	// 法線の向き
	private int[] m_Lines;          // 辺
	private float[] m_UCoords;		// U座標 (V座標はモデル作成時に設定)

	private int polyNum = 24; // ハーフパイプ、パイプ、シリンダーの頂点数

	/// <summary>
	/// 頂点位置を取得
	/// </summary>
	public Vector2[] Verts {
		get {
			return m_Verts;
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

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="widthR"></param>
	/// <param name="widthL"></param>
	/// <param name="road"></param>
	/// <param name="gimic"></param>
	public ExtrudeShape(int meshType, float widthR, float widthL, int road, int gimic) {

		// 半径
		float radius = widthR + widthL;
		radius = radius / 2;

		float edgeR = widthR - 2;
		float edgeL = widthL - 2;

		float shoulderR = edgeR - 2;
		float shoulderL = edgeL - 2;

		float shoulderDown = -0.4f;
		float roadBottom = -2f;

		switch (meshType) {
			case 0:
				#region 路面用
				switch (road) {
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
						#region 基本
						// 0~3
						m_Verts = new Vector2[] {

							// 路面
							new Vector2(widthR, 0),	// 右
							new Vector2(0, 0),		// 中央
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
				}
				#endregion
				break;
			case 1:
				#region 壁
				switch (road) {
					case 0:
						#region 基本

						m_Verts = new Vector2[] {

							// 路面
							new Vector2(widthR, 4),		
							new Vector2(widthR, 0),	// 右
							new Vector2(-widthL, 0),	// 左
							new Vector2(-widthL, 4)
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
						#region 壁無し

						m_Verts = new Vector2[] {
							// 路面
						};
						m_UCoords = new float[] {
							// 路面
						};
						m_Lines = new int[]{
							// 路面
						};
						#endregion
						break;
					default:
						#region 生成エラー防止
						m_Verts = new Vector2[] {

							// 路面
							new Vector2(widthR, 4),
							new Vector2(widthR, 0),	// 右
							new Vector2(-widthL, 0),	// 左
							new Vector2(-widthL, 4)
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
				}
				#endregion
				break;
			case 2:
				#region 表示用
				switch (road) {
					case 0:
						#region 基本
						m_Verts = new Vector2[] {

							// 路面
							new Vector2(shoulderR, shoulderDown),	// 右
							new Vector2(0, 0),		// 中央
							new Vector2(-shoulderL, shoulderDown),// 左

							// 左 路肩
							new Vector2(-shoulderL, shoulderDown),// 左
							new Vector2(-edgeL, 0),// 左 道境界

							// 左 縁
							new Vector2(-edgeL, 0),// 左 道境界
							new Vector2(-widthL, 0),	// 左端

							// 左 横
							new Vector2(-widthL, 0),   // 左端
							new Vector2(-widthL, roadBottom),// 左端下

							// 裏
							new Vector2(-widthL, roadBottom),// 左端下
							new Vector2(widthR, roadBottom),	// 右端下

							// 右 横
							new Vector2(widthR, roadBottom), // 右端下
							new Vector2(widthR, 0),	// 右端

							// 右 縁
							new Vector2(widthR, 0),	// 右端
							new Vector2(edgeR, 0),	// 右 道境界
					
							// 右 路肩
							new Vector2(edgeR, 0),	// 右 道境界
							new Vector2(shoulderR, shoulderDown),	// 右
						};
						m_UCoords = new float[] {
							// 路面
							0f, 1.5f, 3f,
							// 左 路肩
							1f, 0f,
							// 左 縁
							1f, 0f,
							// 左 横
							1f, 0f,
							// 裏
							1f, 0f,
							// 右 横
							1f, 0f,
							// 右 縁
							1f, 0f,
							// 右 路肩
							1f, 0f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							1, 2,
							// 左 路肩
							3, 4,
							// 左 縁
							5, 6,
							// 左 横
							7, 8,
							// 裏
							9, 10,
							// 右 横
							11, 12,
							// 右 縁
							13, 14,
							// 右 路肩
							15, 16
						};
						#endregion
						break;
					case 1:
						#region 高い壁
						m_Verts = new Vector2[] {
							new Vector2(widthR, 0),			// 右
							new Vector2(0, 0),				// 中央
							new Vector2(-widthL, 0),		// 左

							new Vector2(widthR+2, 8),		// 右端
							new Vector2(-(widthL+2), 8),	// 左端

							new Vector2(widthR+2, -2),		// 右端下
							new Vector2(-(widthL+2), -2),	// 左端下
						};
						m_UCoords = new float[] {
							0f,
							1.5f,
							3f,
							1f,
							1f,
							0f,
							1f,
						};
						m_Lines = new int[]{
							0, 1,
							1, 2,
							3, 0,
							2, 4,
							5, 3,
							4, 6
						};
						#endregion
						break;
					case 2:
						#region 壁無し
						m_Verts = new Vector2[] {

							// 路面
							new Vector2(shoulderR, shoulderDown),	// 右
							new Vector2(0, 0),		// 中央
							new Vector2(-shoulderL, shoulderDown),// 左

							// 左 路肩
							new Vector2(-shoulderL, shoulderDown),// 左
							new Vector2(-edgeL, 0),// 左 道境界

							// 左 縁
							new Vector2(-edgeL, 0),// 左 道境界
							new Vector2(-widthL, 0),	// 左端

							// 左 横
							new Vector2(-widthL, 0),   // 左端
							new Vector2(-widthL, roadBottom),// 左端下

							// 裏
							new Vector2(-widthL, roadBottom),// 左端下
							new Vector2(widthR, roadBottom),	// 右端下

							// 右 横
							new Vector2(widthR, roadBottom), // 右端下
							new Vector2(widthR, 0),	// 右端

							// 右 縁
							new Vector2(widthR, 0),	// 右端
							new Vector2(edgeR, 0),	// 右 道境界
					
							// 右 路肩
							new Vector2(edgeR, 0),	// 右 道境界
							new Vector2(shoulderR, shoulderDown),	// 右
						};
						m_UCoords = new float[] {
							// 路面
							0f, 1.5f, 3f,
							// 左 路肩
							1f, 0f,
							// 左 縁
							1f, 0f,
							// 左 横
							1f, 0f,
							// 裏
							1f, 0f,
							// 右 横
							1f, 0f,
							// 右 縁
							1f, 0f,
							// 右 路肩
							1f, 0f
						};
						m_Lines = new int[]{
							// 路面
							0, 1,
							1, 2,
							// 左 路肩
							3, 4,
							// 左 縁
							5, 6,
							// 左 横
							7, 8,
							// 裏
							9, 10,
							// 右 横
							11, 12,
							// 右 縁
							13, 14,
							// 右 路肩
							15, 16
						};
						#endregion
						break;
					case 3:
						// TODO:定義追加
						#region トンネル
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
			theta = (2 * Mathf.PI) / polyNum;
		} else {
			// 半円
			polyNum = polyNum / 2;
			theta = Mathf.PI / polyNum;
			polyNum = polyNum + 1;
		}

		// 各種初期化
		m_Verts = new Vector2[polyNum];
		m_UCoords = new float[polyNum];

		// 辺を構成する頂点群　関連
		int edges;
		edges = polyNum * 2;
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

		for (int i = 0; i < polyNum; i++) {
			// Cos(theta), Sin(theta)から位置を求める
			// それにwidth(半径)をかける
			float m_x = Mathf.Cos(theta * i) * width;
			float m_y = Mathf.Sin(theta * i) * width;

			m_Verts[i] = new Vector2(m_x, m_y);

			m_UCoords[i] = 1 / polyNum;

			vt = i * 2;
			m_Lines[vt] = i;

			if (i == (polyNum - 1)) {
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
