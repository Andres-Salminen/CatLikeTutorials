using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObstacleData {
	public Vector3 Position;
	public Vector3 Scale;
	public Quaternion Rotation;
	public ObjectTypes ObjectType;

	[System.NonSerialized]
	public Transform ReferenceToObstacle;
}
