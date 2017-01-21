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



	public void GenerateLand()
	{
	if(detailScale != 0)
	{	
		float seed = (float)Network.time * 10; 
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
					InstantiatePillar (pillarPos);
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
			
			centerProxFactor = xProx / WorldGrid.worldCenterPoint.x + xOffset;
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
		GameObject newPillar = (GameObject)Instantiate (Resources.Load ("Prefabs/Pillar"), _pillarPos, Quaternion.identity);
		WorldGrid.Instance.groundGrid [(int)_pillarPos.x,(int)_pillarPos.z] = newPillar.GetComponent<Pillar>(); 
	}

	void Start () {
		if (mapCenterImperfect) 
		{
			int absoluteCenterRangeX = Mathf.RoundToInt (mapCenterRange * (GV.World_Size_X / 2));
			int absoluteCenterRangeZ = Mathf.RoundToInt (mapCenterRange * (GV.World_Size_Z / 2));
			int xOffset = Random.Range (-absoluteCenterRangeX, absoluteCenterRangeX);
			int zOffset = Random.Range (-absoluteCenterRangeZ, absoluteCenterRangeZ);
		}

		GenerateLand ();
		
	}
	

}
