using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {

	public int m_id; // 何番目

	public float m_widthR = 26f; // 右幅
	public float m_widthL = 26f; // 左幅
	// 26.5 : Xのスタート地点の値(基準値)
	[Range(0, 360)]
	public float m_roll = 0f; // バンク (0~360まで)

	private ExtrudeShape roadShape;

	public ExtrudeShape RoadShape {
		get {
			return roadShape;
		}
	}

	public enum RoadType {
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

	// パーツ Gimic(ブーストプレート、ダート、マイン、ジャンプ台、[重力エリア、マグネット、風])
	public enum GimicType {
		None,
		Dash,
		Dirt,
		Pit,
		Mine,
		Jump/*,
		Gravity,
		Magnet,
		Wind
		*/
	};

	public RoadType m_RoadType;
	public GimicType m_GimicType;


	public void GetShape() {
		int road;

		switch (m_RoadType) {
			case RoadType.HRoad:
				road = (int)RoadType.HRoad;
				break;
			case RoadType.TRoad:
				road = (int)RoadType.TRoad;
				break;
			case RoadType.Tunnel:
				road = (int)RoadType.Tunnel;
				break;
			case RoadType.Halfpipe:
				road = (int)RoadType.Halfpipe;
				break;
			case RoadType.Pipe:
				road = (int)RoadType.Pipe;
				break;
			case RoadType.Cylinder:
				road = (int)RoadType.Cylinder;
				break;
			case RoadType.Gap:
				road = (int)RoadType.Gap;
				break;
			default:
				road = (int)RoadType.Road;
				break;
		}
		int gimic = (int)GimicType.None;
		/*
		// gimic 分岐
		*/
		roadShape = new ExtrudeShape(m_widthR, m_widthL, road, gimic);
	}
}
