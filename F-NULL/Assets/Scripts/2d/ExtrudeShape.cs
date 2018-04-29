using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 生成する形の定義
// ControlPointから生成される
public class ExtrudeShape {
	public Vector2[] m_verts; // 頂点位置
	public Vector2[] m_normals; // 法線の向き
	public float[] m_uCoords; // U座標 (V座標は指定しない)
	public int[] m_lines; // 頂点の接線

	private int polyNum = 24; // ハーフパイプ、パイプ、シリンダーの頂点数

	// コンストラクタ
	public ExtrudeShape(float widthR, float widthL, int road, int gimic) {

		// 半径
		float radius = widthR + widthL;
		radius = radius / 2;

		switch (road)
		{
			case 1:
				#region 高い壁
				m_verts = new Vector2[] {
					new Vector2(widthR, 0),	// 右
					new Vector2(0, 0),		// 中央
					new Vector2(-widthL, 0),	// 左

					new Vector2(widthR+2, 8),	// 右端
					new Vector2(-(widthL+2), 8),// 左端

					new Vector2(widthR+2, -2),	// 右端下
					new Vector2(-(widthL+2), -2),// 左端下
				};
				m_uCoords = new float[] {
					0f,
					0.5f,
					0f,
					1f,
					1f,
					0f,
					1f,
				};
				m_lines = new int[]{
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
				#region 壁無し(未確認)
					m_verts = new Vector2[] {
					// 路面
					new Vector2(widthR+4, 0),	// 右
					new Vector2(0, 0),		// 中央
					new Vector2(-(widthL+4), 0),// 左

					// 左 路肩
					new Vector2(-(widthL+4), 0),// 左
					new Vector2(-(widthL+2), 0),// 左 道境界

					// 左 縁
					new Vector2(-(widthL+2), 0),// 左 道境界
					new Vector2(-(widthL), 0),	// 左端

					// 左 横
					new Vector2(-(widthL), 0),   // 左端
					new Vector2(-(widthL), -2),// 左端下

					// 裏
					new Vector2(-(widthR), -2),// 左端下
					new Vector2(widthR, -2),	// 右端下

					// 右 横
					new Vector2(widthR, -2), // 右端下
					new Vector2(widthR, 0),	// 右端

					// 右 縁
					new Vector2(widthR, 0),	// 右端
					new Vector2(widthR+2, 0),	// 右 道境界
					
					// 右 路肩
					new Vector2(widthR+2, 0),	// 右 道境界
					new Vector2(widthR+4, 0),	// 右
				};
				m_uCoords = new float[] {
					// 路面
					1f, 0.5f, 0f,
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
				m_lines = new int[]{
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
				#region トンネル(未指定)
				#endregion
				break;
			case 4:
				#region ハーフパイプ(真っ暗)
				CreateCircle(radius, false, false);
				#endregion
				break;
			case 5:
				#region パイプ(真っ暗)
				CreateCircle(radius, true, false);
				#endregion
				break;
			case 6:
				#region シリンダー(真っ暗)
				CreateCircle(radius, true, true);
				#endregion
				break;
			case 7:
				#region 空白

				#endregion
				break;
			default:
				#region 基本
				float edgeR = widthR+4;
				float shoulderR = edgeR-2;

				float edgeL = widthL+4;
				float shoulderL = edgeL-2;

				m_verts = new Vector2[] {

					// 路面
					new Vector2(widthR, 0),	// 右
					new Vector2(0, 0),		// 中央
					new Vector2(-widthL, 0),// 左

					// 左 路肩
					new Vector2(-widthL, 0),// 左
					new Vector2(-shoulderL, 0),// 左 道境界

					// 左 縁
					new Vector2(-shoulderL, 0),// 左 道境界
					new Vector2(-edgeL, 0),	// 左端

					// 左 横
					new Vector2(-edgeL, 0),   // 左端
					new Vector2(-edgeL, -2),// 左端下

					// 裏
					new Vector2(-edgeL, -2),// 左端下
					new Vector2(edgeR, -2),	// 右端下

					// 右 横
					new Vector2(edgeR, -2), // 右端下
					new Vector2(edgeR, 0),	// 右端

					// 右 縁
					new Vector2(edgeR, 0),	// 右端
					new Vector2(shoulderR, 0),	// 右 道境界
					
					// 右 路肩
					new Vector2(shoulderR, 0),	// 右 道境界
					new Vector2(widthR, 0),	// 右
				};
				m_uCoords = new float[] {
					// 路面
					1f, 0.5f, 0f,
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
				m_lines = new int[]{
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
		}
	}

	// 円型道路の頂点を計算
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
		m_verts = new Vector2[polyNum];
		m_uCoords = new float[polyNum];

		// 辺を構成する頂点群　関連
		int edges;
		edges = polyNum * 2;
		/*
		// 半円の場合
		if (full == false) {
			edges = edges - 1;
		}
		*/
		m_lines = new int[edges];

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
			m_verts[i] = new Vector2(m_x, m_y);

			m_uCoords[i] = 1 / polyNum;

			vt = i * 2;
			m_lines[vt] = i;

			if (i == (polyNum - 1)) {
				// 全円の場合
				if (full == true) {
					m_lines[vt + 1] = 0;
				} else {
					m_lines[vt + 1] = i;
				}
			} else {
				m_lines[vt + 1] = i + 1;
			}
		}
	}
}
