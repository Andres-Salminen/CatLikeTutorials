﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FloatRange {
	public float Min, Max;

	public float RandomInRange 
	{
		get { return Random.Range(Min, Max);}
	}
}
