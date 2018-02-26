using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct PrefabInfo {
	public ObjectTypes Key;
	public GameObject prefab;
}

[System.Serializable]
public struct CircleInfo {
	public Vector2 position;
	public float radius;

	public CircleInfo(Vector2 pos, float r)
	{
		position = pos;
		radius = r;
	}
}

public enum ObjectTypes
{		
	Cube,
	Cylinder
};


public class ObstacleCreationManager : MonoBehaviour {


	[SerializeField]
	private PrefabInfo[] _keyPrefabPairsEditor;
	private GameObject[] _prefabs;
	private const string SaveFile = "obstacles.json";

	// The Container for all the obstacle data. Contains rotation, position, scale, object type and a nonserialized reference to the object transform.

	[SerializeField]
	private ObstacleContainer ObstacleDataContainer;



	private ObjectTypes _creatingType;
	private bool _creating;
	private int _enteringReferencePoint;
	private Vector3[] _referencePoints = new Vector3[5];
	//private Vector3 _referencePointThree;



	// Use this for initialization
	void Start () {

		_prefabs = new GameObject[_keyPrefabPairsEditor.Length];
		foreach (var pI in _keyPrefabPairsEditor)
		{
			if (_prefabs[(int)pI.Key] == null)
				_prefabs[(int)pI.Key] = pI.prefab;
			else
				throw new System.Exception("Multiple prefabs with same key!");
		}

		
		
		LoadData();
		// Perhaps change later to persistentDataPath or additional folders etc.

	}
	
	// Update is called once per frame
	void Update () {
		if (_creating)
		{
			// Add input for vive controller 1 and 2 and take the position of the one that was pressed.
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 _mousePosition = Input.mousePosition;
			
				//Fix how far into the scene the mouse should be
				//Transform the mouse position into world space

				_mousePosition.z = 10f;
				_mousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);
				Vector3 _referencePoint = Vector3.zero;
				_referencePoint.x = _mousePosition.x;
				_referencePoint.z = _mousePosition.y;
				AddReferencePoint(_referencePoint);
			}
		}
		
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (_creating)
				CancelCreating();
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			if (!_creating)
				StartCreating(ObjectTypes.Cube);
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			if (!_creating)
				StartCreating(ObjectTypes.Cylinder);
		}

		if (Input.GetKeyDown(KeyCode.R))
			ResetData();

		if (Input.GetKeyDown(KeyCode.Z))
			UndoLastObstacle();

		if (Input.GetKeyDown(KeyCode.Y))
			RedoLastObstacle();
	}

	void AddReferencePoint(Vector3 position)
	{
		// Take the inputted vector and put it in the reference point array.
		// If we're creating a box, after the third reference point, create the object.
		// If we're creating a cylinder, after the fifth reference point, go over all the reference points
		// to calculate the circle center point and radius.

		if (_creating)
		{
			Debug.Log("Adding reference point of: " + position);
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
			DestroyImmediate(obsData.ReferenceToObstacle.gameObject);
			obsData.ReferenceToObstacle = null;
			ObstacleDataContainer.CancelledObstacleData.Add(obsData);
		}
		ObstacleDataContainer.CurrentObstacleData.Clear();
		SaveData();
	}

	void UndoLastObstacle()
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
				DestroyImmediate(cancelData.ReferenceToObstacle.gameObject);
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

		SaveData();
	}

	void RedoLastObstacle()
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
				Transform reference = GameObject.Instantiate(_prefabs[(int)cancelData.ObjectType], cancelData.Position, cancelData.Rotation, this.transform).transform;
				reference.localScale = cancelData.Scale;
				cancelData.ReferenceToObstacle = reference;
				ObstacleDataContainer.CurrentObstacleData.Add(cancelData);
			}
			else
				Debug.LogWarning("Something be broke. Cancelled data has a reference to an obstacle (should not happen).");
		}
		else
		{
			Debug.Log("No obstacles to redo!");
		}

		SaveData();
	}

	void CreateObstacle (ObjectTypes type)
	{
		Vector3 position;
		Vector3 scale;
		Quaternion rotation;
		ObstacleData obsData;

		for (int i = 0; i < _enteringReferencePoint + 1; ++i)
		{
			_referencePoints[i].y = 0f;
		}
		switch (type)
		{
			case ObjectTypes.Cube:
				Vector3 frontLeftCorner = _referencePoints[0]; Vector3 frontRightCorner = _referencePoints[1]; Vector3 backRightCorner = _referencePoints[2];
				position = backRightCorner - frontLeftCorner;
				position = frontLeftCorner + position.normalized * (position.magnitude * 0.5f);

				rotation = Quaternion.LookRotation(backRightCorner - frontRightCorner, Vector3.up);

				scale = new Vector3 (
					(frontRightCorner - frontLeftCorner).magnitude,
					Mathf.Max((frontRightCorner - frontLeftCorner).magnitude, (backRightCorner - frontRightCorner).magnitude),
					(backRightCorner - frontRightCorner).magnitude);

				position.y += scale.y / 2f;

				obsData = CreateObstacleDataAndInstantiate(position, rotation, scale, _creatingType);

				ObstacleDataContainer.CurrentObstacleData.Add(obsData);

				break;
			
			case ObjectTypes.Cylinder:
				List<Vector2> referencePoints = new List<Vector2>();

				for (int i = 0; i < _enteringReferencePoint + 1; ++i)
				{
					Vector2 vec = new Vector2(_referencePoints[i].x, _referencePoints[i].z);
					referencePoints.Add(vec);
				}

				CircleInfo circleInfo = ComputeCircle(referencePoints);

				position = new Vector3(circleInfo.position.x, 0f, circleInfo.position.y);

				rotation = Quaternion.identity;

				scale = new Vector3 (circleInfo.radius * 2f,
											 circleInfo.radius * 2f,
											 circleInfo.radius * 2f);
				
				position.y += scale.y;
				
				obsData = CreateObstacleDataAndInstantiate(position, rotation, scale, _creatingType);

				ObstacleDataContainer.CurrentObstacleData.Add(obsData);

				break;
			
			default:
				Debug.LogWarning("Something is wrong with object types, object type case not found when creating object.");
				break;
		}

		SaveData();
	}

	ObstacleData CreateObstacleDataAndInstantiate(Vector3 position, Quaternion rotation, Vector3 scale, ObjectTypes objectType)
	{
		ObstacleData obsData = new ObstacleData();
		obsData.Position = position;
		obsData.Rotation = rotation;
		obsData.Scale = scale;
		obsData.ObjectType = objectType;

		InstantiateWithData(ref obsData);

		return obsData;
	}

	void InstantiateWithData(ref ObstacleData obsData)
	{
		obsData.ReferenceToObstacle = GameObject.Instantiate(
			_prefabs[(int)obsData.ObjectType], obsData.Position, obsData.Rotation, this.transform)
			.transform;
		obsData.ReferenceToObstacle.localScale = obsData.Scale;
	}

	ObstacleData InstantiateWithData(ObstacleData obsData)
	{

		obsData.ReferenceToObstacle = GameObject.Instantiate(_prefabs[(int)obsData.ObjectType], obsData.Position, obsData.Rotation, this.transform).transform;
		obsData.ReferenceToObstacle.localScale = obsData.Scale;
		return obsData;
	}

	void InstantiateAllData()
	{
		int count = ObstacleDataContainer.CurrentObstacleData.Count;
		for (int i = 0; i < count; ++i)
		{
			ObstacleDataContainer.CurrentObstacleData[i] = InstantiateWithData(ObstacleDataContainer.CurrentObstacleData[i]);
			//Debug.Log("Instantiated data: Position: " + obsData.Position + " Rotation: " + obsData.Rotation + " Scale: " + obsData.Scale + " ObjectType: " + obsData.ObjectType + " Reference: " + obsData.ReferenceToObstacle.name);
		}
	}

	public void StartCreating(ObjectTypes type)
	{
		Debug.Log("Starting creating an object of type: " + type.ToString());
		_enteringReferencePoint = 0;
		_creatingType = type;
		_creating = true;
	}

	public void CancelCreating()
	{
		Debug.Log("Cancel creating of an object");
		_creating = false;
	}

	private void SaveData()
	{
		// Perhaps change later to persistentDataPath or additional folders etc.
		

		string path = Path.Combine(Application.dataPath, SaveFile);
		Debug.Log("Saving data.");
		string saveData = JsonUtility.ToJson(ObstacleDataContainer);
		File.WriteAllText(path, saveData);
	}

	private void LoadData()
	{
		// Perhaps change later to persistentDataPath or additional folders etc.

		string path = Path.Combine(Application.dataPath, SaveFile);
		if (File.Exists(path))
		{
			Debug.Log("Loading saved data.");
			string loadedData = File.ReadAllText(path);
			ObstacleDataContainer = JsonUtility.FromJson<ObstacleContainer>(loadedData);
			InstantiateAllData();
		}
		else
		{
			Debug.Log("Saved data doesn't exist, creating a file for save data.");
			ObstacleDataContainer = new ObstacleContainer();

			string saveData = JsonUtility.ToJson(ObstacleDataContainer);
			File.WriteAllText(path, saveData);
		}
	}

	public static CircleInfo ComputeCircle(List<Vector2> l)
	{

 
		var n = l.Count;
		var sumx = 0f; l.ForEach(vec => { sumx += vec.x; });
		var sumxx = 0f; l.ForEach(vec => { sumxx += vec.x * vec.x; });
		var sumy = 0f; l.ForEach(vec => { sumy += vec.y; });
		var sumyy = 0f; l.ForEach(vec => { sumyy += vec.y * vec.y; });
		var sumxy = 0f; l.ForEach(vec => { sumxy += vec.x * vec.y; });
		var sumxxx = 0f; l.ForEach(vec => { sumxxx += vec.x * vec.x * vec.x; });
		var sumyyy = 0f; l.ForEach(vec => { sumyyy += vec.y * vec.y * vec.y; });
		var sumxxy = 0f; l.ForEach(vec => { sumxxy += vec.x * vec.x * vec.y; });
		var sumyyx = 0f; l.ForEach(vec => { sumyyx += vec.y * vec.y * vec.x; });

		var d11 = n * sumxy - sumx * sumy;
	
		var d20 = n * sumxx - sumx * sumx;
		var d02 = n * sumyy - sumy * sumy;
	
		var d30 = n * sumxxx - sumxx * sumx;
		var d03 = n * sumyyy - sumyy * sumy;
	
		var d21 = n * sumxxy - sumxx * sumy;
		var d12 = n * sumyyx - sumyy * sumx;
	
		var x = ((d30 + d12) * d02 - (d03 + d21) * d11) / (2 * (d20 * d02 - d11 * d11));
		var y = ((d03 + d21) * d20 - (d30 + d12) * d11) / (2 * (d20 * d02 - d11 * d11));
	
		var c = (sumxx + sumyy - 2 * x * sumx - 2 * y * sumy) / n;
		var r = Mathf.Sqrt(c + x * x + y * y);
	
		return new CircleInfo(new Vector2((float)x, (float)y), (float)r);
	}
	
}
