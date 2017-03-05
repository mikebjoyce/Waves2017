using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOut : MonoBehaviour {

    public int x = 0;
    public int y = 0;	
	// Update is called once per frame
	void Update ()
    {
        Pillar p = WorldGrid.Instance.GetPillarAt(new GridPos(x, y));
        Debug.Log(p.DebugOut());
	}
}
