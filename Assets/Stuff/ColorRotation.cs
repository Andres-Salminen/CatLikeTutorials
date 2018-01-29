using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRotation : MonoBehaviour {


	float colorRot;
	public float CycleSpeed = 1f;

	Material material;
	// Use this for initialization
	void Start () {
		material = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		colorRot = Mathf.PingPong(Time.time * CycleSpeed, 1f);
		material.SetFloat("_ColorRotation", colorRot);
	}
}
