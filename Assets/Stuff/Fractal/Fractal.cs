using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Fractal : MonoBehaviour {

	public Mesh mesh;
	public Material material;

	public Transform fractalPrefab;

	public int maxDepth;

	public float childScale;

	private int depth = 0;

	public float maxRotationSpeed;

	public float maxTwist;

	private Vector3 rotationSpeed;

	// Use this for initialization

	void Start () {
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;

		gameObject.AddComponent<BeatScript>();
		gameObject.AddComponent<ColorRotation>();

		transform.Rotate(UnityEngine.Random.Range(-maxTwist, maxTwist), 0f, 0f);

		rotationSpeed = new Vector3(UnityEngine.Random.Range(-maxRotationSpeed, maxRotationSpeed), UnityEngine.Random.Range(-maxRotationSpeed, maxRotationSpeed), UnityEngine.Random.Range(-maxRotationSpeed, maxRotationSpeed));
		if (depth < maxDepth) 
		{
			Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
			{
				new GameObject("fractalPrefab").AddComponent<Fractal>().Initialize(this, Vector3.up, Quaternion.identity);
			});

			Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
			{
				new GameObject("fractalPrefab").AddComponent<Fractal>().Initialize(this, Vector3.down, Quaternion.Euler(180f, 0f, 0f));
			});
			
			Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
			{
				new GameObject("fractalPrefab").AddComponent<Fractal>().Initialize(this, Vector3.right, Quaternion.Euler(0f, 0f, -90f));
			});
			
			Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
			{
				new GameObject("fractalPrefab").AddComponent<Fractal>().Initialize(this, Vector3.left, Quaternion.Euler(0f, 0f, 90f));
			});
		}
	}

	private void Initialize(Fractal parent, Vector3 direction, Quaternion orientation)
	{
		mesh = parent.mesh;
		material = parent.material;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		transform.parent = parent.transform;
		childScale = parent.childScale;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = direction * (0.5f + 0.5f * childScale);
		transform.localRotation = orientation;
		fractalPrefab = parent.fractalPrefab;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(rotationSpeed.x * Time.deltaTime, 0f, 0f);
	}
}
