using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {
	public int Width = 6;
	public int Height = 6;

	public HexCell CellPrefab;
	public Text CellLabelPrefab;

	private HexCell[] _cells;
	private Canvas _gridCanvas;
	private HexMesh _hexMesh;

	public Color DefaultColor = Color.white;
	

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

	public void ColorCell(Vector3 position, Color color)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * Width + coordinates.Z / 2;
		HexCell cell = _cells[index];
		cell.Color = color;
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
		cell.Color = DefaultColor;

		Text label = Instantiate<Text>(CellLabelPrefab);
		label.rectTransform.SetParent(_gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2 (position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
	}
}
