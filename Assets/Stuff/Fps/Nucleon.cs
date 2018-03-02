using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Nucleon : MonoBehaviour {

	public float AttractionForce;
	private Rigidbody _rBody;

	void Awake () {
		_rBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		_rBody.AddForce(transform.localPosition * -AttractionForce);
	}
}
