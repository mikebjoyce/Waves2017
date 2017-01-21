using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int heightScale = 10; //higher values increase vertical intensity of relief 
	public int detailScale = 10; //lower values increase horizontal intensity of relief (density of hills)
	public int terrainAltitudeConstant = 10;//shifts entire terrain upwards to make y=0 logical sea level
	public int centerProxBiasConstant = 3;//linearly scales strength of center proximity bias (how much world slopes down from center)

	public void GenerateLand()
	{
	if(detailScale != 0)
	{	
		int seed = (int)Network.time * 10; 
		for (int x = 0; x < GV.World_Size_X; x++) 
		{
			for (int z = 0; z < GV.World_Size_Y; z++)
			{       
					float centerProxFactor = CalculatePillarCenterProxFactor (x, z);
					y = (int)((Mathf.PerlinNoise((x + seed) / detailScale, (z + seed) / detailScale) * heightScale) + terrainAltitudeConstant - (centerProxFactor * centerProxBiasConstant));

			}
		}
	}
	else
		Debug.Log("detailScale cannot be 0!");
	}

	public float CalculatePillarCenterProxFactor(int _x, int _z) //uses pillar's proximity to map center to calculate altitude bias factor (makes the map slope downwards from center out)
	{

		float centerProxFactor = 0f;
		int xProx = Mathf.Abs (_x - WorldGrid.worldCenterPoint.x);
		int zProx = Mathf.Abs (_z - WorldGrid.worldCenterPoint.z);

		if (xProx >= zProx) 
		{
			
			centerProxFactor = xProx / WorldGrid.worldCenterPoint.x;
		}
		else
		{
			
			centerProxFactor = zProx / WorldGrid.worldCenterPoint.z;
		}

		return(centerProxFactor);
			
	}


	void Start () {
		
	}
	

}
