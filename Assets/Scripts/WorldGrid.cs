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

	static public Vector2 worldCenterPoint = new Vector2((int)(GV.World_Size_X)/2 ,(int) (GV.World_Size_Z)/2);

    public bool isMattTestScene = true;

	public Pillar[,] groundGrid = new Pillar[GV.World_Size_X, GV.World_Size_Z];
    public Pillar[,] waterGrid  = new Pillar[GV.World_Size_X, GV.World_Size_Z];

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

    public float GetHeightAt(Vector2 atLoc, bool groundOnly = false)
    {
        if (atLoc.x >= GV.World_Size_X || atLoc.x < 0 || atLoc.y >= GV.World_Size_Z || atLoc.y < 0)
            return 9999;

        if(!groundOnly && waterGrid[(int)atLoc.x,(int)atLoc.y]) // if water exists
            return waterGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
        else
            return groundGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
    }



}
