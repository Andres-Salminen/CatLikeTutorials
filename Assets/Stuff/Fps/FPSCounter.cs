using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FPSCounter : MonoBehaviour {

	public ReactiveProperty<int> FPS { get; private set; }

	// Use this for initialization
	void Awake () {
		FPS = new ReactiveProperty<int>(0);
	}
	
	// Update is called once per frame
	void Update () {
		FPS.Value = (int)(1f / Time.unscaledDeltaTime);
	}
}
