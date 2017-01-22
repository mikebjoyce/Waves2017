using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHash : MonoBehaviour {

    public int wavelength = 6;
    public int crestlimit = 3;

    public int[,] waveArray; 
    public int[] initializer(int _waveLength)
    {
        waveArray = new int[_waveLength + 1, _waveLength];
        int[] initializerCircles = new int[_waveLength];
        wavelength = _waveLength;
        initializerCircles[1] = 1;
        initializerCircles[2] = -1;
        initializerCircles[3] = -3;
        initializerCircles[4] = -2;
        initializerCircles[5] = 0;
        initializerCircles[6] = 3;

        return (initializerCircles);
    } 

     public void PopulateWaveArray()
    {
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
