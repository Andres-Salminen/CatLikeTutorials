using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleonSpawner : MonoBehaviour {

	[SerializeField] private float _timeBetweenSpawns;
	[SerializeField] private float _spawnDistance;
	[SerializeField] private Nucleon[] _nucleonPrefabs;
	private float _timeSinceLastSpawn = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_timeSinceLastSpawn += Time.fixedDeltaTime;
		if (_timeSinceLastSpawn >= _timeBetweenSpawns)
		{
			_timeSinceLastSpawn -= _timeBetweenSpawns;
			SpawnNucleon();
		}
	}

	void SpawnNucleon()
	{
		Nucleon prefab = _nucleonPrefabs[Random.Range(0, _nucleonPrefabs.Length)];
		Nucleon spawn = Instantiate<Nucleon>(prefab);

		spawn.transform.localPosition = Random.onUnitSphere * _spawnDistance;
	}
}
