using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MattTest : MonoBehaviour {

    public Transform groundParent;
    public Transform waterParent;
    public List<Pillar> mainFountain;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            WorldGrid.Instance.waterManager.UpdateAllWater();
        }
        if (Input.GetKeyDown(KeyCode.Delete) && mainFountain.Count > 0)
        {
            WorldGrid.Instance.waterManager.Debug_DestroyStaticFountain(mainFountain[0]);
            mainFountain.RemoveAt(0);
        }


    }
   
}
