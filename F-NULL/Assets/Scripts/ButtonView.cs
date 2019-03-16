using System.Collections;
using System.Collections.Generic;
using GamepadInput;
using UnityEngine;
using UnityEngine.UI;

public class ButtonView : MonoBehaviour {

	[SerializeField] private GamePad.Index player_id;  //自分が何人目のプレイヤーか設定する。

	[SerializeField] Image Abutton;
	[SerializeField] Image Bbutton;
	[SerializeField] Image Ybutton;
	[SerializeField] Image Xbutton;
	[SerializeField] Image LT;
	[SerializeField] Image RT;
	[SerializeField] Image Select;
	[SerializeField] Image StartB;
	[SerializeField] Image stick;

	float pc = 1f;
	float pa = 1f;
	float da = 0.2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GamepadState state = GamePad.GetState(player_id);

		if (state.A) {
			Abutton.color = new Color(pc,pc,pc,pa);
		} else {
			Abutton.color = new Color(pc, pc, pc, da);
		}

		if (state.B) {
			Bbutton.color = new Color(pc, pc, pc, pa);
		} else {
			Bbutton.color = new Color(pc, pc, pc, da);
		}

		if (state.Y)
		{
			Ybutton.color = new Color(pc, pc, pc, pa);
		} else {
			Ybutton.color = new Color(pc, pc, pc, da);
		}

		if (state.X) {
			Xbutton.color = new Color(pc, pc, pc, pa);
		} else {
			Xbutton.color = new Color(pc, pc, pc, da);
		}


		if (state.Start)
		{
			StartB.color = new Color(pc, pc, pc, pa);
		}
		else {
			StartB.color = new Color(pc, pc, pc, da);
		}

		if (state.Back)
		{
			Select.color = new Color(pc, pc, pc, pa);
		}
		else {
			Select.color = new Color(pc, pc, pc, da);
		}

		if (state.RightTrigger > 0)
		{
			RT.color = new Color(pc, pc, pc, pa);
		}
		else {
			RT.color = new Color(pc, pc, pc, da);
		}

		if (state.LeftTrigger > 0)
		{
			LT.color = new Color(pc, pc, pc, pa);
		}
		else {
			LT.color = new Color(pc, pc, pc, da);
		}

		//if (Mathf.Abs(state.LeftStickAxis.x) > 0 || Mathf.Abs(state.LeftStickAxis.y) > 0)
		if (Mathf.Abs(state.LeftStickAxis.x) > 0)
		{
			stick.transform.localPosition = new Vector3(state.LeftStickAxis.x * 32f, 0, 0);
			//stick.po = new Vector3(stick.rectTransform.position.x + state.LeftStickAxis.x, 0, 0);
		} else {
			stick.transform.localPosition = Vector3.zero;
		}
	}
}
