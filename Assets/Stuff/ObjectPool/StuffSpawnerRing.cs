using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffSpawnerRing : MonoBehaviour {

	[SerializeField] private int _numberOfSpawners;
	[SerializeField] private float _radius, _tiltAngle;
	[SerializeField] private StuffSpawner _spawnerPrefab;
	[SerializeField] private Material[] _stuffMaterials;

	// Use this for initialization
	void Awake () {
		for (int i = 0; i < _numberOfSpawners; ++i)
		{
			CreateSpawner(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreateSpawner(int index)
	{
		Transform rotater = new GameObject("Rotater").transform;
		rotater.SetParent(transform, false);
		rotater.localRotation = Quaternion.Euler(0f, index * 360f / _numberOfSpawners, 0f);

		StuffSpawner spawner = Instantiate<StuffSpawner>(_spawnerPrefab);
		spawner.transform.SetParent(rotater, false);
		spawner.transform.localPosition = new Vector3 (0f, 0f, _radius);
		spawner.transform.localRotation = Quaternion.Euler(_tiltAngle, 0f, 0f);
		spawner.StuffMaterial = _stuffMaterials[index % _stuffMaterials.Length];
	}
}
