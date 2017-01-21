using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVisible : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateWorld()
    {
        for(int x=0; x<GV.World_Size_X; x++)
        {
            for(int z=0; z<GV.World_Size_Z; z++)
            {
                UpdatePillar(new Vector2(x, z));
            }
        }
    }

    public void UpdatePillar(Vector2 atLoc)
    {
        Pillar pillar = WorldGrid.Instance.GetPillarAt(atLoc, true);
        Vector2 neighbourLoc = new Vector2();
        float neighbourHeight;
        float minHeight = 9999;
        foreach (Vector2 dir in GV.Valid_Directions)
        {
            neighbourLoc = atLoc + dir;
            neighbourHeight = WorldGrid.Instance.GetHeightAt(neighbourLoc, true);
            if (neighbourHeight < minHeight)
                minHeight = neighbourHeight;
        }
        int segBelowMinHeight = pillar.FindSegmentBelow(minHeight);
        pillar.SetInvisibleBelow(segBelowMinHeight);
    }
}
