using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MattTest : MonoBehaviour {

    public Transform groundParent;
    public Transform waterParent;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            WorldGrid.Instance.waterManager.UpdateAllWater();
        }


    }
   
}
