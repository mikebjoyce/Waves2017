using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    float timeAtNextUpdate = 0;

    public void Start()
    {
        GameObject.FindObjectOfType<MapGenerator>().GenerateLand();
        WorldGrid.Instance.Initialize();
    }

    public void Update()
    {
        WorldGrid.Instance.tsunamiManager.UpdateTsunami();
        if (Time.time >= timeAtNextUpdate)
        {
            WorldGrid.Instance.waterManager.UpdateAllWater();
            timeAtNextUpdate += GV.Water_Update_Time_Step;
        }
    }
}
