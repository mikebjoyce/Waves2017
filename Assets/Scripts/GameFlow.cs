using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    float timeAtNextSystemCleanup;

	//public PlayerControl[] players = new PlayerControl[4];
	public PlayerControl p1;
    public CameraVisible cameraVisible;
    MapGenerator mapGen;
    bool worldIsLoaded = false;
    float renderCooldown = 1; //gives time for map to be drawn

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
        p1.gameObject.SetActive(true);
		//Debug.Log (p1.isActiveAndEnabled);
        p1.Initialize();
    }


    public void Update()
    {
        if (!worldIsLoaded)
        {
            renderCooldown -= Time.deltaTime;
            if (renderCooldown <= 0)
            {
                renderCooldown = 1;
                worldIsLoaded = mapGen.LoadTiles();
                if (worldIsLoaded)
                    FinishLoad();
            }
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
