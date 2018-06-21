using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (BeatSync.Instance != null)
			BeatSync.Instance.AddObject(transform);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
