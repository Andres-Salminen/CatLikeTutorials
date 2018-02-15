using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example2 : MonoBehaviour
{
    private Quaternion _newRotation;
    private Vector3 _mousePosition;

	private Quaternion _startRotation;
	private Vector3 _startDirection;

	[SerializeField]
	[Range(1f, 179f)]
	private float _activationAngle = 90f;

	[SerializeField]
	private float _resetDuration = 1f;
	private bool _resetting;
	private float _resetStartTime = 0f;
	private Vector3 _activationDirection;
	private Quaternion _activationRotation;

    void Start()
    {
       	_newRotation = new Quaternion();

		_startRotation = transform.rotation;
		_startDirection = transform.up;
		
		_activationDirection = Quaternion.AngleAxis(-_activationAngle, transform.forward) * _startDirection;
		_activationRotation = Quaternion.LookRotation(transform.forward, _activationDirection);
		
    }

    void Update()
    {
		if (_resetting)
		{
			float t = (Time.time - _resetStartTime) / _resetDuration;
			transform.rotation = Quaternion.Lerp(_activationRotation, _startRotation, t);
			if (t >= 1f)
				_resetting = false;
		}
		else if (Input.GetMouseButton(0))
		{
			//Fetch the mouse's position
			_mousePosition = Input.mousePosition;
			
			//Fix how far into the scene the mouse should be
			//Transform the mouse position into world space

			_mousePosition.z = 10f;
			_mousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);

			Vector3 transformedPosition = transform.InverseTransformPoint(_mousePosition);
			transformedPosition.z = 0f;
			_mousePosition = transform.TransformPoint(transformedPosition);

			_newRotation = Quaternion.LookRotation(transform.forward, _mousePosition);

			float angle = Vector3.SignedAngle(_startDirection, _mousePosition, -transform.forward);
			
			if (angle > 0f)
			{
				if (angle >= _activationAngle)
				{
					
					transform.rotation = _activationRotation;
					Debug.Log("Activate");
					ResetLever();
				}
				else
					transform.rotation = _newRotation;
					
			}
			else
				transform.rotation = _startRotation;

			//Debug.Log(rotationEuler);
		}
		else
		{
			if (transform.rotation != _startRotation)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, _startRotation, 30f * Time.deltaTime);
		}
	
    }

	void ResetLever()
	{
		_resetting = true;
		_resetStartTime = Time.time;
	}
}