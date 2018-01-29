using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSync : MonoBehaviour {

	bool beat = false;
	bool beatDir = false;
	float beatPulse = 0f;
	public float BeatDuration = 0.2f;
	public float BeatInterval = 0.5f;
	public float BeatRandomization = 0.5f;

	float timeToBeat;
	float timer = 0f;
	float beatStartTime = 0f;

	public float BeatStrength = 0.5f;

	List<Vector3> origScale = new List<Vector3>();
	List<Transform> beatingObjects = new List<Transform>();
	public static BeatSync Instance;

	bool initialized = false;

	// Use this for initialization
	void Awake () {
		if (Instance == null)
			Instance = this;
		else
		{
			Debug.Log("Please have only one BeatSync script in a scene!");
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// if (!initialized)
		// {
		// 	foreach (var trans in beatingObjects)
		// 	{
		// 		origScale.Add(trans.localScale);
		// 	}
		// 	initialized = true;
		// }
		
		timer += Time.deltaTime;
		if (timer >= timeToBeat && !beat)
		{
			beat = true;
			timer = 0f;
			timeToBeat = Random.Range(BeatInterval, BeatInterval + BeatRandomization);
			beatStartTime = Time.time;
		}
		
		if (beat)
		{
			if (!beatDir)
			{
				if (beatPulse > 0.48f)
					beatDir = true;
			}
			else
			{
				if (beatPulse < 0.1f)
				{
					beatDir = false;
					beat = false;
					beatPulse = 0f;
				}
			}

			if (beat)
				beatPulse = Mathf.PingPong(((Time.time - beatStartTime) / BeatDuration) * BeatStrength, BeatStrength);
		}

		for (int i = 0; i < beatingObjects.Count; ++i)
		{
			beatingObjects[i].localScale = origScale[i] * (1 + beatPulse);
		}

		Debug.Log(1f / Time.deltaTime);
	}

	public void AddObject(Transform trans)
	{
		beatingObjects.Add(trans);
		origScale.Add(trans.localScale);
	}
}
