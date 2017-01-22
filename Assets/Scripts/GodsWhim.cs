using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsWhim : MonoBehaviour {
	GameFlow owner;
	public Earthquakes EQ;
	public bool holdingBook = false;

	float probablityTsunami = 100f; // probability of Tsunami over 1 minute
	float intensityTsunami = 0.1f; //range 0 -> 1


	float probabilityEarthQuake = 100f; //probability of EQ over 1 minute
	float intensityEQ = 0.1f; //range 0 - > 1

	float intensityIncPerSec = 2f;


	float tempIntensity = 0f;
	float book_intensityIncPerSec = 20f;


	// Use this for initialization
	void Start () {
		owner = GetComponent<GameFlow> ();
	}

	// Update is called once per frame
	void Update () {
		intensityTsunami += intensityIncPerSec * Time.deltaTime;
		intensityEQ += intensityIncPerSec * Time.deltaTime;
		//if (GV.)
		//	tempIntensity += book_intensityIncPerSec * Time.deltaTime;


		if (Random.Range ((int)0, 100) <= (int)(probablityTsunami * Time.deltaTime / 60)){
			Vector2[] tsuDir = directionToMapEdge ();
			int riseIterations = (int) Random.Range ((int)GV.GOD_RiseIteration[0]*intensityTsunami, (int) GV.GOD_RiseIteration[1]*intensityTsunami);
			int steps = (int) Random.Range ((int)GV.GOD_StepsInMax [0], GV.GOD_StepsInMax [1]);
			int tempCur = (int)Mathf.Floor (intensityTsunami / GV.GOD_flowRatePerPercentAngre);
			int current = (tempCur != 0) ? tempCur : 1;
			int repeats = (intensityTsunami < 0.33) ? (int) Mathf.Ceil(Random.Range(GV.GOD_repeats[0],GV.GOD_repeats[1]) * intensityTsunami) : 0;
			bool repB = (repeats != 0) ? true: false;
			WorldGrid.Instance.tsunamiManager.CreateLineTsunami (tsuDir[0],tsuDir[1],riseIterations,steps,current,repB,repeats);
			Debug.Log("Make Tsunami!");
		}
		if (Random.Range ((int)0, 100) <= (int)(probabilityEarthQuake * Time.deltaTime / 60)){
			Debug.Log("Make earthwuake!");
			Vector2 location = new Vector2 ((int)Random.Range (0, GV.World_Size_X), (int)Random.Range (0, GV.World_Size_Z));

		}

	}

	private Vector2[] directionToMapEdge(){
		Vector2[] dir = new Vector2[2];
		switch (Random.Range ((int)0, 3)) {
		case 0: 
			dir [0] = new Vector2 (GV.World_Size_X - 1, 0);
			dir [1] = new Vector2 (GV.World_Size_X - 1, GV.World_Size_Z - 1);
			break;
		case 1:
			dir [0] = new Vector2 (0, GV.World_Size_Z - 1);
			dir [1] = new Vector2 (GV.World_Size_X - 1, GV.World_Size_Z - 1);
			break;
		case 2:
			dir [0] = new Vector2 (GV.World_Size_X - 1, GV.World_Size_Z - 1);
			dir [1] = new Vector2 (0, GV.World_Size_Z - 1);
			break;
		case 3:
			dir [0] = new Vector2 (GV.World_Size_X - 1, GV.World_Size_Z - 1);
			dir [1] = new Vector2 (GV.World_Size_X - 1, 0);
			break;
		default:
			dir [0] = new Vector2 (GV.World_Size_X - 1, 0);
			dir [1] = new Vector2 (GV.World_Size_X - 1, GV.World_Size_Z - 1);
			break;

		}
		return dir;
	}
					
}


