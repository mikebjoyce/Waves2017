using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHash : MonoBehaviour {
	private int pCount = 0;
	public bool print = false;
	public bool debugger = false;


	public List<Vector2>[] arrayOfCircles;
	public int wavelength;
	public int crestlimit;

	public int numberOfWaves;
	public int totalPossibleIterations;
	public int totalWaveDistance;

	public int[] decayDist;

	private int _count;
	public int counter {set{_count = (value <= wavelength - 1)? value: 0;} get{return _count;}} //current collumn

	public int iteration = 0; 
	public bool isDone {get {return iteration - totalWaveDistance > totalPossibleIterations;}}

    public int[,] waveArray; 
    public int[] initializer(int _waveLength)
    {
		//wavelength = _waveLength;
        //waveArray = new int[_waveLength + 1, _waveLength];
        int[] initializerCircles = new int[_waveLength];


		initializerCircles[0] = 1; 
		initializerCircles[1] = -1;
		initializerCircles [2] = -3;
		initializerCircles[3] = -2;
		initializerCircles[4] = 0;
		initializerCircles[5] = 3;


/*
        initializerCircles[0] = 1; //0 == 1
        initializerCircles[1] = -1;
        initializerCircles[2] = -2;
        initializerCircles[3] = -2;
        initializerCircles[4] = 0;
        initializerCircles[5] = 2;
*/

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
        for (int y = 0; y < wavelength; y++)
        {
            waveArray[0, y] = initializer(wavelength)[y]; 
        }
			
		for (int y = 0; y < wavelength; y++) {
			int x = 1;
			if(y == 0)
			{
				waveArray[x,y] = waveArray[x - 1, wavelength - 1] -  waveArray[x - 1, y];
			}
			else
			{
				waveArray[x, y] = waveArray[x - 1, y - 1] -  waveArray[x - 1, y];
			}
		}

		for(int x = 2; x <= wavelength; x++)
		{
			for (int y = 0; y < wavelength; y++) {
				if(y == 0)
				{
					waveArray[x,y] = waveArray[x - 1, wavelength - 1];
				}
				else
				{
					waveArray[x, y] = waveArray[x - 1, y - 1];
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
		if (print)
			printDubArr (waveArray);
	}

	public void printDubArr(int[,] temp){
		for(int x = 0; x < temp.GetLength (0); x++){
			string xString = "";
			for(int y = 0; y < temp.GetLength(1); y++){
				xString += " ["+x+","+y+"] = "+temp[x,y].ToString()+"  ";
			}
			Debug.Log (xString);
		}
		print = false;
	}

	public int[] retDecayArr(int _crestLimit, int _wavelength){
		int[] decayInc = new int[_crestLimit];
		for (int i = 0; i < _crestLimit; i++) {
			decayInc [i] = (_crestLimit - i) * _wavelength;
		}
		return decayInc;
	}

	public int getDecayVal(int dist){
		//Debug.Log ("Decay arr " + decayDist.ToString());
		int element = 0;
		for (int c = 0; c < decayDist.Length; c++) {
			if (dist > decayDist [c])
				element = c;
		}
		//Debug.Log ("decay val ret " + element);
		return element;
	}

	public int modifyWaveRet(int deltaHight, int dist){
		int decayVal = getDecayVal (dist);
		if (deltaHight < 0) {
			int temp = (deltaHight + decayVal < 0) ? deltaHight + decayVal : 0;
			//Debug.Log ("1 base change " + deltaHight + " end change " + temp);
			return temp;
		} else if (deltaHight > 0) {
			int temp = (deltaHight - decayVal > 0) ? deltaHight - decayVal : 0;
			//Debug.Log ("2 base change " + deltaHight + " end change " + temp);
			return temp;
		} else {
			//Debug.Log ("3 base change " + deltaHight + " end change " + 0);
			return 0;
		}
	}

	public bool RunLoop(){
		if (!isDone) {
			int activeRows = (iteration < wavelength) ? iteration : wavelength; //array elements
			float howManyCycles = (activeRows != 0) ? Mathf.Floor (iteration / activeRows) : 0; //how many cycles b4 remainder
			float remainder = iteration - howManyCycles * (activeRows + 1); //remainder in array elements 
			for (int i = 0; i < howManyCycles; i++) {
				for (int c = activeRows; c >= 0; c--) {
					int currentCircle = c + (wavelength + 1) * i;
					if (currentCircle > lowestBound () && currentCircle < totalWaveDistance) {
						//Debug.Log ("moded by " + modifyWaveRet (waveArray [c, counter], c + (int)(wavelength + 1) * i));
						foreach (Vector2 v in arrayOfCircles[currentCircle]) {
							if (WorldGrid.Instance.InBounds (v)) {
								int tempWaveRet = modifyWaveRet (waveArray [c, counter], c + (int)(wavelength + 1) * i);
								if (tempWaveRet > 5 && debugger) {
									Debug.Log ("wave Arr ret > 5. Raw = " + waveArray [c, counter] +" Ret = " + tempWaveRet + " iteration " + iteration +"  Curr Circle " + currentCircle + " Time " + Time.time);
								}
								Pillar tempP = WorldGrid.Instance.GetPillarAt (v, true);
								tempP.ModHeight (tempWaveRet);
								//Debug.Log ("toModBy " + tempWaveRet);
							}
						}
					}
				}
			}
			for (int r = 0; r <= remainder; r++) {
				int currentCircle = r + (int)howManyCycles * (wavelength + 1);
				if (currentCircle > lowestBound () && currentCircle < totalWaveDistance) {
					//Debug.Log ("moded by " +modifyWaveRet (waveArray [r, counter], r + (int)howManyCycles * (wavelength + 1)));
					foreach (Vector2 v in arrayOfCircles[currentCircle]) {
						if (WorldGrid.Instance.InBounds (v)) {
							//Debug.Log ("Pillar " + WorldGrid.Instance.GetPillarAt (v, true).ToString ());
							//Debug.Log ("Vector 2 current pillar loc " + v.ToString());
							//Debug.Log ("currentEarthquake.waveArray [r, currentEarthquake.counter] " + currentEarthquake.waveArray [r, currentEarthquake.counter]);
							int tempWaveRet = modifyWaveRet (waveArray [r, counter], r + (int)howManyCycles * (wavelength + 1));
							Pillar tempP =	WorldGrid.Instance.GetPillarAt (v, true);
							tempP.ModHeight (tempWaveRet);
							//Debug.Log ("toModBy " +tempWaveRet);
						}
					}
				}
			}


			counter++;
			iteration++;
			return true;
		} else 
			return false;
	}

	/*
	public int retTotalWaveDistance(int _crestLimit, int _wavelength){
		int waveRange = 0;
		while (_crestLimit > 0)
		{
			waveRange += _crestLimit * _wavelength;
			_crestLimit--;
		}
		return waveRange;
	} */
}
