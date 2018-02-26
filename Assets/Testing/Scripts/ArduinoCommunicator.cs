using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UniRx;

public class ArduinoCommunicator : MonoBehaviour {

	private bool _dataRead = false;
	private Subject<int> _readBuffer = new Subject<int>();

	private SerialPort _serial = new SerialPort("COM6", 9600);

	// Use this for initialization
	void Start () {
		_serial.Open();
		_serial.ReadTimeout = 2;

		_readBuffer.Subscribe(_ => {
			_serial.Write("2");
		});

	}
	
	// Update is called once per frame
	void Update () {
		try 
		{
			_readBuffer.OnNext(_serial.ReadByte());
		}
		catch (System.Exception)
		{

		}
	}
}
