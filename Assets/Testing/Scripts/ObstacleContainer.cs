using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleContainer {
	public List<ObstacleData> CurrentObstacleData;

	[System.NonSerialized]
	public List<ObstacleData> CancelledObstacleData;

	public ObstacleContainer()
	{
		CurrentObstacleData = new List<ObstacleData>();
		CancelledObstacleData = new List<ObstacleData>();

		
		/*
		// Test code start.
		ObstacleData data = new ObstacleData();
		data.Position = Vector3.zero; data.Rotation = Quaternion.identity; data.Scale = Vector3.one; data.ObjectType = "Cube";
		CurrentObstacleData.Add(data);
		// Test code end.
		*/
	}

}
