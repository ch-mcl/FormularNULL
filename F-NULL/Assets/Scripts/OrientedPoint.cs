using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ExtrudeShapeの頂点を回転させるための構造体
// 構造体になっていたがクラスでいい
public class OrientedPoint {

	public Vector3 m_position;
	public Quaternion m_rotation;

	// コンストラクタ
	public OrientedPoint(Vector3 position, Quaternion rotation) {
		m_position = position;
		m_rotation = rotation;
	}

	public Vector3 LocalToWorld(Vector3 point) {
		return m_position + m_rotation * point;
	}

	public Vector3 WorldToLocal(Vector3 point) {
		return Quaternion.Inverse(m_rotation) * (point - m_position);
	}

	public Vector3 LocalToWorldDirction(Vector3 dir) {
		return m_rotation * dir;
	}
}
