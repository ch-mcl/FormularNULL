using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CatmullRomSpline を算出
public class Spline : MonoBehaviour {
	public ControlPoint[] m_controlPoints; // CP // 制御点 XのCPに相当

	// 全長
	[SerializeField]
	private float m_length = 0f;

	[SerializeField]
	Vector3[] m_bezierHandlePoints; // Bezierハンドル
	[SerializeField]
	private OrientedPoint[] m_curvePoints;

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
	
	[System.NonSerialized]
	private int m_curvePointIndex;

	// 区間の長さ
	[SerializeField]
	private float[] m_sectionDistance;

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
	public ControlPoint[] ControlPoints {
		get {
			return m_controlPoints;
		}
	}

	/// <summary>
	/// Bezier曲線上における点の位置、向きを取得
	/// </summary>
	public OrientedPoint[] CurvePoints {
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
	public float[] SectionDistances {
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
			m_curvePoints = new OrientedPoint[m_controlPoints.Length * m_loops]; // 曲線上における全点を保持
			m_sectionDistance = new float[m_controlPoints.Length * m_loops]; // 曲線上における全点間の距離を保持
			m_curvePointIndex = 0; // 曲線上における点の番号


			m_bezierHandlePoints = new Vector3[m_controlPoints.Length * 2]; // bezierハンドルの初期化

			m_length = 0f;

			// 始点から終点までを描画
			for (int i = 0; i < m_controlPoints.Length; i++) {
				if ((i == 0 || i == m_controlPoints.Length - 2 || i == m_controlPoints.Length - 1) && !IsLoop) {
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
		// 自動bezierハンドル
		// handlePointsの位置を計算
		for(int i = 0; i < m_controlPoints.Length; i++) {

			Vector3 hp0 = m_controlPoints[ClampCPPos(i-1)].transform.position; // 1つ前の点
			Vector3 hp1 = m_controlPoints[i].transform.position; // ハンドルを求めたい点
			Vector3 hp2 = m_controlPoints[ClampCPPos(i+1)].transform.position; // 1つ後の点

			Vector3 dir01 = hp0-hp1;
			Vector3 dir21 = hp2-hp1;

			Vector3 dir02 = hp0-hp2;

			float preDis = dir01.magnitude;
			float nextDis = -dir21.magnitude;

			dir01 = dir01.normalized;
			dir21 = -dir21.normalized;
			dir02 = dir02.normalized;

			m_bezierHandlePoints[ClampHandlePos((i*2)-1)] = hp1+(dir02*preDis*m_alpha);
			m_bezierHandlePoints[i*2] = hp1+(dir02*nextDis*m_alpha);

			m_controlPoints[i].transform.LookAt(m_bezierHandlePoints[i*2]); // CPの向きを変更
		}

		Vector3 p0 = m_controlPoints[pos].transform.position;

		Vector3 p1 = m_bezierHandlePoints[pos*2];
		Vector3 p2 = m_bezierHandlePoints[(pos*2)+1];

		Vector3 p3 = m_controlPoints[ClampCPPos(pos+1)].transform.position;
		int current = ClampCPPos(pos); // 現在CPの番号

		// 1つ前の位置
		Vector3 lastPos = p0; // Bezierは1つ前の点を含まないので上書き
		Vector3 newPos;
		float t; 

		int next = ClampCPPos(pos+1); //　次CPの番号


		Vector3 currentForward = m_controlPoints[current].transform.forward; // 現在CPのバンク角
		Vector3 nextForward = m_controlPoints[next].transform.forward; // 次CPのバンク角

		
		float currentBankAngle = -m_controlPoints[current].Bank; // 現在CPのバンク角度
		float nextBankAngle = -m_controlPoints[next].Bank; // 次CPのバンク角度

		Quaternion currentBankQ = Quaternion.AngleAxis(currentBankAngle, currentForward)*m_controlPoints[current].transform.rotation;
		Quaternion nextBankQ = Quaternion.AngleAxis(nextBankAngle, nextForward)*m_controlPoints[next].transform.rotation;

		for (int i = 1; i <= m_loops; i++) {
			t = i * m_resolution;

			newPos = GetBezierPosition(t, p0, p1, p2, p3);

			Vector3 tan = GetBezierTangent(p0, p1, p2, p3, t);

			// バンク、ロール
			Quaternion q;
			q = Quaternion.Lerp(currentBankQ, nextBankQ, t);
			Vector3 up = q*Vector3.up; // 上方向を取得
			q = Quaternion.LookRotation(tan, up);

			// 横幅の表示
			if (m_previewWidth == true) {
				// x軸の方向を描画
				Gizmos.color = Color.red;
				Gizmos.DrawLine(newPos, newPos + (q * Vector3.right) * m_controlPoints[current].WidthR);
				Gizmos.DrawLine(newPos, newPos + (q * Vector3.right) * -m_controlPoints[current].WidthL);
			}

			// 上方向の表示
			if(m_previewUp == true) {
				// y軸の方向を描画
				Gizmos.color = Color.green;
				Gizmos.DrawLine(newPos, newPos + (q * Vector3.up) * 20f);
			}
			m_curvePoints[m_curvePointIndex] = new OrientedPoint(newPos, q);

			// 線分の描画
			Gizmos.color = Color.white;
			Gizmos.DrawLine(lastPos, newPos);

			// 全長として記憶
			m_length += Mathf.Abs((newPos - lastPos).magnitude);
			// 区間長を記憶
			m_sectionDistance[m_curvePointIndex] = m_length;

			lastPos = newPos;
			m_curvePointIndex++;
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
			pos = m_controlPoints.Length - 1;
		}

		if (pos > m_controlPoints.Length) {
			pos = 1;
		} else if (pos > m_controlPoints.Length - 1) {
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
}
