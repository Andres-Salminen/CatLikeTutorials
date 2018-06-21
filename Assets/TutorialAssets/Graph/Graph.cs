using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

	public Transform pointPrefab;
	[Range(10, 100)]
	public int resolution = 10;

	private Transform[] points;

	// Use this for initialization
	void Awake () {
		points = new Transform[resolution];
		float step = 2f / resolution;
		Vector3 scale = Vector3.one * step;
		Vector3 position = new Vector3 (0f, 0f, 0f);

		for(int i = 0; i < resolution; ++i)
		{
			Transform point = Instantiate(pointPrefab);
			position.x = (i + 0.5f) * step - 1f;
			position.y = position.x * position.x * position.x;
			point.localPosition = position;
			point.localScale = scale;
			point.SetParent(transform, false);
			points[i] = point;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time;
		for (int i = 0; i < points.Length; ++i)
		{
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position.y = MultiSineFunction(position.x, t);
			point.localPosition = position;
		}
	}

	float SineFunction(float x, float t) 
	{
		return Mathf.Sin(Mathf.PI * (x + t));
	}

	float MultiSineFunction(float x, float t)
	{
		float y = Mathf.Sin(Mathf.PI * (x + t));
		y += Mathf.Sin(2f * Mathf.PI * (x + t)) / 2f;
		y *= 2f / 3f;
		return y;
	}
}
