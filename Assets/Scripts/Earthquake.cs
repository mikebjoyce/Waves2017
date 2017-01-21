using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour {

	private List<Vector2> FindVoxelCircle(int xCenter, int zCenter, int radius)
	{
		int x = radius;
		int z = 0;
		int error = 0;
		List<Vector2> voxelPositions = new List<Vector2> ();

		while (x >= z) 
		{
			voxelPositions.Add (new Vector2 (xCenter + x, zCenter + z));
			voxelPositions.Add (new Vector2 (xCenter + z, zCenter + x));
			voxelPositions.Add (new Vector2 (xCenter - z, zCenter + x));
			voxelPositions.Add (new Vector2 (xCenter - x, zCenter + z));
			voxelPositions.Add (new Vector2 (xCenter - x, zCenter - z));
			voxelPositions.Add (new Vector2 (xCenter - z, zCenter - x));
			voxelPositions.Add (new Vector2 (xCenter + z, zCenter - x));
			voxelPositions.Add (new Vector2 (xCenter + x, zCenter - z));

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
		return(voxelPositions);
	}

	private void TestCircle(List<Vector2> points)
	{
		
		/*foreach (Vector2 _point in points) 
		{
			Vector3 pos = new Vector3 (_point.x, 0, _point.y);
			GameObject marker = (GameObject) Instantiate(Resources.Load ("Prefabs/markerPrefab"), pos, Quaternion.identity);
		}*/
			
	}

	// Use this for initialization
	void Start () {
		//TestCircle (FindVoxelCircle(0, 0, 50));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
