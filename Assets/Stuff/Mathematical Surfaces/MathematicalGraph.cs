using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathematicalGraph : MonoBehaviour {

	[SerializeField]
	private Transform _pointPrefab;

	[SerializeField]
	private GraphFunctionName _function;
	
	[Range(10, 100)]
	[SerializeField]
	private int _resolution = 10;

	private Transform[] _points;

	const float _pi = Mathf.PI;

	static GraphFunction[] _functions = { SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction };

	// Use this for initialization
	void Awake () {
		_points = new Transform [ _resolution * _resolution];
		float step = 2f / _resolution;
		Vector3 scale = Vector3.one * step;
		Vector3 position = new Vector3 (0f, 0f, 0f);

		for(int i = 0, z = 0; z < _resolution; ++z)
		{
			position.z = (z + 0.5f) * step - 1f;

			for (int x = 0; x < _resolution; ++x, ++i)
			{
				Transform point = Instantiate(_pointPrefab);

				position.x = (x + 0.5f) * step - 1f;
				position.y = position.x * position.x * position.x;

				point.localPosition = position;
				point.localScale = scale;

				point.SetParent(transform, false);
				_points[i] = point;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time;
		GraphFunction f = _functions[(int)_function];

		for (int i = 0; i < _points.Length; ++i)
		{
			Transform point = _points[i];
			Vector3 position = point.localPosition;
			position.y = f(position.x, position.z, t);
			point.localPosition = position;
		}
	}

	static float SineFunction(float x, float z, float t) 
	{
		return Mathf.Sin(_pi * (x + t));
	}

	static float Sine2DFunction(float x, float z, float t) 
	{
		float y = Mathf.Sin(_pi * (x + t));
		y += Mathf.Sin(_pi * (z + t));
		y *= 0.5f;
		return y;
	}

	static float MultiSineFunction(float x, float z, float t)
	{
		float y = Mathf.Sin(_pi * (x + t));
		y += Mathf.Sin(2f * _pi * (x + t)) / 2f;
		y *= 2f / 3f;
		return y;
	}

	static float MultiSine2DFunction(float x, float z, float t)
	{
		float y = 4f* Mathf.Sin(_pi * (x + z + t * 0.5f));
		y += Mathf.Sin(_pi * (x + t));
		y += Mathf.Sin(2f * _pi * (z + 2f * t)) + 0.5f;
		y *= 1f / 5.5f;
		return y;
	}
}
