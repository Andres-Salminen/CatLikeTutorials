using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FPSCounter : MonoBehaviour {

	public ReactiveProperty<int> AverageFPS { get; private set; }
	public ReactiveProperty<int> HighestFPS { get; private set; }
	public ReactiveProperty<int> LowestFPS { get; private set; }

	public ReactiveProperty<float> AverageMS { get; private set; }
	public ReactiveProperty<float> HighestMS { get; private set; }
	public ReactiveProperty<float> LowestMS { get; private set; }

	[SerializeField] private int _avgFrameRange = 60;

	private int[] _fpsBuffer;
	private float[] _msBuffer;
	private int _fpsBufferIndex;
	private int _msBufferIndex;
	

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 300;

		AverageFPS = new ReactiveProperty<int>(0);
		HighestFPS = new ReactiveProperty<int>(0);
		LowestFPS = new ReactiveProperty<int>(99);

		AverageMS = new ReactiveProperty<float>(0f);
		HighestMS = new ReactiveProperty<float>(0f);
		LowestMS = new ReactiveProperty<float>(99f);
	}
	
	// Update is called once per frame
	void Update () {
		if (_fpsBuffer == null || _fpsBuffer.Length != _avgFrameRange)
			InitializeBuffer();
		
		UpdateBuffers();
		CalculateFPS();
		CalculateMS();

	}

	void InitializeBuffer()
	{
		if (_avgFrameRange <= 0)
			_avgFrameRange = 1;
		
		_fpsBuffer = new int[_avgFrameRange];
		_msBuffer = new float[_avgFrameRange];
		_fpsBufferIndex = 0;
		_msBufferIndex = 0;
	}

	void UpdateBuffers()
	{
		_fpsBuffer[_fpsBufferIndex++ % _fpsBuffer.Length] = (int) (1f / Time.unscaledDeltaTime);

		_msBuffer[_msBufferIndex++ % _msBuffer.Length] = Time.unscaledDeltaTime * 1000f;
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

	void CalculateMS()
	{
		float sum = 0f;
		float highest = 0f;
		float lowest = float.MaxValue;
		
		for (int i = 0; i < _avgFrameRange; i++)
		{
			float ms = _msBuffer[i];
			sum += ms;
			if (ms > highest)
				highest = ms;
			if (ms < lowest)
				lowest = ms;
		}

		AverageMS.Value = sum / _avgFrameRange;
		HighestMS.Value = highest;
		LowestMS.Value = lowest;
	}
}
