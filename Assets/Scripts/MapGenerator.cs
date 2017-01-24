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
    List<Vector2> toAddWater = new List<Vector2>();
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
            _pillarPos.y = -10;
        }

        GameObject newPillar = (GameObject)Instantiate (Resources.Load ("Prefabs/Pillar"), _pillarPos, Quaternion.identity);
        newPillar.transform.SetParent(GV.worldLinks.groundParent);
        Pillar p = newPillar.GetComponent<Pillar>();
        WorldGrid.Instance.groundGrid [(int)_pillarPos.x,(int)_pillarPos.z] = p;
        p.Initialize(_pillarPos, GV.PillarType.Ground);

        if ((_pillarPos.x == 0 || _pillarPos.x == GV.World_Size_X - 1) || (_pillarPos.z == 0 || _pillarPos.z == GV.World_Size_Z - 1)) //is edge sea piece
        {
            //Vector3 seaPos = _pillarPos;
            //seaPos.y = GV.Water_Sea_Level;
            WorldGrid.Instance.waterManager.CreateWater(new Vector2(_pillarPos.x, _pillarPos.z), GV.Water_Sea_Level, true);
            //GameObject seaPillarObj = (GameObject)Instantiate(Resources.Load("Prefabs/Pillar"), seaPos, Quaternion.identity);
            //seaPillarObj.transform.SetParent(GameObject.FindObjectOfType<WorldLinks>().waterParent);
            //Pillar seaPillar = seaPillarObj.GetComponent<Pillar>();
            //seaPillar.Initialize(seaPos, GV.PillarType.Water);
            //WorldGrid.Instance.groundGrid[(int)seaPos.x, (int)seaPos.z] = newPillar.GetComponent<Pillar>();
        }

    }

    public void OceanFiller()
    {
        List<Vector2> openList = new List<Vector2>();
        List<Vector2> closedList = new List<Vector2>();
        

        //start at inner ring, since outer ring has sea tiles already
        for (int x = 1; x < GV.World_Size_X; x++)
        {
            openList.Add(new Vector2(x, 1));
            openList.Add(new Vector2(x, GV.World_Size_X - 2));
        }
        for (int z = 1; z < GV.World_Size_Z; z++)
        {
            openList.Add(new Vector2(1,z));
            openList.Add(new Vector2(GV.World_Size_Z - 2,z));
        }

        while(openList.Count > 0)
        {
           if(WorldGrid.Instance.GetHeightAt(openList[0]) < GV.Water_Sea_Level)
           {
                toAddWater.Add(openList[0]);
                foreach(Vector2 offset in GV.Valid_Directions)
                {
                    if (!openList.Contains(openList[0] + offset) && !closedList.Contains(openList[0] + offset) && !toAddWater.Contains(openList[0] + offset))
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
            WorldGrid.Instance.waterManager.CreateWater(toAddWater[waterTileLoadingIndex], GV.Water_Sea_Level, false);
        return waterTileLoadingIndex == toAddWater.Count - 1;
    }

    public bool LoadTiles()
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
