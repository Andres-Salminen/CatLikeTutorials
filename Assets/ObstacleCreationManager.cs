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
	private Vector3 _referencePointOne;
	private Vector3 _referencePointTwo;
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
			if (Input.GetMouseButtonDown(0))
			{
				if (_enteringReferencePoint == 0)
				{
					_referencePointOne = Vector3.zero;
					++_enteringReferencePoint;
				}
				else if (_enteringReferencePoint == 1)
				{
					_referencePointTwo = Vector3.one;
				   ++_enteringReferencePoint;
				   CreateObstacle(_creatingType);
				}
			}
		}
		
	}

	void ResetData()
	{
		ObstacleDataContainer.CurrentObstacleData.Clear();
	}

	void CancelLastObstacle()
	{
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
					Debug.Log("Did not find a prefab with object type key. You might be missing a reference from the dictionary or the key did not match the object type.");
			}
			else
				Debug.Log("Something be broke. Cancelled data has a reference to an obstacle (should not happen).");
			ObstacleDataContainer.CurrentObstacleData.Add(cancelData);
		}
		else
		{
			Debug.Log("No obstacles to redo!");
		}
	}

	void CreateObstacle (ObjectTypes type)
	{
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
