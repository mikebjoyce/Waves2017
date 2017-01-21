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
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector2 _from = new Vector2(GV.World_Size_X - 1, GV.World_Size_Z - 1);
            Vector2 _to = new Vector2(0, GV.World_Size_Z - 1);
            WorldGrid.Instance.tsunamiManager.CreateLineTsunami(_from, _to, 4, 5);
        }


    }
   
}
