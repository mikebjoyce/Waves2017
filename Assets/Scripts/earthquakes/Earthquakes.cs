using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquakes : MonoBehaviour
{

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
        int waveCyclesRemaining = numOfWaveCycles;
        int waveRange = 0;
        while (_crestLimit > 0)
        {
            waveRange += _crestLimit * _wavelength;
            _crestLimit--;
        }
        List<Vector2>[] arrayOfCircles = new List<Vector2>[waveRange];
       
        arrayOfCircles = CircleArrayMaker(waveRange, _quakePos);

        gameObject.AddComponent<WaveHash>();
       
    }





    // Use this for initialization
    void Start()
    {
        CreateEarthquake(6, 3, 3, (Vector2.zero));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

