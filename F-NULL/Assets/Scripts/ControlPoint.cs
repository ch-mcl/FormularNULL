using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {
	/// <summary>
	/// CPの番号
	/// </summary>
	[SerializeField]
	private int m_id;

	/// <summary>
	/// 路面形状
	/// [default:Road]
	/// </summary>
	[SerializeField]
	private RoadTypes m_RoadType = RoadTypes.Road;

	/// <summary>
	/// 設置物(右)
	/// [default:None]
	/// </summary>
	[SerializeField]
	private ItemTypes m_ItemR = ItemTypes.None;

	/// <summary>
	/// 設置物(中央)
	/// [default:None]
	/// </summary>
	[SerializeField]
	private ItemTypes m_ItemCenter = ItemTypes.None;

	/// <summary>
	/// 設置物(左)
	/// [default:None]
	/// </summary>
	[SerializeField]
	private ItemTypes m_ItemL = ItemTypes.None;

	//幅
	//26.0f Xのスタート地点の値(基準値)
	/// <summary>
	/// 幅(右)
	/// [default:26.0f]
	/// </summary>
	[SerializeField]
	private float m_widthR = 26f;
	/// <summary>
	/// 幅(左)
	/// [default:26.0f]
	/// </summary>
	[SerializeField]
	private float m_widthL = 26f;


	//エリアサイズ
	//回復、スリップ、減速
	/// <summary>
	/// エリアサイズ(右)
	/// [default:60%]
	/// </summary>
	[SerializeField]
	private float m_AreaSizeR = 0.6f;

	/// <summary>
	/// エリアサイズ(左)
	/// [default:60%]
	/// </summary>
	[SerializeField]
	private float m_AreaSizeL = 0.6f;

	/// <summary>
	/// バンク角
	/// (0度以上、360度未満)
	/// [default:0.0f]
	/// </summary>
	[SerializeField] [Range(0, 360)]
	private float m_roll = 0f;

	/// <summary>
	/// IDのプロパティ
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
	/// 路面形状のプロパティ
	/// </summary>
	public int RoadType {
		get {
			return (int)m_RoadType;
		}
	}

	/// <summary>
	/// 設置物(右)のプロパティ
	/// </summary>
	public int ObjectR {
		get {
			return (int)m_ItemCenter;
		}
	}

	/// <summary>
	/// 設置物(中央)のプロパティ
	/// </summary>
	public int ObjectCenter {
		get {
			return (int)m_ItemCenter;
		}
	}

	/// <summary>
	/// 設置場所(左)のプロパティ
	/// </summary>
	public int ObjectL {
		get {
			return (int)m_ItemL;
		}
	}

	/// <summary>
	/// 幅(右)のプロパティ
	/// </summary>
	public float WidthR {
		get {
			return m_widthR;
		}
	}

	/// <summary>
	/// 幅(左)のプロパティ
	/// </summary>
	public float WidthL {
		get {
			return m_widthL;
		}
	}

	/// <summary>
	/// バンク角のプロパティ
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

	/// <summary>
	/// 設置物
	/// </summary>
	public enum ItemTypes {
		None, // 無し
		Pit, // ピット
		Dash, // ブーストプレート
		Dirt, // ダート
		Jump/*, // ジャンプ台
		Mine, // マイン
		Force //重力エリア, マグネット, 風
		*/
	};
}
