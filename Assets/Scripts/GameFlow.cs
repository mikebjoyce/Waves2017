using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    float timeAtNextSystemCleanup;

	//public PlayerControl[] players = new PlayerControl[4];
	public PlayerControl p1;
    public CameraVisible cameraVisible;

    public void Start()
    {
        MapGenerator mapGen = GameObject.FindObjectOfType<MapGenerator>();
        mapGen.GenerateLand();
        while(!mapGen.LoadTiles()){}
        mapGen.OceanFiller();
        WorldGrid.Instance.Initialize();
        timeAtNextSystemCleanup = Time.time + GV.System_Pillar_Cleanup_Interval * 2; //bit of buffer for first cleanup
        cameraVisible.UpdateWorld();
		/*foreach (PlayerControl p in players) {
			

		}*/
		p1.Initialize ();
    }

    public void Update()
    {
        WorldGrid.Instance.tsunamiManager.UpdateTsunami();
        WorldGrid.Instance.waterManager.UpdateWaterManager();
        if(Time.time >= timeAtNextSystemCleanup)
        {
            WorldGrid.Instance.PreformSnapCleanup();
            timeAtNextSystemCleanup = Time.time + GV.System_Pillar_Cleanup_Interval;
        }
    }
}
