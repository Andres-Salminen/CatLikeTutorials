using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh _hexMesh;
	List<Vector3> _vertices;
	List<int> _triangles;

	// Use this for initialization
	void Awake () {
		GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
		_hexMesh.name = "Hex Mesh";
		_vertices = new List<Vector3>();
		_triangles = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Triangulate (HexCell[] cells)
	{
		_hexMesh.Clear();
		_vertices.Clear();
		_triangles.Clear();

		for (int i = 0; i < cells.Length; ++i)
		{
			Triangulate(cells[i]);
		}

		_hexMesh.vertices = _vertices.ToArray();
		_hexMesh.triangles = _triangles.ToArray();
		_hexMesh.RecalculateNormals();
	}

	void Triangulate (HexCell cell)
	{
		Vector3 center = cell.transform.localPosition;
		for (int i = 0; i < 6; ++i)
		{
			AddTriangle(
				center,
				center + HexMetrics.corners[i],
				center + HexMetrics.corners[i + 1]
			);
		}
	}

	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = _vertices.Count;
		_vertices.Add(v1);
		_vertices.Add(v2);
		_vertices.Add(v3);
		_triangles.Add(vertexIndex);
		_triangles.Add(vertexIndex + 1);
		_triangles.Add(vertexIndex + 2);
	}
}
