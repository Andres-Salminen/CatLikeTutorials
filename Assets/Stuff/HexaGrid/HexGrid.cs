using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {
	public int Width = 6;
	public int Height = 6;

	public HexCell CellPrefab;
	public Text CellLabelPrefab;

	HexCell[] _cells;
	Canvas _gridCanvas;
	HexMesh _hexMesh;

	void Awake() 
	{
		_gridCanvas = GetComponentInChildren<Canvas>();
		_hexMesh = GetComponentInChildren<HexMesh>();

		_cells = new HexCell[Height * Width];
		for (int z = 0, i = 0; z < Height; z++)
		{
			for (int x = 0; x < Width; x ++)
				CreateCell(x, z, i++);
		}
	}

	void Start()
	{
		_hexMesh.Triangulate(_cells);
	}

	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = _cells[i] = Instantiate<HexCell>(CellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		Text label = Instantiate<Text>(CellLabelPrefab);
		label.rectTransform.SetParent(_gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2 (position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
	}
}
