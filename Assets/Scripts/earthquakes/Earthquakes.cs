using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquakes : MonoBehaviour
{
	public CameraVisible refresher;
	public bool isActive = false;
	public WaveHash currentEarthquake;
	public List<WaveHash> currentEQs = new List<WaveHash>();

	private float lastUpdateTime = 0;
	//GV.Earthquake_Tick_Length;

    private List<Vector2> FindVoxelCircle(int xCenter, int zCenter, int radius)
    {
        int x = radius;
        int z = 0;
        int error = 0;
        List<Vector2> voxelPositions = new List<Vector2>();

        while (x >= z)
        {
            voxelPositions.Add(new Vector2(xCenter + x, zCenter + z));
            voxelPositions.Add(new Vector2(xCenter + z, zCenter + x));
            voxelPositions.Add(new Vector2(xCenter - z, zCenter + x));
            voxelPositions.Add(new Vector2(xCenter - x, zCenter + z));
            voxelPositions.Add(new Vector2(xCenter - x, zCenter - z));
            voxelPositions.Add(new Vector2(xCenter - z, zCenter - x));
            voxelPositions.Add(new Vector2(xCenter + z, zCenter - x));
            voxelPositions.Add(new Vector2(xCenter + x, zCenter - z));

            if (error <= 0)
            {
                z += 1;
                error += 2 * z + 1;
            }
            if (error > 0)
            {
                x -= 1;
                error -= 2 * x + 1;
            }
        }
        return (voxelPositions);
    }

    private List<Vector2>[] CircleArrayMaker(int waveRange, Vector2 quakePos)
    {
        List<Vector2>[] arrayOfCircles = new List<Vector2>[waveRange];
        
        for (int i = 0; i < waveRange; i++)
        {
            arrayOfCircles[i] = FindVoxelCircle((int)quakePos.x,(int) quakePos.y, i + 1);
        }
        return (arrayOfCircles);
    }

    public void CreateEarthquake(int _wavelength, int _crestLimit, int numOfWaveCycles, Vector2 _quakePos)
    {

        currentEarthquake = gameObject.AddComponent<WaveHash>();
		currentEarthquake.InitializeArr (_wavelength, _crestLimit, numOfWaveCycles);
        //int waveCyclesRemaining = numOfWaveCycles;
		int waveRange = currentEarthquake.retTotalWaveDistance(_crestLimit, _wavelength);

		currentEarthquake.decayDist = currentEarthquake.retDecayArr (_crestLimit, _wavelength);

		currentEarthquake.arrayOfCircles = new List<Vector2>[waveRange];
		currentEarthquake.arrayOfCircles = CircleArrayMaker(waveRange, _quakePos);

		currentEarthquake.PopulateWaveArray ();
		isActive = true;
		//currentEarthquake.waveArray
		currentEQs.Add(currentEarthquake);
    }

		
    // Use this for initialization
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		bool toRem = false;
		WaveHash objToRem = null;
		if (lastUpdateTime + GV.Earthquake_Tick_Length < Time.time && isActive) {
			if (currentEQs.Count != 0) {
				foreach (WaveHash earthQ in currentEQs) {
					toRem = !earthQ.RunLoop ();
					objToRem = earthQ;
					lastUpdateTime = Time.time;
				}
				if(toRem && objToRem != null)
					currentEQs.Remove (objToRem);
			} else {
				isActive = false;
			}
            refresher.UpdateWorld();
        }
    }
}

