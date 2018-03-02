using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour {

	private FPSCounter _fpsCounter;

	[SerializeField] private Text _fpsLabel;


	void Start () {
		_fpsCounter = GetComponent<FPSCounter>();
		_fpsCounter.FPS.Subscribe(x => _fpsLabel.text = Mathf.Clamp(x, 0, 99).ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
