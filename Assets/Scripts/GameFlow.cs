﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    float timeAtNextSystemCleanup;

	//public PlayerControl[] players = new PlayerControl[4];
	public GameObject p1;
    public CameraVisible cameraVisible;
    MapGenerator mapGen;
    bool worldIsLoaded = false;
    int renderOrLoad = 0; //every 4 optimizes some stuff

    public void Awake()
    {
        GV.gameFlow = this;
    }

    public void Start()
    {
        mapGen = GameObject.FindObjectOfType<MapGenerator>();
        mapGen.GenerateLand();
        //next step goes into update
    }

    public void FinishLoad()
    { //called after map loads
        mapGen.OceanFiller();
        WorldGrid.Instance.Initialize();
        timeAtNextSystemCleanup = Time.time + GV.System_Pillar_Cleanup_Interval * 2; //bit of buffer for first cleanup
        cameraVisible.UpdateWorld();
		//Debug.Log (p1.gameObject.isActiveAndEnabled);
        p1.SetActive(true);
		p1.GetComponentInChildren<PlayerControl> ().Initialize ();

		//Debug.Log (p1.isActiveAndEnabled);
    }


    public void Update()
    {
        if (!worldIsLoaded)
        {
            if(renderOrLoad < 3)
            {
                
                if (mapGen.tilesLoadedTwoUpdateAgo.Count > 0)
                    cameraVisible.UpdatePartial(mapGen.tilesLoadedTwoUpdateAgo);
            }
            else
            {
                worldIsLoaded = mapGen.LoadTiles();
                if (worldIsLoaded)
                    FinishLoad();
            }
            renderOrLoad++;
            renderOrLoad %= 4;
        }
        else
        {
            WorldGrid.Instance.tsunamiManager.UpdateTsunami();
            WorldGrid.Instance.waterManager.UpdateWaterManager();
            if (Time.time >= timeAtNextSystemCleanup)
            {
                WorldGrid.Instance.PreformSnapCleanup();
                timeAtNextSystemCleanup = Time.time + GV.System_Pillar_Cleanup_Interval;
            }
        }
    }
}
