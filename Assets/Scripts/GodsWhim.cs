using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsWhim : MonoBehaviour {
	List<Earthquakes> EarthQuakes = new List<Earthquakes> ();

    public bool Editor_Set_God_Mode = false;
    [HideInInspector]
    public bool isOn = false;
    bool dynamicGodMode = false;

	GameFlow owner;
	public Earthquakes EQ;
	public bool holdingBook = false;

    public float eventCountdown = 15;
    public float timeBetweenEvents = 15;
    public float timeBetweenEventsDimish = .5f; //every event time between events decreases
    public float probEventIsTsunami = 1f; //else its eq
    public float probEventIsCatacylsmic = .2f; //event is twice as strong
    public float godsCurrentRage = 0;
    float godsRagePerSec = .05f;
    float godsRagePerSecHoldingBook = .1f;
    public float godsMaxRage = 5;  //its a tier system
    public float godsCalmAfterEvent = .1f; //god cools off a bit after sending a event, *2 for cataclysmic


	float probablityTsunami = 15; // probability of Tsunami over 1 minute
	float intensityTsunami = 0.1f; //range 0 -> 1

	float probabilityEarthQuake = 15; //probability of EQ over 1 minute
	float intensityEQ = 0.1f; //range 0 - > 1

	float intensityIncPerSec = 2f;

	float tempIntensity = 0f;
	float book_intensityIncPerSec = 20f;

	public float realIntensityTsu(){
		return intensityTsunami + tempIntensity;
	}

	public float realIntensityEQ(){
		return intensityEQ + tempIntensity;
	}


	// Use this for initialization
	void Start () {
		owner = GetComponent<GameFlow> ();
		//EQ = GetComponent<Earthquakes> ();
	}

	// Update is called once per frame
	void Update () {
		if(isOn)
        {
            if (dynamicGodMode)
                DynamicGodUpdate();
            else
                StaticGodUpdate();
        }
	}
		
	private Vector2 fillEqLoc(){
			return new Vector2((int)Random.Range (0, GV.World_Size_X), (int)Random.Range (0, GV.World_Size_Z));
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

    private void DynamicGodUpdate()
    {
        intensityTsunami += intensityIncPerSec * Time.deltaTime;
        intensityEQ += intensityIncPerSec * Time.deltaTime;
        if (GV.theOneBook != null && GV.theOneBook.isHeld)
            tempIntensity += book_intensityIncPerSec * Time.deltaTime;
        else
            tempIntensity -= book_intensityIncPerSec * Time.deltaTime;
        if (tempIntensity < 0)
            tempIntensity = 0;

        float curIntensityEQ = realIntensityEQ();
        float curIntensityTsu = realIntensityTsu();


        if (Random.Range(0, 100) <= (probablityTsunami * Time.deltaTime))
        {
            Vector2[] tsuDir = directionToMapEdge();
            int riseIterations = (int)Mathf.Clamp(Random.Range((int)GV.GOD_RiseIteration[0] * curIntensityTsu, (int)GV.GOD_RiseIteration[1] * curIntensityTsu), (int)GV.GOD_RiseIteration[0], (int)GV.GOD_RiseIteration[1]);
            int steps = (int)Mathf.Clamp(Random.Range((int)GV.GOD_StepsInMax[0], GV.GOD_StepsInMax[1]), (int)GV.GOD_StepsInMax[0], (int)GV.GOD_StepsInMax[1]);
            int tempCur = (int)Mathf.Clamp(Mathf.Floor(curIntensityTsu / GV.GOD_flowRatePerPercentAngre), 1, 3);
            tempCur = (tempCur != 0) ? tempCur : 1;
            int repeats = (curIntensityTsu < 0.33) ? (int)Mathf.Ceil(Random.Range(GV.GOD_repeats[0], GV.GOD_repeats[1]) * curIntensityTsu) : 0;
            repeats = (int)Mathf.Clamp(repeats, GV.GOD_repeats[0], GV.GOD_repeats[1]);
            bool repB = (repeats != 0) ? true : false;
            WorldGrid.Instance.tsunamiManager.CreateLineTsunami(tsuDir[0], tsuDir[1], riseIterations, steps, tempCur, repB, repeats);
            //Debug.Log("Make Tsunami!");
        }
        if (Random.Range(0, 100) <= (probabilityEarthQuake * Time.deltaTime))
        {
            //Debug.Log("Make earthwuake!");
            Vector2 location = new Vector2((int)Random.Range((int)GV.World_Size_X / 4, (int)GV.World_Size_X - (int)GV.World_Size_X / 4), (int)Random.Range(GV.World_Size_X / 4, (int)GV.World_Size_Z - (int)GV.World_Size_X / 4));
            int wavelength = (int)Mathf.Clamp((Random.Range(2, 5) * curIntensityEQ), 2, 5);
            int crestLimit = (int)Mathf.Clamp((Random.Range(1, 3) * curIntensityEQ), 1, 3);
            int cycles = Mathf.Clamp((int)(Random.Range(1, 5) * curIntensityEQ), 1, 5);
            EQ.CreateEarthquake(6, 3, 1, fillEqLoc());
        }
    }

    private void StaticGodUpdate()
    {
        eventCountdown -= Time.deltaTime;
        godsCurrentRage += godsRagePerSec * Time.deltaTime;
        if (holdingBook)
            godsCurrentRage += godsRagePerSecHoldingBook * Time.deltaTime;

        if (eventCountdown <= 0)
        {
            timeBetweenEvents -= timeBetweenEventsDimish;
            eventCountdown = timeBetweenEvents;
            bool isCataclysmic = Random.Range(0, 1f) < probEventIsCatacylsmic;
            int sentRage = (isCataclysmic) ?(int)godsCurrentRage*2:(int)godsCurrentRage;
            if(Random.Range(0, 1f) < probEventIsTsunami)
            {
                SummonTsunamiEvent(sentRage);
            }
            else
            {
                SummonEarthquakeEvent(sentRage);
            }
            //summon event end
            float calmness = godsCurrentRage * godsCalmAfterEvent;
            if (isCataclysmic)
            {
                calmness *= 2;
                calmness = Mathf.Min(godsCurrentRage, calmness);
            }
            godsCurrentRage -= calmness;
        }
    }

    private void SummonTsunamiEvent(int eventLevel)
    {
        Vector2[] mapEdgePts = directionToMapEdge();
        eventLevel = Mathf.Clamp(eventLevel, 0, 4);
        int indexAtThatTier = Random.Range(0, tsunamiStorage[eventLevel].Count);
        Tsunami.ActiveTsunami tas = tsunamiStorage[eventLevel][indexAtThatTier];
        GV.Coord tsnumaiCord = (GV.Coord)Random.Range((int)0, 5);
        Debug.Log("tsunami lvl: " + eventLevel + " from dir: " + tsnumaiCord);
        WorldGrid.Instance.tsunamiManager.CreateTsunami(tas, tsnumaiCord );
    }

    private void SummonEarthquakeEvent(int eventLevel)
    {

    }

    private List<List<Tsunami.ActiveTsunami>> tsunamiStorage = new List<List<Tsunami.ActiveTsunami>>() //can populate the internal lists so can have multiple of single tier type
    {
        new List<Tsunami.ActiveTsunami> { new Tsunami.ActiveTsunami(3,3,1,true,2)},
        new List<Tsunami.ActiveTsunami> { new Tsunami.ActiveTsunami(5,5,2,true,3)},
        new List<Tsunami.ActiveTsunami> { new Tsunami.ActiveTsunami(3,2,6,true,4)},
        new List<Tsunami.ActiveTsunami> { new Tsunami.ActiveTsunami(6,7,4,false,0)},
        new List<Tsunami.ActiveTsunami> { new Tsunami.ActiveTsunami(7,7,7,true,2)}
    };
					
}


