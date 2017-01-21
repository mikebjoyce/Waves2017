using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int heightScale = 10; //higher values increase vertical intensity of relief 
	public int detailScale = 10; //lower values increase horizontal intensity of relief (density of hills)

	public void GenerateLand()
	{
		for (int x = 0; x < GV.World_Size_X; x++) 
		{
			//for(int z = 0; z < GV.World_Size_Y; z++)
		}
	}


	void Start () {
		
	}
	

}
