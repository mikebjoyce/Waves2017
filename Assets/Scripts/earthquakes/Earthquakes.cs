using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquakes : MonoBehaviour
{
	public bool isActive = false;
	public WaveHash currentEarthquake;
	public List<Vector2>[] arrayOfCircles;
	public int[] decayDist;

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
		
        //int waveCyclesRemaining = numOfWaveCycles;
		int waveRange = retTotalWaveDistance(_crestLimit, _wavelength);

		decayDist = retDecayArr (_crestLimit, _wavelength);

		arrayOfCircles = new List<Vector2>[waveRange];
        arrayOfCircles = CircleArrayMaker(waveRange, _quakePos);

		currentEarthquake = gameObject.AddComponent<WaveHash>();
		currentEarthquake.InitializeArr (_wavelength, _crestLimit, numOfWaveCycles);
		currentEarthquake.PopulateWaveArray ();
		isActive = true;

		//currentEarthquake.waveArray
    }

	public int[] retDecayArr(int _crestLimit, int _wavelength){
		int[] decayInc = new int[_crestLimit];
		for (int i = 0; i < _crestLimit; i++) {
			decayInc [i] = (_crestLimit - i) * _wavelength;
		}
		return decayInc;
	}

	public int getDecayVal(int dist){
		for (int c = 0; c < decayDist.Length; c++) {
			if (dist < decayDist [c])
				return c - 1;
		}
		return decayDist.Length - 1;
	}

	public int modifyWaveRet(int deltaHight, int dist){
		int decayVal = getDecayVal (dist);
		if (deltaHight < 0) {
			return deltaHight + decayVal;
		} else if (deltaHight > 0) {
			return deltaHight - decayVal;
		} else {
			return 0;
		}
	}

	public int retTotalWaveDistance(int _crestLimit, int _wavelength){
		int waveRange = 0;
		while (_crestLimit > 0)
		{
			waveRange += _crestLimit * _wavelength;
			_crestLimit--;
		}
		return waveRange;
	}

	private void RunLoop(){
		int activeRows = (currentEarthquake.iteration < currentEarthquake.wavelength) ? currentEarthquake.iteration : currentEarthquake.wavelength; //array elements
		float howManyCycles = (activeRows != 0) ? Mathf.Floor(currentEarthquake.iteration / activeRows) : 0; //how many cycles b4 remainder
		float remainder =  currentEarthquake.iteration - howManyCycles * (activeRows + 1); //remainder in array elements 
		for (int i = 0; i < howManyCycles; i++) {
			for (int c = activeRows; c >= 0; c--) {
				int currentCircle = c + (currentEarthquake.wavelength + 1) * i;
				if (currentCircle > currentEarthquake.lowestBound() && currentCircle < currentEarthquake.totalWaveDistance) {
					foreach (Vector2 v in arrayOfCircles[currentCircle]) {
						WorldGrid.Instance.GetPillarAt (v, true).ModHeight (modifyWaveRet (currentEarthquake.waveArray [c, currentEarthquake.counter], c + (int) (currentEarthquake.wavelength + 1) * i));
					}
				}
			}
		}
		for (int r = 0; r <= remainder; r++) {
			int currentCircle = r + (int) howManyCycles * (currentEarthquake.wavelength + 1);
			if (currentCircle > currentEarthquake.lowestBound () && currentCircle < currentEarthquake.totalWaveDistance) {
				foreach (Vector2 v in arrayOfCircles[currentCircle]) {
					WorldGrid.Instance.GetPillarAt (v, true).ModHeight (modifyWaveRet (currentEarthquake.waveArray [r - 1, currentEarthquake.counter], r + (int) howManyCycles * (currentEarthquake.wavelength + 1)));
				}
			}
		}
		currentEarthquake.counter++;
		currentEarthquake.iteration++;
	}
		
    // Use this for initialization
    void Start()
    {
        CreateEarthquake(6, 3, 3, (Vector2.zero));
    }

    // Update is called once per frame
    void Update()
    {
		if (lastUpdateTime + GV.Earthquake_Tick_Length < Time.time) {
			//update
			//update last
			RunLoop();
			lastUpdateTime = Time.time;
		}
    }
}

