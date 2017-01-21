using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    float timeAtNextUpdate = 0;
    float timeAtNextSystemCleanup;

    public void Start()
    {
        GameObject.FindObjectOfType<MapGenerator>().GenerateLand();
        WorldGrid.Instance.Initialize();
        timeAtNextSystemCleanup = Time.time + GV.Water_Update_Time_Step*2; //bit of buffer for first cleanup
    }

    public void Update()
    {
        WorldGrid.Instance.tsunamiManager.UpdateTsunami();
        if (Time.time >= timeAtNextUpdate)
        {
            WorldGrid.Instance.waterManager.UpdateAllWater();
            timeAtNextUpdate += GV.Water_Update_Time_Step;
        }
        if(Time.time >= timeAtNextSystemCleanup)
        {
            WorldGrid.Instance.PreformSnapCleanup();
            timeAtNextSystemCleanup = Time.time + GV.System_Pillar_Cleanup_Interval;
        }
    }
}
