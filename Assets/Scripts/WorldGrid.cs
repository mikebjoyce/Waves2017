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

	public Pillar[,] groundGrid = new Pillar[GV.World_Size_X, GV.World_Size_Z];
    public Pillar[,] waterGrid  = new Pillar[GV.World_Size_X, GV.World_Size_Z];

    public void Initialize() //used for default load in test scene
    {
        foreach(Transform t in GV.worldLinks.groundParent)
        {
            Pillar p = t.GetComponent<Pillar>();
            Vector3 loc = t.position;
            loc = new Vector3((int)loc.x, (int)loc.y, (int)loc.z);
            p.Initialize(loc, GV.PillarType.Ground);
            groundGrid[(int)loc.x, (int)loc.z] = p;
        }
        /*
        List<Pillar> waterPillars = new List<Pillar>();
        foreach (Transform t in GameObject.FindObjectOfType<WorldLinks>().waterParent)
        {
            Pillar p = t.GetComponent<Pillar>();
            Vector3 loc = t.position;
            loc = new Vector3((int)loc.x, (int)loc.y, (int)loc.z);
            p.Initialize(loc, GV.PillarType.Water);
            waterGrid[(int)loc.x, (int)loc.z] = p;
            waterPillars.Add(p);
        }
        waterManager.AddStaticActiveWater(waterPillars);*/
        
    }

    public void ModGround(Vector2 atLoc, float amt)
    {
        if (InBounds(atLoc))
        {
            groundGrid[(int)atLoc.x, (int)atLoc.y].ModHeight(amt);
            if (waterGrid[(int)atLoc.x, (int)atLoc.y]) // if water exists
                waterGrid[(int)atLoc.x, (int)atLoc.y].ModHeight(amt);

            CameraVisible cv = GV.gameFlow.cameraVisible;

            cv.UpdateLocation(atLoc);
            foreach(Vector2 dir in GV.Valid_Directions)
            {
                if(GetPillarAt(atLoc + dir,true))
                    cv.UpdateLocation(atLoc + dir);
            }
        }
    }

    public List<Pillar> GetAllNeighbors(Vector2 atLoc)
    {
        List<Pillar> toRet = new List<Pillar>();
        foreach (Vector2 dir in GV.Valid_Directions)
        {
            Pillar p = GetPillarAt(atLoc + dir);
            if (p)
                toRet.Add(p);
        }
        return toRet;
    }

    public Pillar GetPillarAt(Vector2 atLoc, bool groundOnly = false)
    { //returns the highest one, ground or water
		if (!InBounds(atLoc))
			return null;

        if (!groundOnly && waterGrid[(int)atLoc.x, (int)atLoc.y]) // if water exists
        {
            return waterGrid[(int)atLoc.x, (int)atLoc.y];
        }
        else
            return groundGrid[(int)atLoc.x, (int)atLoc.y];
    }

    public void PreformSnapCleanup() //every once in a while, things build up tiny (like E-8) float values that mess calcs
    {
        for(int x = 0; x < GV.World_Size_X; x++)
            for (int z = 0; z < GV.World_Size_Z; z++)
            {
                Pillar groundPillar = GetPillarAt(new Vector2(x, z), true);
                CleanPillar(groundPillar);
                Pillar waterPillar = GetPillarAt(new Vector2(x, z));
                if (waterPillar != groundPillar)
                    CleanPillar(waterPillar);
            }
    }

    private void CleanPillar(Pillar toClean)
    {
        toClean.pos.x = (int)MathHelper.RoundFloat(toClean.pos.x, 1);
        if(toClean.pillarType == GV.PillarType.Ground)
            toClean.pos.y = (int)MathHelper.RoundFloat(toClean.pos.y, 1);
        else
            toClean.pos.y = MathHelper.RoundFloat(toClean.pos.y, 1);
        toClean.pos.z = (int)MathHelper.RoundFloat(toClean.pos.z, 1);
        toClean.SetHeight(toClean.pos.y);
    }

    private bool InBounds(Vector2 atLoc)
    {
        return !(atLoc.x >= GV.World_Size_X || atLoc.x < 0 || atLoc.y >= GV.World_Size_Z || atLoc.y < 0) ;
    }

    public float GetHeightAt(Vector2 atLoc, bool groundOnly = false)
    {
        if (!InBounds(atLoc))
            return 9999;

        Pillar waterPillar = waterGrid[(int)atLoc.x, (int)atLoc.y];
        if (!groundOnly && waterPillar) // if water exists
        {
            /*if (waterManager.staticPillars.Contains(waterPillar))
                return 9999;
            else*/
                return waterGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
        }
        else
            return groundGrid[(int)atLoc.x, (int)atLoc.y].GetHeight();
    }



}
