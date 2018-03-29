using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : PooledObject {

	public Rigidbody RBody { get; private set; }

	// Use this for initialization
	void Awake () {
		RBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "KillZone")
			ReturnToPool();
	}
}
