using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	PooledObject prefab;
	List<PooledObject> availableObjects = new List<PooledObject>();

	// Use this for initialization
	public PooledObject GetObject() 
	{
		PooledObject obj;
		int lastAvailableIndex = availableObjects.Count - 1;
		if (lastAvailableIndex >= 0)
		{
			obj = availableObjects[lastAvailableIndex];
			availableObjects.RemoveAt(lastAvailableIndex);
			obj.gameObject.SetActive(true);
		} 
		else
		{
			obj = Instantiate<PooledObject>(prefab);
			obj.transform.SetParent(transform, false);
			obj.Pool = this;
		}
		return obj;
	}

	public void AddObject (PooledObject o)
	{
		o.gameObject.SetActive(false);
		availableObjects.Add(o);
	}

	public static ObjectPool GetPool (PooledObject prefab)
	{
		GameObject obj;
		ObjectPool pool;
		if (Application.isEditor)
		{
			obj = GameObject.Find(prefab.name + " Pool");
			if (obj)
			{
				pool = obj.GetComponent<ObjectPool>();
				if (pool)
					return pool;
			}
		}
		obj = new GameObject(prefab.name + " Pool");
		pool = obj.AddComponent<ObjectPool>();
		pool.prefab = prefab;
		return pool;
	}
}
