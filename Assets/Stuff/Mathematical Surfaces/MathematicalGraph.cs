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

	static GraphFunction[] _functions = { SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder, Sphere, Torus };

	// Use this for initialization
	void Awake () {
		_points = new Transform [ _resolution * _resolution];
		float step = 2f / _resolution;
		Vector3 scale = Vector3.one * step;
		Vector3 position = new Vector3 (0f, 0f, 0f);

		for (int i = 0; i < _points.Length; ++i)
		{
			Transform point = Instantiate(_pointPrefab);
			point.localScale = scale;
			point.SetParent(transform, false);
			_points[i] = point;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time;
		GraphFunction f = _functions[(int)_function];

		float step = 2f / _resolution;
		for (int i = 0, z = 0; z < _resolution; ++z) 
		{
			float v = (z + 0.5f) * step - 1f;
			for (int x = 0; x < _resolution; ++x, ++i)
			{
				float u = (x + 0.5f) * step - 1f;
				_points[i].localPosition = f(u, v, t);
			}
		}
	}

	static Vector3 SineFunction(float x, float z, float t) 
	{
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(_pi * (x + t));
		p.z = z;

		return p;
	}

	static Vector3 Sine2DFunction(float x, float z, float t) 
	{
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(_pi * (x + t));
		p.y += Mathf.Sin(_pi * (z + t));
		p.y *= 0.5f;
		p.z = z;

		return p;
	}

	static Vector3 MultiSineFunction(float x, float z, float t)
	{
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(_pi * (x + t));
		p.y += Mathf.Sin(2f * _pi * (x + t)) / 2f;
		p.y *= 2f / 3f;
		p.z = z;

		return p;
	}

	static Vector3 MultiSine2DFunction(float x, float z, float t)
	{
		Vector3 p;

		p.x = x;
		p.y = 4f* Mathf.Sin(_pi * (x + z + t * 0.5f));
		p.y += Mathf.Sin(_pi * (x + t));
		p.y += Mathf.Sin(2f * _pi * (z + 2f * t)) + 0.5f;
		p.y *= 1f / 5.5f;
		p.z = z;

		return p;
	}

	static Vector3 Ripple (float x, float z, float t) 
	{
		Vector3 p;

		float d = Mathf.Sqrt(x * x + z * z);
		p.x = x;
		p.y = Mathf.Sin(_pi * (4f * d - t));
		p.y /= 1f + 10f * d;
		p.z = z;
		return p;
	}

	static Vector3 Cylinder (float u, float v, float t)
	{
		Vector3 p;
		float r = 0.8f + Mathf.Sin(_pi * (6f * u + 2f * v + t)) * 0.2f;
		p.x = r * Mathf.Sin(_pi * u);
		p.y = v;
		p.z = r * Mathf.Cos(_pi * u);

		return p;
	}

	static Vector3 Sphere (float u, float v, float t)
	{
		Vector3 p;

		float r = 0.8f + Mathf.Sin(_pi * (6f * u + t)) * 0.1f;
		r += Mathf.Sin(_pi * (4f * v + t)) * 0.1f;
		float s = r * Mathf.Cos(_pi * 0.5f * v);
		p.x = s * Mathf.Sin(_pi * u);
		p.y = r * Mathf.Sin(_pi * 0.5f * v);
		p.z = s * Mathf.Cos(_pi * u);

		return p;
	}

	static Vector3 Torus (float u, float v, float t) 
	{
		Vector3 p;

		float r1 = 0.65f + Mathf.Sin(_pi * (6f * u + t)) * 0.1f;
		float r2 = 0.2f + Mathf.Sin(_pi * (4f * v + t) * 0.05f);
		float s = r2 * Mathf.Cos(_pi * v) + r1;
		p.x = s * Mathf.Sin(_pi * u);
		p.y = r2 * Mathf.Sin(_pi * v);
		p.z = s * Mathf.Cos(_pi * u);

		return p;
	}
}
