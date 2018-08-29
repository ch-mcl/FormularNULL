using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CatmullRomSpline を算出
public class Spline : MonoBehaviour {
	[SerializeField]
	private List <ControlPoint> m_controlPoints = new List<ControlPoint>(); // CP // 制御点 XのCPに相当

	// 全長
	[SerializeField]
	private float m_length = 0f;

	[SerializeField]
	Vector3[] m_bezierHandlePoints; // Bezierハンドル

	[SerializeField]
	private List<OrientedPoint> m_curvePoints = new List<OrientedPoint>();

	// 横幅表示フラグ
	[SerializeField]
	bool m_previewHandle = false;

	// 横幅表示フラグ
	[SerializeField]
	private bool m_previewWidth = false;

	// 上方向表示フラグ
	[SerializeField]
	private bool m_previewUp = false;

	// 細かさ
	[SerializeField]
	[Range(0.01f, 1.0f)]
	private float m_resolution = 0.04f;

	// 点間における分割数
	[System.NonSerialized]
	private int m_loops;

	// 区間の長さ
	[SerializeField]
	private List<float> m_sectionDistance = new List<float>();

	// ループになっているか (常に true)
	private bool m_isLooping = true;

	// アルファ値
	private float m_alpha = 0.4f; 


	/// <summary>
	/// ループフラグの取得
	/// </summary>
	public bool IsLoop{
		get { 
			return m_isLooping;
		}
	}

	/// <summary>
	/// ControlPointの取得
	/// </summary>
	public List<ControlPoint> ControlPoints {
		get {
			return m_controlPoints;
		}
		set {
			m_controlPoints = value;
		}
	}

	/// <summary>
	/// Bezier曲線上における点の位置、向きを取得
	/// </summary>
	public List<OrientedPoint> CurvePoints {
		get {
			return m_curvePoints;
		}
		set {
			m_curvePoints = value;
		}
	}

	/// <summary>
	/// 点間の距離を取得
	/// </summary>
	public List<float> SectionDistances {
		get {
			return m_sectionDistance;
		}
	}

	/// <summary>
	/// 点間の分割数を取得
	/// </summary>
	public int Loops {
		get {
			return m_loops;
		}
	}

	/// <summary>
	/// SceneViewに描画
	/// </summary>
	void OnDrawGizmos(){

		// bezier曲線の細かさ
		m_loops = Mathf.FloorToInt(1f / m_resolution);

		// ControlPointが無効か判定
		if (m_controlPoints != null) {
			Gizmos.color = Color.white;
			m_curvePoints.Clear(); // 曲線上における全点を保持
			m_sectionDistance.Clear(); // 曲線上における全点間の距離を保持

			m_bezierHandlePoints = new Vector3[m_controlPoints.Count * 2]; // bezierハンドルの初期化
			AutoHandle();

			m_length = 0f;

			// 始点から終点までを描画
			for (int i = 0; i < m_controlPoints.Count; i++) {
				if ((i == m_controlPoints.Count - 2 || i == m_controlPoints.Count - 1) && !IsLoop) {
					continue;
				}

				DisplaySpline(i); // 線分を描画

				Gizmos.DrawWireSphere(m_controlPoints[i].transform.position, 20f);

			}
		} else {
			Debug.LogError("Invalid : ControlPoints");
		}
	}

	/// <summary>
	/// bezier曲線を描画
	/// </summary>
	/// <param name="pos"></param>
	public void DisplaySpline(int pos) {
		Vector3 p0 = m_controlPoints[pos].transform.position;

		Vector3 p1 = m_bezierHandlePoints[pos * 2];
		Vector3 p2 = m_bezierHandlePoints[(pos * 2) + 1];

		Vector3 p3 = m_controlPoints[ClampCPPos(pos + 1)].transform.position;
		int current = ClampCPPos(pos); // 現在CPの番号

		// 1つ前の位置
		Vector3 lastPos = p0; // Bezierは1つ前の点を含まないので上書き
		Vector3 newPos;
		float t = 0;

		int next = ClampCPPos(pos + 1); //　次CPの番号

		ControlPoint currentCP = m_controlPoints[current];
		ControlPoint nextCP = m_controlPoints[next];


		Vector3 currentForward = currentCP.transform.forward; // 現在CPのバンク角
		Vector3 nextForward = nextCP.transform.forward; // 次CPのバンク角


		float currentBankAngle = -currentCP.Bank; // 現在CPのバンク角度
		float nextBankAngle = -nextCP.Bank; // 次CPのバンク角度

		Quaternion currentBankQ = Quaternion.AngleAxis(currentBankAngle, currentForward) * currentCP.transform.rotation;
		Quaternion nextBankQ = Quaternion.AngleAxis(nextBankAngle, nextForward) * nextCP.transform.rotation;

		for (int i = 0; i < m_loops; i++) {
			t = i*m_resolution;

			newPos = GetBezierPosition(t, p0, p1, p2, p3);

			Vector3 tan = GetBezierTangent(p0, p1, p2, p3, t);

			// バンク、ロール
			Quaternion q;
			q = Quaternion.Lerp(currentBankQ, nextBankQ, t);
			Vector3 up = q * Vector3.up; // 上方向を取得
			q = Quaternion.LookRotation(tan, up);

			// 横幅の表示
			if (m_previewWidth == true) {
				// x軸の方向を描画
				float widthR = 0f;
				float widthL = 0f;

				// 右幅が変化する場合
				if (currentCP.WidthR != nextCP.WidthR) {
					// 右幅を更新
					widthR = currentCP.WidthR + t * (nextCP.WidthR - currentCP.WidthR);
				} else {
					widthR = currentCP.WidthR;
				}

				if (currentCP.WidthL != nextCP.WidthL) {
					// 左幅を更新
					widthL = currentCP.WidthL + t * (nextCP.WidthL - currentCP.WidthL);
				} else {
					widthL = currentCP.WidthL;
				}

				Gizmos.color = Color.red;
				Gizmos.DrawLine(newPos, newPos + (q * Vector3.right) * widthR);
				Gizmos.DrawLine(newPos, newPos + (q * Vector3.right) * -widthL);
			}

			// 上方向の表示
			if (m_previewUp == true) {
				// y軸の方向を描画
				Gizmos.color = Color.green;
				Gizmos.DrawLine(newPos, newPos + (q * Vector3.up) * 20f);
			}
			m_curvePoints.Add(new OrientedPoint(newPos, q));

			// 線分の描画
			Gizmos.color = Color.white;
			Gizmos.DrawLine(lastPos, newPos);

			// 全長として記憶
			float distance = Mathf.Abs((newPos - lastPos).magnitude);
			m_length += distance;
			// 区間長を記憶
			m_sectionDistance.Add(m_length);

			lastPos = newPos;
		}

		// Bezierハンドルを表示
		if (m_previewHandle) {
			Gizmos.color = Color.gray;
			Gizmos.DrawLine(p0, p1);
			Gizmos.DrawLine(p2, p3);

			Gizmos.DrawWireSphere(p1, 10f);
			Gizmos.DrawWireSphere(p2, 10f);
		}
	}

	/// <summary>
	/// 自動bezierハンドル
	/// handlePointsの位置を計算
	/// </summary>
	private void AutoHandle() {
		for (int i = 0; i < m_controlPoints.Count; i++) {

			Vector3 hp0 = m_controlPoints[ClampCPPos(i - 1)].transform.position; // 1つ前の点
			Vector3 hp1 = m_controlPoints[i].transform.position; // ハンドルを求めたい点
			Vector3 hp2 = m_controlPoints[ClampCPPos(i + 1)].transform.position; // 1つ後の点

			Vector3 dir01 = hp0 - hp1;
			Vector3 dir21 = hp2 - hp1;

			Vector3 dir02 = hp0 - hp2;

			float preDis = dir01.magnitude;
			float nextDis = -dir21.magnitude;

			dir01 = dir01.normalized;
			dir21 = -dir21.normalized;
			dir02 = dir02.normalized;

			m_bezierHandlePoints[ClampHandlePos((i * 2) - 1)] = hp1 + (dir02 * preDis * m_alpha);
			m_bezierHandlePoints[i * 2] = hp1 + (dir02 * nextDis * m_alpha);

			m_controlPoints[i].transform.LookAt(m_bezierHandlePoints[i * 2]); // CPの向きを変更
		}
	}

	/// <summary>
	/// Bezier曲線上における位置を取得
	/// </summary>
	/// <param name="t"></param>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="p3"></param>
	/// <returns></returns>
	Vector3 GetBezierPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		float t2 = t*t;
		float t3 = t2*t;

		float a0 = -t3 + 3f*t2 - 3f*t + 1f;
		float a1 = 3f*t3 - 6f*t2 + 3f*t;
		float a2 = -3f*t3 + 3f*t2;
		float a3 = t3;


		Vector3 pos = p0*a0 + p1*a1 + p2*a2 + p3*a3;

		return pos;
	}

	/// <summary>
	/// 向きを取得
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="p3"></param>
	/// <param name="t"></param>
	/// <param name="up"></param>
	/// <returns></returns>
	Quaternion GetOrientation3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Vector3 up) {
		Vector3 tng = GetBezierTangent(p0, p1, p2, p3, t);
		Vector3 nrm = GetNormal3D(p0, p1, p2, p3, t, up);
		return Quaternion.LookRotation(tng, nrm);
	}

	/// <summary>
	/// 法線を取得
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="p3"></param>
	/// <param name="t"></param>
	/// <param name="up"></param>
	/// <returns></returns>
	Vector3 GetNormal3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Vector3 up) {
		Vector3 tng = GetBezierTangent(p0, p1, p2, p3, t);
		Vector3 bionormal = Vector3.Cross(up, tng).normalized;
		return Vector3.Cross(tng, bionormal);
	}

	/// <summary>
	/// 接線を取得
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="p3"></param>
	/// <param name="t"></param>
	/// <returns></returns>
	Vector3 GetBezierTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float t2 = t*t;

		float a0 = -3f*t2 + 6f*t - 3f;
		float a1 = 9f*t2 - 12f*t + 3f;
		float a2 = -9f*t2 + 6f*t;
		float a3 = 3f*t2;

		/*
		// 微分前 ( -> より右は微分後)
		float a0 = -t3 + 3f*t2 - 3f*t + 1f; -> 3f*-t2 + 6f*t -3f
		float a1 = 3f*t3 - 6f*t2 + 3f*t; -> 9f*t2 -12f*t + 3f
		float a2 = -3f*t3 + 3f*t2; -> -9f*t2 + 6f*t
		float a3 = t3; -> 3f*t2
		*/

		Vector3 tangent = p0*a0 + p1*a1 + p2*a2 + p3*a3;

		return tangent.normalized;
	}

	/// <summary>
	/// IndexをControlPointsの中だけに制限
	/// </summary>
	/// <param name="pos"></param>
	/// <returns>丸めた値</returns>
	public int ClampCPPos(int pos) {
		if (pos < 0) {
			pos = m_controlPoints.Count - 1;
		}

		if (pos > m_controlPoints.Count) {
			pos = 1;
		} else if (pos > m_controlPoints.Count - 1) {
			pos = 0;
		}

		return pos;
	}

	/// <summary>
	/// handlePointsの中だけに制限する
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public int ClampHandlePos(int pos) {
		if (pos < 0) {
			pos = m_bezierHandlePoints.Length - 1;
		}

		if (pos > m_bezierHandlePoints.Length) {
			pos = 1;
		} else if (pos > m_bezierHandlePoints.Length - 1) {
			pos = 0;
		}

		return pos;
	}

	public void GetCP() {
		m_controlPoints.Clear();

		ControlPoint[] cps = GetComponentsInChildren<ControlPoint>();

		for (int i = 0; i < cps.Length; i++) {
			m_controlPoints.Add(cps[i]);
		}
	}

	/// <summary>
	/// CPの名前を更新する
	/// </summary>
	public void ChangeCPName() {
		m_controlPoints[0].name = (m_controlPoints.Count - 1).ToString();
		m_controlPoints[0].ID = (m_controlPoints.Count - 1);
		for (int i = 0; i < m_controlPoints.Count - 1; i++) {
			m_controlPoints[i + 1].name = i.ToString();
			m_controlPoints[i + 1].ID = i;
		}
	}

	/// <summary>
	/// CPを挿入する
	/// </summary>
	/// <param name="insertIndex"></param>
	public void InsertCP(int insertIndex) {
		if(insertIndex > m_controlPoints.Count - 1) {
			// CP最大値を超える場合
			insertIndex = m_controlPoints.Count - 1;
		} else if(insertIndex == m_controlPoints.Count - 1) {
			// 1つ目に追加する場合
			insertIndex = 0;
		} else {
			// 追加する場合
			insertIndex += 1;
		}

		GameObject cp = Instantiate(m_controlPoints[insertIndex].gameObject);
		cp.transform.parent = transform;

		Vector3 p0 = m_controlPoints[insertIndex].transform.position;
		Vector3 p1= m_bezierHandlePoints[insertIndex * 2];
		Vector3 p2 = m_bezierHandlePoints[(insertIndex * 2) + 1];
		Vector3 p3 = m_controlPoints[ClampCPPos(insertIndex + 1)].transform.position;


		Vector3 cpPos = GetBezierPosition(0.5f, p0, p1, p2, p3);

		cp.transform.position = cpPos;
		cp.gameObject.transform.SetSiblingIndex(insertIndex+1);

		GetCP();
		ChangeCPName();
	}

	/// <summary>
	/// CPを追加する
	/// </summary>
	public void AddCP() {
		InsertCP(ControlPoints.Count - 2);
	}
}
