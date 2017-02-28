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
    
    private WorldGrid() { waterManager = new WaterManager(); tsunamiManager = new Tsunami(); }

    public WaterManager waterManager;
    public Tsunami tsunamiManager;

    static public Vector2 worldCenterPoint = new Vector2((int)(GV.World_Size_X)/2 ,(int) (GV.World_Size_Z)/2);

    public bool isMattTestScene = false;

	public Pillar[,] worldGrid = new Pillar[GV.World_Size_X, GV.World_Size_Z];
    public Pillar[,] waterGrid  = new Pillar[GV.World_Size_X, GV.World_Size_Z];

    public void Initialize() //used for default load in test scene
    {
        for (int x = 0; x < GV.World_Size_X; x++)
            for (int y = 0; y < GV.World_Size_Z; y++)
                worldGrid[x, y] = new Pillar(new GridPos(x, y));
    }

    public void ModPillarHeight(GridPos atLoc, GV.PillarType pillarType, float amt)
    {
        if (InBounds(atLoc))
            worldGrid[atLoc.x, atLoc.y].ModHeight(pillarType, amt);
    }

    public void DisturbTargetAndNeighbors(GridPos atLoc)
    {
        foreach (GridPos dir in GV.Valid_Directions)
        {
            GridPos pos = atLoc + dir;
            if (InBounds(pos))
                GetPillarAt(pos).SetDisturbed(true);
        }
        GetPillarAt(atLoc).SetDisturbed(true);
    }

    public float GetPillarStaticHeight(GridPos atLoc, GV.PillarType ptype = GV.PillarType.Water) //water will give you tallest height
    {
        if (!InBounds(atLoc))
            return 999;
        return worldGrid[atLoc.x, atLoc.y].GetStaticHeight(ptype);
    }
    

    public Pillar GetPillarAt(GridPos atLoc)
    { //returns the highest one, ground or water
		if (!InBounds(atLoc))
			return null;
        return worldGrid[atLoc.x, atLoc.y];
    }

    public PillarStruct GetPillarStructAt(GridPos atLoc,GV.PillarType _ptype)
    {
        if (!InBounds(atLoc))
            return null;

        return worldGrid[atLoc.x, atLoc.y].GetPillarStruct(_ptype);
    }

    public void PreformSnapCleanup() //every once in a while, things build up tiny (like E-8) float values that mess calcs
    {
        for(int x = 0; x < GV.World_Size_X; x++)
            for (int z = 0; z < GV.World_Size_Z; z++)
                GetPillarAt(new GridPos(x, z)).CleanPillar();
    }

    public bool InBounds(GridPos atLoc)
    {
        return !(atLoc.x >= GV.World_Size_X || atLoc.x < 0 || atLoc.y >= GV.World_Size_Z || atLoc.y < 0) ;
    }

}
