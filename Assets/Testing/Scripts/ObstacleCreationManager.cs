using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObstacleCreationManager : MonoBehaviour {

	private const string SaveFile = "obstacles.json";

	// The Container for all the obstacle data. Contains rotation, position, scale, object type and a nonserialized reference to the object transform.
	public ObstacleContainer ObstacleDataContainer;
	public Dictionary<string, GameObject> ObstaclePrefabs;



	private ObjectTypes _creatingType;
	private bool _creating;
	private int _enteringReferencePoint;
	private Vector3[] _referencePoints = new Vector3[5];
	//private Vector3 _referencePointThree;

	public enum ObjectTypes
	{
		Cube,
		Cylinder
	};

	// Use this for initialization
	void Start () {
		// Perhaps change later to persistentDataPath or additional folders etc.
		string path = Path.Combine(Application.dataPath, SaveFile);
		if (File.Exists(path))
		{
			Debug.Log("Loading saved data.");
			string loadedData = File.ReadAllText(path);
			ObstacleDataContainer = JsonUtility.FromJson<ObstacleContainer>(loadedData);
		}
		else
		{
			Debug.Log("Saved data doesn't exist, creating a file for save data.");
			ObstacleDataContainer = new ObstacleContainer();

			string savedData = JsonUtility.ToJson(ObstacleDataContainer);
			File.WriteAllText(path, savedData);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_creating)
		{
			// Add input for vive controller 1 and 2 and take the position of the one that was pressed.
			if (Input.GetMouseButtonDown(0))
			{
				AddReferencePoint(Vector3.zero);
			}
		}
		
	}

	void AddReferencePoint(Vector3 position)
	{
		// Take the inputted vector and put it in the reference point array.
		// If we're creating a box, after the third reference point, create the object.
		// If we're creating a cylinder, after the fifth reference point, go over all the reference points
		// to calculate the circle center point and radius.

		if (_creating)
		{
			if (_creatingType == ObjectTypes.Cube)
			{
				_referencePoints[_enteringReferencePoint] = position;
				if (_enteringReferencePoint == 2)
				{
					_creating = false;
					CreateObstacle(_creatingType);
				}
				else
					++_enteringReferencePoint;
			}

			if (_creatingType == ObjectTypes.Cylinder)
			{
				_referencePoints[_enteringReferencePoint] = position;
				if (_enteringReferencePoint == 4)
				{
					_creating = false;
					CreateObstacle(_creatingType);
				}
				else
					++_enteringReferencePoint;
			}
		}
	}

	void ResetData()
	{
		// Go over all the created obstacles, destroy the gameobject and add the data of the object
		// to the cancelled data list so we can undo the deleting.

		int count = ObstacleDataContainer.CurrentObstacleData.Count;
		for (int i = 0; i < count; ++i)
		{
			ObstacleData obsData = ObstacleDataContainer.CurrentObstacleData[i];
			GameObject.Destroy(obsData.ReferenceToObstacle.gameObject);
			obsData.ReferenceToObstacle = null;
			ObstacleDataContainer.CancelledObstacleData.Add(obsData);
		}
		ObstacleDataContainer.CurrentObstacleData.Clear();
	}

	void CancelLastObstacle()
	{
		// Take the last created object and destroy the gameobject and add it
		// to cancelled data list so we can undo the deleting of it.

		int indexOfLast = ObstacleDataContainer.CurrentObstacleData.Count-1;
		if (indexOfLast >= 0)
		{
			ObstacleData cancelData = ObstacleDataContainer.CurrentObstacleData[indexOfLast];
			ObstacleDataContainer.CurrentObstacleData.RemoveAt(indexOfLast);
			if (cancelData.ReferenceToObstacle != null)
			{
				GameObject.Destroy(cancelData.ReferenceToObstacle.gameObject);
				cancelData.ReferenceToObstacle = null;
			}
			else
				Debug.Log("Something be broke. Obstacle data is missing a reference to the obstacle it has data on.");
			ObstacleDataContainer.CancelledObstacleData.Add(cancelData);
		}
		else
		{
			Debug.Log("No obstacles to cancel!");
		}
	}

	void RedoLastCancelledObstacle()
	{
		// Take the last object that was deleted by the user, create the gameobject again
		// and remove the data from cancelled data and add it to instantiated data.

		int indexOfLast = ObstacleDataContainer.CancelledObstacleData.Count-1;
		if (indexOfLast >= 0)
		{
			ObstacleData cancelData = ObstacleDataContainer.CancelledObstacleData[indexOfLast];
			ObstacleDataContainer.CancelledObstacleData.RemoveAt(indexOfLast);
			if (cancelData.ReferenceToObstacle == null)
			{
				GameObject go;
				ObstaclePrefabs.TryGetValue(cancelData.ObjectType, out go);
				if (go != null)
				{
					Transform reference = GameObject.Instantiate(go, cancelData.Position, cancelData.Rotation, this.transform).transform;
					reference.localScale = cancelData.Scale;
					cancelData.ReferenceToObstacle = reference;
					ObstacleDataContainer.CurrentObstacleData.Add(cancelData);
				}
				else
					Debug.LogWarning("Did not find a prefab with object type key. You might be missing a reference from the dictionary or the key did not match the object type.");
			}
			else
				Debug.LogWarning("Something be broke. Cancelled data has a reference to an obstacle (should not happen).");
			ObstacleDataContainer.CurrentObstacleData.Add(cancelData);
		}
		else
		{
			Debug.Log("No obstacles to redo!");
		}
	}

	void CreateObstacle (ObjectTypes type)
	{
		for (int i = 0; i < _enteringReferencePoint + 1; ++i)
		{
			_referencePoints[i].y = 0f;
		}
		switch (type)
		{
			case ObjectTypes.Cube:
				Vector3 frontLeftCorner = _referencePoints[0]; Vector3 frontRightCorner = _referencePoints[1]; Vector3 backRightCorner = _referencePoints[2];
				Vector3 position = backRightCorner - frontLeftCorner;
				position = frontLeftCorner + position.normalized * (position.magnitude * 0.5f);

				Quaternion rotation = Quaternion.LookRotation(backRightCorner - frontRightCorner, Vector3.up);

				Vector3 scale = new Vector3 (
					(frontRightCorner - frontLeftCorner).magnitude,
					Mathf.Max((frontRightCorner - frontLeftCorner).magnitude, (backRightCorner - frontRightCorner).magnitude),
					(backRightCorner - frontRightCorner).magnitude);
				
				string objectType = type.ToString();

				ObstacleData obsData = CreateObstacleDataAndInstantiate(position, rotation, scale, objectType);

				ObstacleDataContainer.CurrentObstacleData.Add(obsData);

				break;
			
			case ObjectTypes.Cylinder:
				break;
			
			default:
				Debug.LogWarning("Something is wrong with object types, object type case not found when creating object.");
				break;
		}
	}

	ObstacleData CreateObstacleDataAndInstantiate(Vector3 position, Quaternion rotation, Vector3 scale, string objectType)
	{
		ObstacleData obsData = new ObstacleData();
		obsData.Position = position;
		obsData.Rotation = rotation;
		obsData.Scale = scale;
		obsData.ObjectType = objectType;

		GameObject prefab = null;
		ObstaclePrefabs.TryGetValue(objectType, out prefab);
		if (prefab != null)
		{
			GameObject go = GameObject.Instantiate(prefab, obsData.Position, obsData.Rotation, this.transform);
			go.transform.localScale = scale;
			obsData.ReferenceToObstacle = go.transform;
		}
		else
			Debug.LogWarning("Did not find a prefab with object type key. You might be missing a reference from the dictionary or the key did not match the object type.");

		return obsData;
	}

	public void StartCreating(ObjectTypes type)
	{
		_enteringReferencePoint = 0;
		_creatingType = type;
		_creating = true;
	}

	public void CancelCreating()
	{
		_creating = false;
	}
	
}
