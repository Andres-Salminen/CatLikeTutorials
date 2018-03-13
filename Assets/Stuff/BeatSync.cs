using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSync : MonoBehaviour {

	private bool _beat = false;
	private bool _beatDir = false;
	private float _beatPulse = 0f;
	[SerializeField] private float _beatDuration = 0.2f;
	[SerializeField] private float _beatInterval = 0.5f;
	[SerializeField] private float _beatRandomization = 0.5f;

	private float _timeToBeat;
	private float _timer = 0f;
	private float _beatStartTime = 0f;

	[SerializeField] private float _beatStrength = 0.5f;

	private List<Vector3> _origScale = new List<Vector3>();
	private List<Transform> _beatingObjects = new List<Transform>();
	public static BeatSync Instance;

	private bool _initialized = false;

	// Use this for initialization
	void Awake () {
		if (Instance == null)
			Instance = this;
		else
		{
			Debug.Log("Please have only one BeatSync script in a scene!");
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// if (!initialized)
		// {
		// 	foreach (var trans in beatingObjects)
		// 	{
		// 		origScale.Add(trans.localScale);
		// 	}
		// 	initialized = true;
		// }
		if (!_beat)
			_timer += Time.deltaTime;
		if (_timer >= _timeToBeat && !_beat)
		{
			_beat = true;
			_timer = 0f;
			_timeToBeat = Random.Range(_beatInterval, _beatInterval + _beatRandomization);
			_beatStartTime = Time.time;
		}
		Debug.Log("Beat: " + _beat);
		Debug.Log("Beat dir: " + _beatDir);
		if (_beat)
		{
			if (!_beatDir)
			{
				_beatPulse += (Time.deltaTime * 2f / _beatDuration) * _beatStrength;
				if (_beatPulse > _beatStrength - 0.1f)
					_beatDir = true;
			}
			else
			{
				_beatPulse -= (Time.deltaTime * 2f / _beatDuration) * _beatStrength;
				if (_beatPulse < 0.1f)
				{
					_beatDir = false;
					_beat = false;
					_beatPulse = 0f;
				}
			}

				
		}

		for (int i = 0; i < _beatingObjects.Count; ++i)
		{
			_beatingObjects[i].localScale = _origScale[i] * (1 + _beatPulse);
		}
	}

	public void AddObject(Transform trans)
	{
		_beatingObjects.Add(trans);
		_origScale.Add(trans.localScale);
	}
}
