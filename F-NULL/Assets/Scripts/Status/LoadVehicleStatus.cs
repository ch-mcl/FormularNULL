using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

// マシン設定を読み取る

public class LoadVehicleStatus : MonoBehaviour {
	[SerializeField] string dbname; // 読み込むcsvファイル
	/*
	csv内容
		マシンID
		マシン名
		最高速度
		機体重量
		ボディ耐久度
		ブースト力
		グリップ
		機体旋回速度
		モデルへのパス
		出力カーブ
	*/
	Dictionary<int, VehicleStatus> vehicleDic = new Dictionary<int, VehicleStatus>(); // マシン番号とマシンステータスを紐づけ

	void Awake() {
		string line; // 文字列を格納
		
		StreamReader reader = new StreamReader(Application.dataPath + "/" + dbname + ".csv", System.Text.Encoding.GetEncoding("UTF-8"));

		string[] s;

		// csvsから文字列を読み取り、辞書へ登録
		while((line = reader.ReadLine()) != null) {
			s = line.Split(',');

			// パフォーマンス数値を配列にする
			float[] peform = {
				float.Parse(s[10]), float.Parse(s[11]), float.Parse(s[12]), float.Parse(s[13]), float.Parse(s[14]), float.Parse(s[15]), float.Parse(s[16]), float.Parse(s[17]), float.Parse(s[18]), float.Parse(s[19])
			};

			// マシンステータスの登録
			VehicleStatus dbv = new VehicleStatus(
				s[1], float.Parse(s[2]), int.Parse(s[3]), float.Parse(s[4]), float.Parse(s[5]), float.Parse(s[6]), float.Parse(s[7]), s[8], peform
			);

			vehicleDic.Add(int.Parse(s[0]), dbv);
		}
	}

	//マシン性能を教える
	public VehicleStatus GetVehicleStatus(int id) {
		return vehicleDic[id];
	}
}
