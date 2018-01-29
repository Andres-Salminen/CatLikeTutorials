using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour {

	public enum SplineWalkerMode 
	{
		Once,
		Loop,
		PingPong
	}

	public BezierSpline spline;

	public bool lookForward;

	public float duration;

	private float progress;

	public SplineWalkerMode mode;

	// Use this for initialization
	void Start () {

	}
		
	void Update () {
		progress += Time.deltaTime / duration;
		if (progress > 1f && mode != SplineWalkerMode.PingPong)
		{
			progress = 1f;
		}
		Vector3 position;
		if (mode == SplineWalkerMode.PingPong)
		{
			position = spline.GetPoint(Mathf.PingPong(progress, 1f));
		}
		else
		{
			position = spline.GetPoint(progress);
		}

		transform.localPosition = position;


		if (lookForward)
		{
			if (mode == SplineWalkerMode.PingPong)
				transform.LookAt(position + spline.GetDirection(Mathf.PingPong(progress, 1f)));
			else
				transform.LookAt(position + spline.GetDirection(progress));
		}

		if (mode == SplineWalkerMode.Loop && progress >= 1f)
		{
			progress = 0f;
		}
	}
		
}
