﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MattTest : MonoBehaviour {

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector2 _from = new Vector2(GV.World_Size_X - 1, GV.World_Size_Z - 1);
            Vector2 _to = new Vector2(0, GV.World_Size_Z - 1);
            WorldGrid.Instance.tsunamiManager.CreateLineTsunami(_from, _to,3,5,3,false,0);
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log(string.Format("Update exit causes:  Time({0}), Full Loop({1})", WaterManager.timeExit, WaterManager.loopExit));
        }
        
    }
   
}
