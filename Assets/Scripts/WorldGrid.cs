using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid  {
    #region Singleton
    private static WorldGrid instance;

    

    public static WorldGrid Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WorldGrid();
            }
            return instance;
        }
    }
    #endregion

    private WorldGrid() { waterManager = new WaterManager(); }

    WaterManager waterManager;

    public bool isMattTestScene = true;

	public Pillar[,] groundGrid = new Pillar[GV.World_Size_X, GV.World_Size_Y];
    public Pillar[,] waterGrid  = new Pillar[GV.World_Size_X, GV.World_Size_Y];

    public void Initialize() //used for default load in test scene
    {
        if(isMattTestScene)
        {
            foreach(Transform t in GameObject.FindObjectOfType<MattTest>().groundParent)
            {
                Pillar p = t.GetComponent<Pillar>();
                Vector3 loc = t.position;
                loc = new Vector3((int)loc.x, (int)loc.y, (int)loc.z);
                p.Initialize(loc, GV.PillarType.Ground);
                groundGrid[(int)loc.x, (int)loc.z] = p;
            }

            List<Pillar> waterPillars = new List<Pillar>();
            foreach (Transform t in GameObject.FindObjectOfType<MattTest>().waterParent)
            {
                Pillar p = t.GetComponent<Pillar>();
                Vector3 loc = t.position;
                loc = new Vector3((int)loc.x, (int)loc.y, (int)loc.z);
                p.Initialize(loc, GV.PillarType.Water);
                waterGrid[(int)loc.x, (int)loc.z] = p;
                waterPillars.Add(p);
            }
            waterManager.AddStaticActiveWater(waterPillars);
        }
    }

    public float GetHeightAt(Vector2 atLoc)
    {
        if (atLoc.x >= GV.World_Size_X || atLoc.x < 0 || atLoc.y >= GV.World_Size_Y || atLoc.y < 0)
        {
            Debug.Log("invalid grid loc: " + atLoc);
            return 9999;
        }

        if(waterGrid[(int)atLoc.x,(int)atLoc.y]) // if water exists
            return waterGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
        else
            return groundGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
    }



}
