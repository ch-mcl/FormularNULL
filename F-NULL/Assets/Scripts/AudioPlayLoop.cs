using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayLoop : MonoBehaviour {

	public AudioClip intro;
	public AudioClip loop;
	public float delay = 0.1f;

	AudioSource[] audioSource = new AudioSource[2];

	// Use this for initialization
	void Start () {
		audioSource[0] = this.gameObject.AddComponent<AudioSource>();
		audioSource[1] = this.gameObject.AddComponent<AudioSource>();

		if (intro != null) {
			audioSource[0].playOnAwake = false;
			audioSource[0].clip = intro;
			audioSource[0].PlayScheduled(AudioSettings.dspTime + delay);
			if(loop != null) {
				audioSource[1].playOnAwake = false;
				audioSource[1].clip = loop;
				audioSource[1].loop = true;
				audioSource[1].PlayScheduled(AudioSettings.dspTime + delay + intro.length);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
