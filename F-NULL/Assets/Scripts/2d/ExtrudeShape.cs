using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//生成する形の定義
//ControlPointから生成される
public class ExtrudeShape {
	#region 定数
	///<summary>
	///路肩
	///</summary>
	private const float m_edge = 0.8f;

	///<summary>
	///路肩幅
	///</summary>
	private const float m_shoulder = 0.8f;

	///<summary>
	///路肩で最も低い点
	///</summary>
	private const float m_edgeBottom = -0.2f;

	///<summary>
	///路面　底辺
	///</summary>
	private const float m_roadBottom = -2f;

	///<summary>
	///壁幅
	///</summary>
	private const float m_wallWidth = 4f;

	///<summary>
	///壁　高さ(通常)
	///</summary>
	private const float m_wallHeight = 0.8f;

	///<summary>
	///壁　高さ(高い場合)
	///</summary>
	private const float m_wallHeightHigh = 8f;

	///<summary>
	///トンネルの最高点
	///</summary>
	private const float m_tunnelHeight = 16f;

	/// <summary>
	/// 物体の厚さ
	/// (トンネル、ハーフパイプ、パイプの外側との厚さ)
	/// </summary>
	private const float m_objThin = 2f;

	#endregion

	#region フィールド
	///<summary>
	///ハーフパイプ、パイプ、シリンダーの頂点数
	///</summary>
	private int m_polyNum = 24;

	///<summary>
	///頂点位置
	///</summary>
	private Vector2[] m_Vertices;
	
	///<summary>
	///法線の向き
	///</summary>
	private Vector2[] m_Normals;
	
	///<summary>
	///辺
	///</summary>
	private int[] m_Lines;
	
	///<summary>
	///U座標
	///(V座標はモデル作成時に設定)
	///</summary>
	private float[] m_UCoords;
	#endregion

	#region プロパティ
	///<summary>
	///頂点位置を取得
	///</summary>
	public Vector2[] Vertices {
		get {
			return m_Vertices;
		}
	}

	///<summary>
	///法線を取得
	///</summary>
	public Vector2[] Normals {
		get {
			return m_Normals;
		}
	}

	///<summary>
	///辺を取得
	///</summary>
	public int[] Lines {
		get {
			return m_Lines;
		}
	}

	///<summary>
	///U座標を取得
	///</summary>
	public float[] UCoords {
		get {
			return m_UCoords;
		}
	}
	#endregion

	///<summary>
	///コンストラクタ
	///</summary>
	///<param name="widthR"></param>
	///<param name="widthL"></param>
	///<param name="road"></param>
	///<param name="gimic"></param>
	public ExtrudeShape(int meshType, float t, float widthR, float widthL, int road) {

		//道路の中心
		float center = widthR - widthL;
		if(Mathf.Abs(center) > 0) {
			//幅が右/左のどちらかに寄っている場合
			//右幅が大きい: 正の値 
			//左幅が大きい: 負の値
			center = center / 2f;
		}

		//半径
		float radius = widthR + widthL;
		radius = radius / 2;

		float edgeR = widthR - m_edge;
		float edgeL = widthL - m_edge;

		float shoulderR = edgeR - m_shoulder;
		float shoulderL = edgeL - m_shoulder;

		//壁
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
						//0~3
						m_Vertices = new Vector2[] {

							//路面
							new Vector2(widthR, 0),	//右
							new Vector2(center, 0),		//中央
							new Vector2(-widthL, 0)	//左
						};
						m_UCoords = new float[] {
							//路面
							1f, 0.5f, 0f
						};
						m_Lines = new int[]{
							//路面
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
							//路面
							new Vector2(widthR, m_wallHeightHigh),		
							new Vector2(widthR, 0),	//右
							new Vector2(-widthL, 0),	//左
							new Vector2(-widthL, m_wallHeightHigh)
						};
						m_UCoords = new float[] {
							//路面
							0f, 1f, 0f, 1f
						};
						m_Lines = new int[]{
							//路面
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
							//路面
							new Vector2(shoulderR, m_edgeBottom),	//右
							new Vector2(center, 0),	//中央
							new Vector2(center, 0),	//中央
							new Vector2(-shoulderL, m_edgeBottom),	//左

							//左 路肩
							new Vector2(-shoulderL, m_edgeBottom),	//左
							new Vector2(-edgeL, 0),					//左 道境界

							//左 縁
							new Vector2(-edgeL, 0),		//左 道境界
							new Vector2(-widthL, 0),	//左端

							//左 横
							new Vector2(-(widthL + m_wallWidth), m_wallHeight),   //左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),	//左端下

							//裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),	//左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	//右端下

							//右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom),	//右端下
							new Vector2(widthR + m_wallWidth, m_wallHeight),	//右端

							//右 縁
							new Vector2(widthR, 0),	//右端
							new Vector2(edgeR, 0),	//右 道境界
					
							//右 路肩
							new Vector2(edgeR, 0),					//右 道境界
							new Vector2(shoulderR, m_edgeBottom),	//右

							//右壁
							new Vector2(wallR, m_wallHeight),
							new Vector2(widthR, 0),

							//左壁
							new Vector2(-widthL, 0),
							new Vector2(-wallL, m_wallHeight)
						};
						m_UCoords = new float[] {
							//路面
							0.5f, 0f, 
							0f, 0.5f,
							//左 路肩
							0.75f, 0.625f,
							//左 縁
							0.75f, 0.625f,
							//左 横
							0.75f, 0.625f,
							//裏
							1f, 0.75f, 
							//右 横
							0.75f, 0.625f,
							//右 縁
							0.75f, 0.625f,
							//右 路肩
							0.75f, 0.625f,
							//右壁
							0.625f, 0.5f,
							//左壁
							0.625f, 0.5f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							//左 路肩
							4, 5,
							//左 縁
							6, 7,
							//左 横
							8, 9,
							//裏
							10, 11,
							//右 横
							12, 13,
							//右 縁
							14, 15,
							//右 路肩
							16, 17,
							//右壁
							18, 19,
							//左壁
							20, 21
						};
						#endregion
						break;
					case 1:
						#region 高い壁
						m_Vertices = new Vector2[] {
							//路面
							new Vector2(shoulderR, m_edgeBottom),	//右
							new Vector2(center, 0),	//中央
							new Vector2(center, 0),	//中央
							new Vector2(-shoulderL, m_edgeBottom),//左

							//左 路肩
							new Vector2(-shoulderL, m_edgeBottom),//左
							new Vector2(-edgeL, 0),//左 道境界

							//左 縁
							new Vector2(-edgeL, 0),//左 道境界
							new Vector2(-widthL, 0),	//左端

							//左 横
							new Vector2(-(widthL + m_wallWidth), m_wallHeightHigh),   //左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),//左端下

							//裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),//左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	//右端下

							//右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom), //右端下
							new Vector2(widthR + m_wallWidth, m_wallHeightHigh),	//右端

							//右 縁
							new Vector2(widthR, 0),	//右端
							new Vector2(edgeR, 0),	//右 道境界
					
							//右 路肩
							new Vector2(edgeR, 0),	//右 道境界
							new Vector2(shoulderR, m_edgeBottom),	//右

							//右壁
							new Vector2(wallR, m_wallHeightHigh),
							new Vector2(widthR, 0),

							//左壁
							new Vector2(-widthL, 0),
							new Vector2(-wallL, m_wallHeightHigh)
						};
						m_UCoords = new float[] {
							//路面
							0.5f, 0f,
							0f, 0.5f,
							//左 路肩
							0.75f, 0.625f,
							//左 縁
							0.75f, 0.625f,
							//左 横
							0.75f, 0.625f,
							//裏
							1f, 0.75f, 
							//右 横
							0.75f, 0.625f,
							//右 縁
							0.75f, 0.625f,
							//右 路肩
							0.75f, 0.625f,
							//右壁
							0.625f, 0.5f,
							//左壁
							0.625f, 0.5f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							//左 路肩
							4, 5,
							//左 縁
							6, 7,
							//左 横
							8, 9,
							//裏
							10, 11,
							//右 横
							12, 13,
							//右 縁
							14, 15,
							//右 路肩
							16, 17,
							//右壁
							18, 19,
							//左壁
							20, 21
						};
						#endregion
						break;
					case 2:
						#region 壁無し

						shoulderL = shoulderL - m_wallWidth;
						shoulderR = shoulderR - m_wallWidth;
						edgeL = edgeL - m_wallWidth;
						edgeR = edgeR - m_wallWidth;

						m_Vertices = new Vector2[] {
							//路面
							new Vector2(shoulderR, m_edgeBottom),	//右
							new Vector2(center, 0),	//中央
							new Vector2(center, 0),	//中央
							new Vector2(-shoulderL, m_edgeBottom),//左

							//左 路肩
							new Vector2(-shoulderL, m_edgeBottom),//左
							new Vector2(-edgeL, 0),//左 道境界

							//左 縁
							new Vector2(-edgeL, 0),//左 道境界
							new Vector2(-widthL, 0),	//左端

							//左 横
							new Vector2(-widthL, 0),   //左端
							new Vector2(-widthL, m_roadBottom),//左端下

							//裏
							new Vector2(-widthL, m_roadBottom),//左端下
							new Vector2(widthR, m_roadBottom),	//右端下

							//右 横
							new Vector2(widthR, m_roadBottom), //右端下
							new Vector2(widthR, 0),	//右端

							//右 縁
							new Vector2(widthR, 0),	//右端
							new Vector2(edgeR, 0),	//右 道境界
					
							//右 路肩
							new Vector2(edgeR, 0),	//右 道境界
							new Vector2(shoulderR, m_edgeBottom),	//右
						};
						m_UCoords = new float[] {
							//路面
							0.5f, 0f,
							0f, 0.5f,
							//左 路肩
							0.75f, 0.625f,
							//左 縁
							0.75f, 0.625f,
							//左 横
							0.75f, 0.625f,
							//裏
							1f, 0.75f, 
							//右 横
							0.75f, 0.625f,
							//右 縁
							0.75f, 0.625f,
							//右 路肩
							0.75f, 0.625f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							//左 路肩
							4, 5,
							//左 縁
							6, 7,
							//左 横
							8, 9,
							//裏
							10, 11,
							//右 横
							12, 13,
							//右 縁
							14, 15,
							//右 路肩
							16, 17
						};
						#endregion
						break;
					case 3:
						#region トンネル

						float outTop = m_tunnelHeight + m_objThin;
						float inRadius = radius + m_wallWidth;
						float outRadius = inRadius + m_objThin;

						m_Vertices = new Vector2[] {
							//路面
							new Vector2(shoulderR, m_edgeBottom),	//右
							new Vector2(center, 0),	//中央
							new Vector2(center, 0),	//中央
							new Vector2(-shoulderL, m_edgeBottom),//左

							//左 路肩
							new Vector2(-shoulderL, m_edgeBottom),//左
							new Vector2(-edgeL, 0),//左 道境界

							//左 縁
							new Vector2(-edgeL, 0),//左 道境界
							new Vector2(-widthL, 0),	//左端

							//左 横
							new Vector2(-(widthL + m_wallWidth + m_objThin), m_wallHeightHigh),   //左端
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),//左端下

							//裏
							new Vector2(-(widthL + m_wallWidth), m_roadBottom),//左端下
							new Vector2(widthR + m_wallWidth, m_roadBottom),	//右端下

							//右 横
							new Vector2(widthR + m_wallWidth, m_roadBottom), //右端下
							new Vector2(widthR + m_wallWidth + m_objThin, m_wallHeightHigh),	//右端

							//右 縁
							new Vector2(widthR, 0),	//右端
							new Vector2(edgeR, 0),	//右 道境界
					
							//右 路肩
							new Vector2(edgeR, 0),	//右 道境界
							new Vector2(shoulderR, m_edgeBottom),	//右

							//右壁
							new Vector2(wallR, m_wallHeightHigh),
							new Vector2(widthR, 0),

							//左壁
							new Vector2(-widthL, 0),
							new Vector2(-wallL, m_wallHeightHigh),

							//天井(左内)
							new Vector2(-wallL, m_wallHeightHigh),
							TunnelCelling(0.25f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.25f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.5f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.5f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.75f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.75f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(1f, -inRadius, center, m_wallHeightHigh, m_tunnelHeight),

							//天井(右内)
							TunnelCelling(1f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.75f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.75f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.5f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.5f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.25f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							TunnelCelling(0.25f, inRadius, center, m_wallHeightHigh, m_tunnelHeight),
							new Vector2(wallR, m_wallHeightHigh),

							//天井(左外)
							TunnelCelling(0.25f, -outRadius, center, m_wallHeightHigh, outTop),
							new Vector2(-(wallL+m_objThin), m_wallHeightHigh),
							TunnelCelling(0.5f, -outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.25f, -outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.75f, -outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.5f, -outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(1f, -outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.75f, -outRadius, center, m_wallHeightHigh, outTop),

							//天井(右外)
							TunnelCelling(0.75f, outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(1f, outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.5f, outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.75f, outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.25f, outRadius, center, m_wallHeightHigh, outTop),
							TunnelCelling(0.5f, outRadius, center, m_wallHeightHigh, outTop),
							new Vector2(wallR+m_objThin, m_wallHeightHigh),
							TunnelCelling(0.25f, outRadius, center, m_wallHeightHigh, outTop)
						};
						m_UCoords = new float[] {
							//路面
							0.5f, 0f,
							0f, 0.5f,
							//左 路肩
							0.75f, 0.625f,
							//左 縁
							0.75f, 0.625f,
							//左 横
							0.75f, 0.625f,
							//裏
							1f, 0.75f, 
							//右 横
							0.75f, 0.625f,
							//右 縁
							0.75f, 0.625f,
							//右 路肩
							0.75f, 0.625f,
							//右壁
							0.625f, 0.5f,
							//左壁
							0.625f, 0.5f,
							//天井(左内)
							0.625f, 0.5f,
							0f, 0f,
							0f, 0f,
							0f, 0f,
							//天井(右内)
							0f, 0f,
							0f, 0f,
							0f, 0f,
							0.625f, 0.5f,
							//天井(左外)
							0f, 0f,
							0f, 0f,
							0f, 0f,
							0f, 0f,
							//天井(右外)
							0f, 0f,
							0f, 0f,
							0f, 0f,
							0f, 0f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							//左 路肩
							4, 5,
							//左 縁
							6, 7,
							//左 横
							8, 9,
							//裏
							10, 11,
							//右 横
							12, 13,
							//右 縁
							14, 15,
							//右 路肩
							16, 17,
							//右壁
							18, 19,
							//左壁
							20, 21,
							//天井(左内)
							22, 23,
							24, 25,
							26, 27,
							28, 29,
							//天井(右内)
							30, 31,
							32, 33,
							34, 35,
							36, 37,
							//天井(左外)
							38, 39,
							40, 41,
							42, 43,
							44, 45,
							//天井(左外)
							46, 47,
							48, 49,
							50, 51,
							52, 53
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
			case 3:
				#region エリア
				switch (road) {
					case 0:
					case 1:
						#region なし
						#endregion
						break;
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						#region 右
						m_Vertices = new Vector2[] {
							new Vector2(widthR-0.8f, 0),
							new Vector2(widthR-0.8f, m_wallHeightHigh),

							new Vector2(widthR*0.4f-0.8f, m_wallHeightHigh),
							new Vector2(widthR*0.4f-0.8f, 0),

							new Vector2(widthR-0.8f, m_wallHeightHigh),
							new Vector2(widthR*0.4f-0.8f, m_wallHeightHigh)
						};
						m_UCoords = new float[] {
							//路面
							0f, 1f,
							0f, 1f,
							0f, 1f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							4, 5
						};
						#endregion
						break;
					case 2:
						#region 左
						m_Vertices = new Vector2[] {
							new Vector2(-widthL*0.4f-0.8f, 0),
							new Vector2(-widthL*0.4f-0.8f, m_wallHeightHigh),

							new Vector2(-widthL-0.8f, m_wallHeightHigh),
							new Vector2(-widthL-0.8f, 0),

							new Vector2(-widthL*0.4f-0.8f, m_wallHeightHigh),
							new Vector2(-widthL-0.8f, m_wallHeightHigh)
						};
						m_UCoords = new float[] {
							//路面
							0f, 1f,
							0f, 1f,
							0f, 1f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							4, 5
						};
						#endregion
						break;
					case 3:
						#region 両方
						m_Vertices = new Vector2[] {
							new Vector2(widthR-0.8f, 0),
							new Vector2(widthR-0.8f, m_wallHeightHigh),

							new Vector2(widthR*0.4f-0.8f, m_wallHeightHigh),
							new Vector2(widthR*0.4f-0.8f, 0),

							new Vector2(widthR-0.8f, m_wallHeightHigh),
							new Vector2(widthR*0.4f-0.8f, m_wallHeightHigh),

							new Vector2(-widthL*0.4f-0.8f, 0),
							new Vector2(-widthL*0.4f-0.8f, m_wallHeightHigh),

							new Vector2(-widthL-0.8f, m_wallHeightHigh),
							new Vector2(-widthL-0.8f, 0),

							new Vector2(-widthL*0.4f-0.8f, m_wallHeightHigh),
							new Vector2(-widthL-0.8f, m_wallHeightHigh)
						};
						m_UCoords = new float[] {
							//路面
							0f, 1f,
							0f, 1f,
							0f, 1f,
							0f, 1f,
							0f, 1f,
							0f, 1f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							4, 5,
							6, 7,
							8, 9,
							10, 11
						};
						#endregion
						break;
					case 4:
						#region 中央
						m_Vertices = new Vector2[] {
							new Vector2(widthR*0.4f-0.8f, 0),
							new Vector2(widthR*0.4f-0.8f, m_wallHeightHigh),

							new Vector2(-widthL*0.4f-0.8f, m_wallHeightHigh),
							new Vector2(-widthL*0.4f-0.8f, 0),

							new Vector2(widthR*0.4f-0.8f, m_wallHeightHigh),
							new Vector2(-widthL*0.4f-0.8f, m_wallHeightHigh)
						};
						m_UCoords = new float[] {
							//路面
							0f, 1f,
							0f, 1f,
							0f, 1f
						};
						m_Lines = new int[]{
							//路面
							0, 1,
							2, 3,
							4, 5
						};
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

	///<summary>
	///トンネル天井カーブを計算
	///</summary>
	///<param name="t"></param>
	///<param name="radius"></param>
	///<param name="centerX"></param>
	///<param name="centerY"></param>
	///<param name="top"></param>
	///<returns></returns>
	private Vector2 TunnelCelling(float t, float radius, float centerX, float centerY, float top) {
		Vector2 pos;	//算出して得た頂点の位置

		float startRad = Mathf.PI * -0.5f;
		float rad = Mathf.PI * t * 0.5f;

		float mx = Mathf.Sin(startRad+rad);
		float my = Mathf.Cos(startRad+rad);

		pos = new Vector2(mx * -radius + centerX, my * top + centerY);

		return pos;
	}

	///<summary>
	///円型道路の頂点を計算
	///</summary>
	///<param name="width"></param>
	///<param name="full"></param>
	///<param name="outside"></param>
	private void CreateCircle(float width, bool full, bool outside) {
		float theta; //回転角度

		width = width - 2f;

		//円の種類
		if (full == true) {
			//全円
			theta = (2 * Mathf.PI) / m_polyNum;
		} else {
			//半円
			m_polyNum = m_polyNum / 2;
			theta = Mathf.PI / m_polyNum;
			m_polyNum = m_polyNum + 1;
		}

		//各種初期化
		m_Vertices = new Vector2[m_polyNum];
		m_UCoords = new float[m_polyNum];

		//辺を構成する頂点群　関連
		int edges;
		edges = m_polyNum * 2;
		/*
		//半円の場合
		if (full == false) {
			edges = edges - 1;
		}
		*/
		m_Lines = new int[edges];

		//外向きにする
		if (outside == false) {
			theta *= -1;
		}

		int vt = 0; //辺を構成する頂点配列中の添え字

		for (int i = 0; i < m_polyNum; i++) {
			//Cos(theta), Sin(theta)から位置を求める
			//それにwidth(半径)をかける
			float m_x = Mathf.Cos(theta * i) * width;
			float m_y = Mathf.Sin(theta * i) * width;

			m_Vertices[i] = new Vector2(m_x, m_y);

			m_UCoords[i] = 1 / m_polyNum;

			vt = i * 2;
			m_Lines[vt] = i;

			if (i == (m_polyNum - 1)) {
				//全円の場合
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
