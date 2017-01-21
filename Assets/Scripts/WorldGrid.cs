using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid  {
    #region Singleton
    private static WorldGrid instance;

    private WorldGrid() { }

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

    public bool isMattTestScene = false;

    public Pillar[,] groundGrid = new Pillar[(int)GV.World_Size.x, (int)GV.World_Size.y];
    public Pillar[,] waterGrid  = new Pillar[(int)GV.World_Size.x, (int)GV.World_Size.y];

    public void Awake() //used for default load in test scene
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
        }
    }

    public float GetHeightAt(Vector2 atLoc)
    {
        if(waterGrid[(int)atLoc.x,(int)atLoc.y]) // if water exists
            return waterGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
        else
            return groundGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
    }

}
