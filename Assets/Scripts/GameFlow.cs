using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    float timeAtNextSystemCleanup;

    //public PlayerControl[] players = new PlayerControl[4];
    public CameraVisible cameraVisible;
    public GameObject cinematicCamera;
    MapGenerator mapGen;
    RoundSetup roundSetup;

    bool worldIsLoaded = false;
    bool landIsLoaded = false;
    bool waterIsLoaded = false;
    int renderOrLoad = 0; //every 4 optimizes some stuff

    public Earthquakes earthQ;
    public GodsWhim god;

    public void Awake()
    {
        GV.gameFlow = this;
    }

    public void Start()
    {
        mapGen = GetComponent<MapGenerator>();
        roundSetup = GetComponent<RoundSetup>();
        WorldGrid.Instance.Initialize();
        mapGen.GenerateLand();

        //next step goes into update
    }

    public void FinishLoad()
    { //called after map loads
        timeAtNextSystemCleanup = Time.time + 12; //bit of buffer for first cleanup
        cameraVisible.UpdateWorld();
        //Debug.Log (p1.gameObject.isActiveAndEnabled);
        //p1.SetActive(true);
        //p1.GetComponentInChildren<PlayerControl> ().Initialize ();
        Destroy(cinematicCamera);
        if (GameVariable.activePlayerCntrls == null)
            roundSetup.SetupPlayers(new List<GameVariable.controlerType>() { GameVariable.controlerType.KeyLeft });//, GameVariable.controlerType.Joy1, GameVariable.controlerType.Joy2, GameVariable.controlerType.KeyRight });
        else
            roundSetup.SetupPlayers(GameVariable.activePlayerCntrls);
        if(god.Editor_Set_God_Mode)
            god.isOn = true;
        //Debug.Log (p1.isActiveAndEnabled);
    }

    private void LandCreationUpdate()
    {
        if (renderOrLoad < 3)
        {
            if (mapGen.tilesLoadedTwoUpdateAgo.Count > 0)
                cameraVisible.UpdatePartial(mapGen.tilesLoadedTwoUpdateAgo);
        }
        else
        {
            if (!landIsLoaded)
            {
                landIsLoaded = mapGen.LoadGroundTiles();
                if (landIsLoaded)
                    mapGen.OceanFiller();
            }
            else if (!waterIsLoaded)
            {
                waterIsLoaded = mapGen.LoadWaterTiles();
                if (waterIsLoaded)
                    FinishLoad();
            }
        }
        renderOrLoad++;
        renderOrLoad %= 4;
    }

    public void Update()
    {
        if (!landIsLoaded || !waterIsLoaded)
        {
            LandCreationUpdate();
        }
        else
        {
            WorldGrid.Instance.waterManager.UpdateWaterManager();
            WorldGrid.Instance.tsunamiManager.UpdateTsunami();
            //Update god and earthquakes should go here
            /*
            if (Time.time >= timeAtNextSystemCleanup)
            {
                cameraVisible.UpdateWorld();
                WorldGrid.Instance.PreformSnapCleanup();
                timeAtNextSystemCleanup = Time.time + GV.System_Pillar_Cleanup_Interval;
            }
            */
        }
    }
}
