using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffSpawner : MonoBehaviour {

	[SerializeField] private FloatRange _timeBetweenSpawns, _scale, _randomVelocity, _angularVelocity;
	[SerializeField] private Stuff[] _stuffPrefabs;
	[HideInInspector] public Material StuffMaterial;

	private float _timeSinceLastSpawn;
	private float _currentSpawnDelay;
	[SerializeField] private float _velocity;

	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_timeSinceLastSpawn += Time.deltaTime;
		if (_timeSinceLastSpawn >= _currentSpawnDelay)
		{
			_timeSinceLastSpawn -= _currentSpawnDelay;
			_currentSpawnDelay = _timeBetweenSpawns.RandomInRange;
			SpawnStuff();
		}
	}

	void SpawnStuff()
	{
		Stuff prefab = _stuffPrefabs[Random.Range(0, _stuffPrefabs.Length)];
		Stuff spawn = prefab.GetPooledInstance<Stuff>();
		spawn.transform.localPosition = transform.position;
		spawn.transform.localScale = Vector3.one * _scale.RandomInRange;
		spawn.transform.localRotation = Random.rotation;
		spawn.RBody.velocity = transform.up * _velocity + Random.onUnitSphere * _randomVelocity.RandomInRange;
		spawn.RBody.angularVelocity = Random.onUnitSphere * _angularVelocity.RandomInRange;
		spawn.GetComponent<MeshRenderer>().material = StuffMaterial;
	}
}
