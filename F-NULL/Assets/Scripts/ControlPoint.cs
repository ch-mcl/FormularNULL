using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {

	[SerializeField]
	private int m_id; // 何番目

	[SerializeField]
	private RoadTypes m_RoadType = RoadTypes.Road; // 路面形状
	[SerializeField]
	private GimicTypes m_GimicType = GimicTypes.None; // ギミック

	[SerializeField]
	private float m_widthR = 26f; // 右幅
	[SerializeField]
	private float m_widthL = 26f; // 左幅
	// 26.0 : Xのスタート地点の値(基準値)

	[SerializeField] [Range(0, 360)]
	private float m_roll = 0f; // バンク (0~360まで)

	/// <summary>
	/// IDの取得
	/// </summary>
	public int ID {
		get {
			return m_id;
		}
		set {
			if (value >= 0) {
				m_id = value;
			}
		}
	}

	/// <summary>
	/// 路面形状の取得
	/// </summary>
	public int RoadType {
		get {
			return (int)m_RoadType;
		}
	}

	/// <summary>
	/// 設置物の取得
	/// </summary>
	public int GimicType {
		get {
			return (int)m_GimicType;
		}
	}

	/// <summary>
	/// 右幅の取得
	/// </summary>
	public float WidthR {
		get {
			return m_widthR;
		}
	}

	/// <summary>
	/// 左幅の取得
	/// </summary>
	public float WidthL {
		get {
			return m_widthL;
		}
	}

	/// <summary>
	/// バンク角の取得
	/// </summary>
	public float Bank {
		get {
			return m_roll;
		}
	}

	/// <summary>
	/// 路面形状
	/// </summary>
	public enum RoadTypes {
		Road, // (NormalWall Road)
		HRoad, // (HighWall Road)
		TRoad, // (NoWall Road)
		Tunnel,
		// width を radius(半径)として扱う
		Halfpipe,
		Pipe,
		Cylinder,
		Gap/*,
		// 仕様未定
		Branch
		*/
	};

	// 
	/// <summary>
	/// 設置物
	/// </summary>
	public enum GimicTypes {
		None, // 無し
		Dash, // ブーストプレート
		Dirt, // ダート
		Pit, // ピット
		Mine, // マイン
		Jump/*, // ジャンプ台
		Gravity, // 重力エリア
		Magnet, // マグネット
		Wind // 風
		*/
	};


	
}
