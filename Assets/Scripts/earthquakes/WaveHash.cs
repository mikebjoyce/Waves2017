using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHash : MonoBehaviour {

	public int wavelength;
	public int crestlimit;

	public int numberOfWaves;
	public int totalPossibleIterations;
	public int totalWaveDistance;

	private int _count;
	public int counter {set{_count = (value <= wavelength - 1)? value: 0;} get{return _count;}} //current collumn

	public int iteration = 0; 

    public int[,] waveArray; 
    public int[] initializer(int _waveLength)
    {
		//wavelength = _waveLength;
        //waveArray = new int[_waveLength + 1, _waveLength];
        int[] initializerCircles = new int[_waveLength];
        initializerCircles[0] = 1; //0 == 1
        initializerCircles[1] = -1;
        initializerCircles[2] = -3;
        initializerCircles[3] = -2;
        initializerCircles[4] = 0;
        initializerCircles[5] = 3;

        return (initializerCircles);
    } 

	public void InitializeArr(int _waveLength, int _crestLimit,int _numberOfWaves){
		wavelength = _waveLength;
		crestlimit = _crestLimit;
		waveArray = new int[_waveLength + 1, _waveLength];
		numberOfWaves = _numberOfWaves;
		totalPossibleIterations = numberOfWaves * wavelength;
		totalWaveDistance = retTotalWaveDistance (crestlimit, wavelength);
	}


	public void PopulateWaveArray()
    {
		//InitializeArr (wavelength); //happens b4 we populate anyways
        for (int y = 0; y < wavelength; y++)
        {
            waveArray[0, y] = initializer(wavelength)[y]; 
            for(int x = 1; x <= wavelength; x++)
            {
                if(y == 0)
                {
                    waveArray[x,y] = waveArray[x - 1, wavelength-1] - waveArray[x - 1, y];
                }
                else
                {
                    waveArray[x, y] = waveArray[x - 1, y - 1] - waveArray[x - 1, y];
                }
                
            }
        }
    }
		
	public int lowestBound(){
		return iteration - totalPossibleIterations;
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
