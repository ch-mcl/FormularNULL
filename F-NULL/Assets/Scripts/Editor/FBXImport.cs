using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// FBXインポートの設定を変更する
public class FBXImport : AssetPostprocessor {
	//モデルインポートする直前の処理
	void OnPreprocessModel() {
		ModelImporter mdlImporter = assetImporter as ModelImporter;

		// FBXのMaterialに従ってMaterialを生成
		mdlImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
		// MaterialはLocalの「Materials」フォルダから読み込む
		mdlImporter.materialSearch = ModelImporterMaterialSearch.Local;
	}
}
