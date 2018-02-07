using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	private Mesh _hexMesh;
	private List<Vector3> _vertices;
	private List<int> _triangles;
	private MeshCollider _meshCollider;

	private List<Color> _colors;
	// Use this for initialization
	void Awake () {
		GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
		_meshCollider = gameObject.AddComponent<MeshCollider>();

		_hexMesh.name = "Hex Mesh";
		_vertices = new List<Vector3>();
		_triangles = new List<int>();
		_colors = new List<Color>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Triangulate (HexCell[] cells)
	{
		_hexMesh.Clear();
		_vertices.Clear();
		_triangles.Clear();
		_colors.Clear();

		for (int i = 0; i < cells.Length; ++i)
		{
			Triangulate(cells[i]);
		}

		_hexMesh.vertices = _vertices.ToArray();
		_hexMesh.colors = _colors.ToArray();
		_hexMesh.triangles = _triangles.ToArray();
		_hexMesh.RecalculateNormals();

		_meshCollider.sharedMesh = _hexMesh;
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

			AddTriangleColor(cell.Color);
		}
	}

	void AddTriangleColor(Color color)
	{
		_colors.Add(color);
		_colors.Add(color);
		_colors.Add(color);
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
