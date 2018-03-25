using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EngineEffect))]
[CanEditMultipleObjects]

// EngineEffectに必要なparticleSystemを子から入手する
public class EngineEffecttInspector : Editor {
	EngineEffect engineEffect;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		// particleSystemを子から入手する
		if (GUILayout.Button("Get Engines")) {
			engineEffect = target as EngineEffect;
			engineEffect.engines = engineEffect.GetComponentsInChildren<ParticleSystem>();

			// ついでに名前変更
			for (int i = 0; i < engineEffect.engines.Length; i++){
				engineEffect.engines[i].name = "Engine_" + (i+1).ToString();
			}
		}
	}
}
