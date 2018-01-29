using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MyMesh : MonoBehaviour {

	public int XSize, YSize;

	private Mesh mesh;

	private Vector3[] vertices;
	private Vector2[] uv;
	private Vector4[] tangents;

	// Use this for initialization
	void Awake () {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		var GenerateMethod = Observable.Start(() =>
		{

			vertices = new Vector3[(XSize + 1) * (YSize + 1)];
			uv = new Vector2[vertices.Length];
			tangents = new Vector4[vertices.Length];
			Vector4 tangent = new Vector4(0f, 1f, 0f, -1f);

			for (int i = 0, y = 0; y <= YSize; ++y)
			{
				for (int x = 0; x <= XSize; ++x, ++i)
				{
					vertices[i] = new Vector3 (x, y);
					uv[i] = new Vector2((float)x / XSize, (float)y / YSize);
					tangents[i] = tangent;
				}
			}
		});

		GenerateMethod
		.ObserveOnMainThread()
		.Subscribe(_ => {
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.tangents = tangents;
			Debug.Log("Here I am.");
			
			int[] triangles = new int[XSize * YSize * 6];

			for (int ti = 0, vi = 0, y = 0; y < YSize; ++y, ++vi)
			{
				for (int x = 0; x < XSize; ++x, ti += 6, ++vi)
				{
					triangles[ti] = vi;
					triangles[ti + 3] = triangles[ti + 2] = vi + 1;
					triangles[ti + 4] = triangles[ti + 1] = vi + XSize + 1;
					triangles[ti + 5] = vi + XSize + 2;
				}
			}

			mesh.triangles = triangles;
			mesh.RecalculateNormals();
		});


		Debug.Log("Is it a thread?");
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnDrawGizmos() 
	{
		// if (vertices == null)
		// 	return;

		// Gizmos.color = Color.black;
		// for (int i = 0; i < vertices.Length; ++i)
		// {
		// 	Gizmos.DrawSphere(vertices[i], 0.1f);
		// }
	}
}
