using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FPSCounter : MonoBehaviour {

	public ReactiveProperty<int> AverageFPS { get; private set; }
	public ReactiveProperty<int> HighestFPS { get; private set; }
	public ReactiveProperty<int> LowestFPS { get; private set; }

	[SerializeField] private int _avgFrameRange = 60;

	private int[] _fpsBuffer;
	private int _fpsBufferIndex;

	// Use this for initialization
	void Awake () {
		AverageFPS = new ReactiveProperty<int>(0);
		HighestFPS = new ReactiveProperty<int>(0);
		LowestFPS = new ReactiveProperty<int>(99);
	}
	
	// Update is called once per frame
	void Update () {
		if (_fpsBuffer == null || _fpsBuffer.Length != _avgFrameRange)
			InitializeBuffer();
		
		UpdateBuffer();
		CalculateFPS();
	}

	void InitializeBuffer()
	{
		if (_avgFrameRange <= 0)
			_avgFrameRange = 1;
		
		_fpsBuffer = new int[_avgFrameRange];
		_fpsBufferIndex = 0;
	}

	void UpdateBuffer()
	{
		_fpsBuffer[_fpsBufferIndex++] = (int) (1f / Time.unscaledDeltaTime);

		if (_fpsBufferIndex >= _avgFrameRange)
			_fpsBufferIndex = 0;
	}

	void CalculateFPS() 
	{
		int sum = 0;
		int highest = 0;
		int lowest = int.MaxValue;
		
		for (int i = 0; i < _avgFrameRange; i++)
		{
			int fps = _fpsBuffer[i];
			sum += fps;
			if (fps > highest)
				highest = fps;
			if (fps < lowest)
				lowest = fps;
		}

		AverageFPS.Value = sum / _avgFrameRange;
		HighestFPS.Value = highest;
		LowestFPS.Value = lowest;
	}
}
