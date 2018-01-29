using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		BeatSync syncronizer = BeatSync.Instance;
		if (syncronizer != null)
			syncronizer.AddObject(transform);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
