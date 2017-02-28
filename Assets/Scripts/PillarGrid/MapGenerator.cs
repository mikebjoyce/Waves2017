using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int heightScale = 8; //higher values increase vertical intensity of relief 
	public int detailScale = 6; //lower values increase horizontal intensity of relief (density of hills)
	public int terrainAltitudeConstant = 5;//shifts entire terrain upwards to make y=0 logical sea level
	public int centerProxBiasConstant = 3;//linearly scales strength of center proximity bias (how much world slopes down from center)
	public bool heightScaleDecay = true;
	public float heightDecayControl = 3f;
	public float smoothDecay = .3f;
	public bool volcano = false;
	public float volcanoIntensity = .4f;
	public bool mapCenterImperfect = false;
	public float mapCenterRange = .2f;

	int xOffset = 0;
	int zOffset = 0;

    [HideInInspector]
    public List<Vector3> tilesToLoad;
    [HideInInspector]
    List<GridPos> toAddWater = new List<GridPos>();
    [HideInInspector]
    public List<Vector3> tilesLoadedOneUpdateAgo; //for graphics loading
    [HideInInspector]
    public List<Vector3> tilesLoadedTwoUpdateAgo;
    int tileLoadingIndex = 0;
    int waterTileLoadingIndex = 0;
    // public float loadWaitCycle = false;

    public void GenerateLand()
	{
        tilesToLoad = new List<Vector3>();
        tileLoadingIndex = 0;
        if (mapCenterImperfect)
        {
            int absoluteCenterRangeX = Mathf.RoundToInt(mapCenterRange * (GV.World_Size_X / 2));
            int absoluteCenterRangeZ = Mathf.RoundToInt(mapCenterRange * (GV.World_Size_Z / 2));
            xOffset = Random.Range(-absoluteCenterRangeX, absoluteCenterRangeX);
            zOffset = Random.Range(-absoluteCenterRangeZ, absoluteCenterRangeZ);
        }


        if (detailScale != 0)
	    {	
		    float seed = (float)Network.time * 10;  //real
			//float seed = (int)Network.time * 10; //flatworld
		    for (int x = 0; x < GV.World_Size_X; x++) 
		    {
		    	for (int z = 0; z < GV.World_Size_Z; z++)
		    	{       
		    			float centerProxFactor = CalculatePillarCenterProxFactor (x, z);
		    			float sqrtProxFactor = Mathf.Sqrt (centerProxFactor);
		    			float heightScaleDecayFactor = 1;
		    			if (!volcano)
		    				volcanoIntensity = 0f;


		    			if (heightScaleDecay)
		    				heightScaleDecayFactor = ((Mathf.Pow ((centerProxFactor - 1f - volcanoIntensity), 2)) + smoothDecay) * heightDecayControl;
		    		
		    			int y = Mathf.RoundToInt((Mathf.PerlinNoise((x + seed ) / (detailScale ), (z + seed) / (detailScale)) * heightScale * heightScaleDecayFactor) + terrainAltitudeConstant - (sqrtProxFactor * centerProxBiasConstant));
		    				
		    			Vector3 pillarPos = new Vector3 (x, y, z);
                        tilesToLoad.Add(pillarPos);
		    			//InstantiatePillar (pillarPos);
		    	}
		    }
	    }
	    else
	    	Debug.Log("detailScale cannot be 0!");

        
	}

	public float CalculatePillarCenterProxFactor(int _x, int _z) //uses pillar's proximity to map center to calculate altitude bias factor (makes the map slope downwards from center out)
	{

		float centerProxFactor = 0f;
		int xProx = (int) Mathf.Abs (_x - WorldGrid.worldCenterPoint.x + xOffset);
		int zProx = (int) Mathf.Abs (_z - WorldGrid.worldCenterPoint.y + zOffset);

		if (xProx >= zProx) 
		{
			
			centerProxFactor = xProx / WorldGrid.worldCenterPoint.x;
		}
		else
		{
			
			centerProxFactor = zProx / WorldGrid.worldCenterPoint.y;
		}
		if (centerProxFactor == 0f) 
		{
			centerProxFactor = 1/WorldGrid.worldCenterPoint.x;
		}

		return(centerProxFactor);
			
	}

	void InstantiatePillar(Vector3 _pillarPos)
	{
        if ((_pillarPos.x <= GV.Water_Sea_Width || _pillarPos.x >= GV.World_Size_X - GV.Water_Sea_Width) || (_pillarPos.z <= GV.Water_Sea_Width || _pillarPos.z >= GV.World_Size_Z - GV.Water_Sea_Width))
        {
            _pillarPos.y = -15; //Outmost edges are dropped super low for the sea
        }

        WorldGrid.Instance.worldGrid[(int)_pillarPos.x, (int)_pillarPos.z].InitializePillarStruct(GV.PillarType.Ground, _pillarPos.y);
    }

    public void OceanFiller()
    {
        List<GridPos> openList = new List<GridPos>();
        List<GridPos> closedList = new List<GridPos>();

        //start at inner ring, since outer ring has sea tiles already
        for (int x = 0; x < GV.World_Size_X; x++)
        {
            openList.Add(new GridPos(x, 0));
            openList.Add(new GridPos(x, GV.World_Size_X - 1));
        }
        for (int z = 0; z < GV.World_Size_Z; z++)
        {
            openList.Add(new GridPos(0,z));
            openList.Add(new GridPos(GV.World_Size_Z - 1,z));
        }

        while(openList.Count > 0)
        {
           if(WorldGrid.Instance.GetPillarStaticHeight(openList[0], GV.PillarType.Ground) < GV.Water_Sea_Level)
           {
                if(!toAddWater.Contains(openList[0]))
                    toAddWater.Add(openList[0]);
                foreach(GridPos offset in GV.Valid_Directions)
                {
                    GridPos posToCheck = openList[0] + offset;
                    if(WorldGrid.Instance.InBounds(posToCheck))
                        if (!openList.Contains(posToCheck) && !closedList.Contains(posToCheck) && !toAddWater.Contains(posToCheck))
                            openList.Add(openList[0] + offset);
                }
            
           }
           closedList.Add(openList[0]);
           openList.RemoveAt(0);
        }
    }

    public bool LoadWaterTiles()
    {
        for (int i = 0; waterTileLoadingIndex < toAddWater.Count && i < GV.MapGen_Tiles_Load_Per_Cycle; i++, waterTileLoadingIndex++)
            WorldGrid.Instance.GetPillarAt(toAddWater[waterTileLoadingIndex]).InitializePillarStruct(GV.PillarType.Water, GV.Water_Sea_Level);
        return !(waterTileLoadingIndex < toAddWater.Count); //if not less than, returns true to indicate its finished
    }

    public bool LoadGroundTiles()
    {
        float startTime = Time.time;
        tilesLoadedTwoUpdateAgo = new List<Vector3>(tilesLoadedOneUpdateAgo);
        tilesLoadedOneUpdateAgo = new List<Vector3>();
        for (int i = 0; tileLoadingIndex < tilesToLoad.Count && i < GV.MapGen_Tiles_Load_Per_Cycle; i++, tileLoadingIndex++)
        {
            tilesLoadedOneUpdateAgo.Add(tilesToLoad[tileLoadingIndex]);
            InstantiatePillar(tilesToLoad[tileLoadingIndex]);
        }
        float endTime = Time.time - startTime;
        if (endTime < GV.MapGen_Ideal_Time_Per_cycle)
            GV.MapGen_Tiles_Load_Per_Cycle += GV.MapGen_Tiles_Load_Bonus;
        else if(GV.MapGen_Tiles_Load_Per_Cycle > 20)
            GV.MapGen_Tiles_Load_Per_Cycle -= GV.MapGen_Tiles_Load_Bonus;

        return tileLoadingIndex >= tilesToLoad.Count;
    }
}
